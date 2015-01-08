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
    class DataLoader
    {
        private Application _app = new Microsoft.Office.Interop.Excel.Application();
        SqlConnection _connection;
        ExcelBookHolder _bookHolder;
        DateTime _dtValuationDate;

        public DataLoader(string path, string dbconn, DateTime dtValuationDate)
        {
            //first load the cash account and investment record books for the current year
            string InvestmentRecordFile = string.Format(@"{0}\{1}-{2}", path, ExcelBookHolder.InvestmentRecordName, DateTime.Today.Year);
            string CashAccountFile = string.Format(@"{0}\{1}-{2}", path, ExcelBookHolder.CashAccountName, DateTime.Today.Year);
            _bookHolder = new ExcelBookHolder(_app, InvestmentRecordFile, null, null, CashAccountFile, path);
            _connection = new SqlConnection(dbconn);
            _dtValuationDate = dtValuationDate;
        }

        public void LoadData(string path)
        {
           //now upload the investment record data
            _UploadInvestmentRecordData();
            _UploadMemberCapitalAccountData();
            _UploadUnitPriceData();
        }

        private void _UpdateMembersCapitalAccountTable(string User, double Units)
        {
            var strSQL = "INSERT INTO dbo.[MembersCapitalAccount] (Valuation_Date, Member, Units) ";
            strSQL += string.Format("VALUES ({0}, {1}, {2})", _dtValuationDate, User, Units);

            using (var command = new SqlCommand(strSQL, _connection))
            {
                command.CommandType = System.Data.CommandType.Text;
                command.ExecuteNonQuery();
            }
        }

        private void _UpdateValuationTable(DateTime dtValuationDate, double dUnitPrice)
        {
            var strSQL = string.Format("INSERT INTO dbo.[Valuations] (Valuation_Date, Unit_Price) VALUES ({0}, {1})",
                dtValuationDate, dUnitPrice);

            using (var command = new SqlCommand(strSQL, _connection))
            {
                command.CommandType = System.Data.CommandType.Text;
                command.ExecuteNonQuery();
            }
        }

        private void _UploadUnitPriceData()
        {
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
            _Worksheet mcaSheet = _bookHolder.GetCashBook().Worksheets["Members Capital Account"];
            var iMonth = _dtValuationDate.Month;
            var iRefRow = 9 * (iMonth - 1) + 5;

            for (int i = 0; i < 5; i++)
            {
                var user = mcaSheet.get_Range("B" + iRefRow++).Value;
                var units = 0d;
                mcaSheet.GetValueDouble("K", iRefRow, ref units);
                _UpdateMembersCapitalAccountTable(user, units);
            }

        }

        //upload investment record data into database
        private void _UploadInvestmentRecordData()
        {
            for (int index = 1; index <= _bookHolder.GetInvestmentRecordBook().Worksheets.Count; ++index)
            {
                _Worksheet recordSheet = _bookHolder.GetInvestmentRecordBook().Worksheets[index];
                string sCompanyName = recordSheet.Name;
                string sCompanyDescription = recordSheet.get_Range("B" + 3).Value;
                string sSymbol = recordSheet.get_Range("B" + 4).Value;
                string sCurrency = recordSheet.get_Range("B" + 5).Value;
                int scalingFactor = 0;
                recordSheet.GetValueInt("C", 4, ref scalingFactor);
                bool bIsActive = recordSheet.get_Range("B" + 7).Value == "TRUE";

                int lastRow = recordSheet.GetLastPopulatedRow("A", 10);
                var dtValuation = recordSheet.GetValueDateTime("A", lastRow);
                if (dtValuation.HasValue == false)
                {
                    continue;
                }
                int iSharesBought, iBonusShares, iSharesSold;
                double dSharePrice, dTotalCost, dSellingPrice;
                iSharesBought = iBonusShares = iSharesSold = 0;
                dTotalCost = dSharePrice = dSellingPrice = 0;
                recordSheet.GetValueInt("B", lastRow, ref iSharesBought);
                recordSheet.GetValueInt("C", lastRow, ref iBonusShares);
                recordSheet.GetValueInt("D", lastRow, ref iSharesSold);
                recordSheet.GetValueDouble("E", lastRow, ref dTotalCost);
                recordSheet.GetValueDouble("F", lastRow, ref dSharePrice);
                recordSheet.GetValueDouble("G", lastRow, ref dSellingPrice);
                int totalShares = iSharesBought + iBonusShares - iSharesSold;
               _CreateNewInvestment(dtValuation.Value, sCompanyName, sSymbol, sCurrency, scalingFactor, totalShares, dTotalCost, dSharePrice );
            }
        }

        private void _CreateNewInvestment(DateTime dtValuation, string sName, string sSymbol, string sCurrency, int iScalingFactor,
                                          int iShares, double dTotalCost, double dClosing)
        {
            using (var command = new SqlCommand("sp_CreateNewInvestment", _connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@valuationDate", dtValuation));
                command.Parameters.Add(new SqlParameter("@investment", sName));
                command.Parameters.Add(new SqlParameter("@symbol", sSymbol));
                command.Parameters.Add(new SqlParameter("@currency", sCurrency));
                command.Parameters.Add(new SqlParameter("@scalingFactor", iScalingFactor));
                command.Parameters.Add(new SqlParameter("@shares", iShares));
                command.Parameters.Add(new SqlParameter("@totalCost", dTotalCost));
                command.Parameters.Add(new SqlParameter("@closingPrice", dClosing));

                command.ExecuteNonQuery();
            }
        }
    }
}
