using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using ExcelAccountsManager;
using System.Data.SqlClient;

namespace ExcelDataUpload
{
    class DataLoader : IDisposable
    {
        private Application _app = new Microsoft.Office.Interop.Excel.Application();
        SqlConnection _connection;
        ExcelBookHolder _bookHolder;
        DateTime _dtValuationDate;

        public DataLoader(string path, string dbconn, DateTime dtValuationDate) 
        {
            //first load the cash account and investment record books for the curent year
            string InvestmentRecordFile = string.Format(@"{0}\{1}-{2}", path, ExcelBookHolder.InvestmentRecordName, dtValuationDate.Year);
            string CashAccountFile = string.Format(@"{0}\{1}-{2}", path, ExcelBookHolder.CashAccountName, dtValuationDate.Year);
            _bookHolder = new ExcelBookHolder(_app, InvestmentRecordFile, CashAccountFile, path, dtValuationDate);
            _connection = new SqlConnection(dbconn);
            _connection.Open();
            _dtValuationDate = dtValuationDate;
        }

        public void Dispose()
        {
            _bookHolder.Dispose();
            _connection.Close();
        }

        public void LoadData()
        {
           //now upload the investment record data
            _UploadInvestmentRecordData();
            _UploadMemberCapitalAccountData();
            _UploadUnitPriceData();
        }

        private void _UpdateMembersCapitalAccountTable(DateTime dtValuation, string Member, double Units)
        {
            Console.WriteLine("uodating members capital account table...");

            using (var command = new SqlCommand("sp_UpdateMembersCapitalAccount", _connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@ValuationDate", dtValuation));
                command.Parameters.Add(new SqlParameter("@Member", Member));
                command.Parameters.Add(new SqlParameter("@Units", Units));
                command.ExecuteNonQuery();
            }
        }

        private void _UpdateValuationTable(DateTime dtValuationDate, double dUnitPrice)
        {
            Console.WriteLine("updating valuation table...");

            using (var command = new SqlCommand("sp_UpdateValuationTable", _connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@ValuationDate", dtValuationDate));
                command.Parameters.Add(new SqlParameter("@UnitPrice", dUnitPrice));
                command.ExecuteNonQuery();
            }
        }

        private void _UploadUnitPriceData()
        {
            Console.WriteLine("uploading unit price data");
            foreach(var book in _bookHolder.GetHistoricalAssetBooks())
            {
                foreach(_Worksheet sheet in book.Worksheets)
                {
                    double dUnitValue = 0d;
                    if (sheet.GetUnitValueFromAssetSheet(ref dUnitValue))
                    {
                        var dtDate = sheet.GetValueDateTime("C", 4);
                        if (dtDate.HasValue)
                        {
                            _UpdateValuationTable(dtDate.Value, dUnitValue);
                        }
                    }
                }
            }
        }

        private void _UploadMemberCapitalAccountData()
        {
            Console.WriteLine("uploading members capital account data");
            _Worksheet mcaSheet = _bookHolder.GetCashBook().Worksheets["Members Capital Account"];
            var iMonth = _dtValuationDate.Month;
            var iRefRow = 9 * (iMonth - 1) + 5;

            for (int i = 0; i < 5; i++)
            {
                var user = mcaSheet.get_Range("B" + iRefRow).Value;
                var units = 0d;
                mcaSheet.GetValueDouble("K", iRefRow, ref units);
                iRefRow++;
                _UpdateMembersCapitalAccountTable(_dtValuationDate, user, units);
            }
        }  

        //upload investment record data into database
        private void _UploadInvestmentRecordData()
        {
            Console.WriteLine("upload investment record data...");
            for (int index = 1; index <= _bookHolder.GetInvestmentRecordBook().Worksheets.Count; ++index)
            {
                _Worksheet recordSheet = _bookHolder.GetInvestmentRecordBook().Worksheets[index];
                string sCompanyName = recordSheet.Name;
                string sCompanyDescription = recordSheet.get_Range("B" + 3).Value;
                string sSymbol = recordSheet.get_Range("B" + 4).Value;
                string sCurrency = recordSheet.get_Range("B" + 5).Value;
                double dScalingFactor = 0;
                double dDividend = 0d;
                recordSheet.GetValueDouble("C", 4, ref dScalingFactor);
                bool bActive = recordSheet.get_Range("B" + 7).Value;
                if(bActive == false)
                {
                    continue;
                }

                int lastRow = recordSheet.GetLastPopulatedRow("A", 10);
                var dtValuation = recordSheet.GetValueDateTime("A", lastRow);
                if (dtValuation.HasValue == false)
                {
                    continue;
                }
                double dSharesBought, dBonusShares, dSharesSold;
                double dSharePrice, dTotalCost;
                dSharesBought = dBonusShares = dSharesSold = 0;
                dTotalCost = dSharePrice = 0;
                recordSheet.GetValueDouble("B", lastRow, ref dSharesBought);
                recordSheet.GetValueDouble("C", lastRow, ref dBonusShares);
                recordSheet.GetValueDouble("E", lastRow, ref dSharesSold);
                recordSheet.GetValueDouble("G", lastRow, ref dTotalCost);
                recordSheet.GetValueDouble("J", lastRow, ref dSharePrice);
                recordSheet.GetValueDouble("P", lastRow, ref dDividend);

                int totalShares = (int)(dSharesBought + dBonusShares - dSharesSold);
               _CreateNewInvestment(dtValuation.Value, sCompanyName, sSymbol, sCurrency, (int)dScalingFactor, totalShares, dTotalCost, dSharePrice, dDividend );
            }
        }

        private void _CreateNewInvestment(DateTime dtValuation, string sName, string sSymbol, string sCurrency, int iScalingFactor,
                                          int dShares, double dTotalCost, double dClosing, double dDividend)
        {
            Console.WriteLine("creating new investmnet: {0}", sName);
            using (var command = new SqlCommand("sp_CreateNewInvestment", _connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@valuationDate", dtValuation));
                command.Parameters.Add(new SqlParameter("@investment", sName));
                command.Parameters.Add(new SqlParameter("@symbol", sSymbol));
                command.Parameters.Add(new SqlParameter("@currency", sCurrency));
                command.Parameters.Add(new SqlParameter("@scalingFactor", iScalingFactor));
                command.Parameters.Add(new SqlParameter("@shares", dShares));
                command.Parameters.Add(new SqlParameter("@totalCost", dTotalCost));
                command.Parameters.Add(new SqlParameter("@closingPrice", dClosing));
                command.Parameters.Add(new SqlParameter("@dividend", dDividend));

                command.ExecuteNonQuery();
            }
        }
    }
}
