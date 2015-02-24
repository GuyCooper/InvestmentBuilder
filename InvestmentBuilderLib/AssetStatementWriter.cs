using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using ExcelAccountsManager;
using System.Data.SqlClient;
using NLog;

namespace InvestmentBuilder
{
    interface IAssetStatementWriter
    {
        void WriteAssetStatement(IEnumerable<CompanyData> companyData, CashAccountData cashData, DateTime? dtPreviousValution, DateTime valuationDate);
    }

    internal abstract class AssetStatementWriter : IAssetStatementWriter
    {
        protected Logger Log { get; private set; }

        protected List<string> Months = new List<string>
        {
            "January",
            "February",
            "March",
            "April",
            "May",
            "June",
            "July",
            "August",
            "September",
            "October",
            "Novemeber",
            "December"
        };

        //private Application _App;
        protected ExcelBookHolder _bookHolder;

        public AssetStatementWriter(ExcelBookHolder bookHolder)
        {
            //_App = app;
            _bookHolder = bookHolder;
            Log = LogManager.GetLogger(GetType().FullName);
        }

        protected abstract double UpdateMembersCapitalAccount(double dPreviousUnitValue, DateTime? dtPreviousValution, DateTime valuationDate);

        //override save unit value for databasse inplementation. otherwise do nothing
        protected abstract void SaveNewUnitValue(DateTime valuationDate);

        protected abstract double GetPreviousUnitValuation(DateTime valuationDate, DateTime? previousDate);

        protected bool _TryGetUnitValue(string month, ref double unitValue)
        {
            //_assetBook.Worksheets.
            _Worksheet assetSheet = _bookHolder.GetAssetSheetBook().FindWorksheet(month);
            if (assetSheet != null)
            {
                return assetSheet.GetUnitValueFromAssetSheet(ref unitValue);
            }
            return false;
        }

        public void WriteAssetStatement(IEnumerable<CompanyData> companyData, CashAccountData cashData, DateTime? dtPreviousValution, DateTime valuationDate)
        {
            Log.Log(LogLevel.Info, "writing asset statement sheet...");
            //Console.WriteLine("writing asset statement sheet...");
            //var spreadsheetLocation = @"C:\Users\Guy\Documents\Guy\Investments\Investment Club\accounts\Monthly Assets Statement-Test.xls";
            //var templateLocation = @"C:\Users\Guy\Documents\Guy\Investments\Investment Club\accounts\Template.xls";
           
            //first create a copy of the template sheet and add it at the beginning of the MAS sheet
            //var exWbk = exApp.Workbooks.Open(spreadsheetLocation);

            //get the previous months unit value amount to calculate the total units allocated
            double previousUnitValue = GetPreviousUnitValuation(valuationDate, dtPreviousValution);
            
            //add in a new monthly asset sheet from the template workbook
            _Worksheet templateSheet = _bookHolder.GetTemplateBook().Worksheets["Assets"];
            templateSheet.Copy(_bookHolder.GetAssetSheetBook().Worksheets[1]);

            _Worksheet newSheet = _bookHolder.GetAssetSheetBook().Worksheets[1];
            newSheet.EnableCalculation = true;
            newSheet.Name = Months[valuationDate.Month - 1];

            //add valuation date
            newSheet.get_Range("C4").Value = valuationDate;

            //add in the new rows
            var lstCompanyData = companyData.ToList();
            for (int row = 1; row < lstCompanyData.Count; ++row)
            {
                Range rowToCopy = newSheet.get_Range("A7", "A7").EntireRow;
                rowToCopy.Copy(); //put row onto clipboard
                rowToCopy.Insert(XlInsertShiftDirection.xlShiftDown);
                newSheet.get_Range("A7", "A7").EntireRow.PasteSpecial(); //paste format into new row
            }

            int count = 7;
            //now add the company data
            foreach (var company in companyData)
            {
                Log.Log(LogLevel.Info, string.Format("Adding {0} to asset sheet", company.sName));
                //Console.WriteLine("Adding {0} to asset sheet", company.sName);

                newSheet.get_Range("B" + count).Value = company.sName;
                newSheet.get_Range("C" + count).Value = company.dtLastBrought.Value;
                newSheet.get_Range("D" + count).Value = company.iNumberOfShares;
                newSheet.get_Range("E" + count).Value = company.dAveragePricePaid;
                newSheet.get_Range("F" + count).Value = company.dTotalCost;
                newSheet.get_Range("G" + count).Value = company.dSharePrice;
                newSheet.get_Range("H" + count).Value = company.dNetSellingValue;
                newSheet.get_Range("J" + count).Value = company.dMonthChange;
                newSheet.get_Range("K" + count).Value = company.dMonthChangeRatio;
                newSheet.get_Range("L" + count).Value = company.dDividend;
                count++;
            }

            //add the sum for the net sell value and change during month
            newSheet.get_Range("H" + count).Formula = string.Format("=SUM(H7:H{0})",count - 1);
            newSheet.get_Range("J" + count).Formula = string.Format("=SUM(J7:J{0})", count - 1);

            //calculate the allocated units for the current month and the bank balance and update the monthly assets sheet
            var dAllocatedUnits = UpdateMembersCapitalAccount(previousUnitValue, dtPreviousValution, valuationDate);
            //var dBankBalance = _GetBankBalance();

            int unitsRow = 0;
            if (newSheet.TryGetRowReference("I", "ISSUED UNITS", ref unitsRow))
            {
                newSheet.get_Range("K" + unitsRow).Value = dAllocatedUnits;
            }

            int balanceRow = 0;
            if (newSheet.TryGetRowReference("F", "Bank Balance", ref balanceRow))
            {
                newSheet.get_Range("H" + balanceRow).Value = cashData.BankBalance;
            }

            //now save the new unit value
            SaveNewUnitValue(valuationDate);
        }
    }

    class AssetStatementWriterExcel : AssetStatementWriter
    {
        public AssetStatementWriterExcel(ExcelBookHolder bookHolder) : base(bookHolder)
        {

        }

        protected override double UpdateMembersCapitalAccount(double dPreviousUnitValue, DateTime? dtPreviousValution, DateTime valuationDate)
        {
            Log.Log(LogLevel.Info, "updating members capital account...");
            //Console.WriteLine("updating members capital account...");
            //using previous unit value update member capital account, this will return the total number of units allocated
            //var cashBook = _App.Workbooks.Open(cashAccountLocation);
            _Worksheet mcaSheet = _bookHolder.GetCashBook().Worksheets["Members Capital Account"];

            mcaSheet.EnableCalculation = true;
            ////use the valuation date month to determine which cells we need to update with the previous unit value
            var iMonth = valuationDate.Month;
            var iRefRow = 9 * (iMonth - 1) + 5;

            for (int i = 0; i < 5; i++)
            {
                mcaSheet.get_Range("G" + iRefRow++).Value = dPreviousUnitValue;
            }

            var oRes = mcaSheet.get_Range("K" + iRefRow).Value;
            if (oRes != null)
            {
                return (double)oRes;
            }

            throw new ApplicationException("unable to calculate new allocated units");
            //return 0d;
            //return (double)mcaSheet.get_Range("K" + iRefRow + 5).Value;
        }

        private string _GetPreviousMonth(DateTime valuationDate)
        {
            var previousMonth = valuationDate.Month - 1;
            if (previousMonth < 1)
                previousMonth = 12;
            return Months[previousMonth - 1];
        }

        protected override void SaveNewUnitValue(DateTime valuationDate)
        {
        }

        protected override double GetPreviousUnitValuation(DateTime valuationDate, DateTime? previousDate)
        {
            double previousUnitValue = 0d;
            if (_TryGetUnitValue(_GetPreviousMonth(valuationDate), ref previousUnitValue) == false)
            {
                throw new ApplicationException("unable to retrieve previous unit value");
            }
            return previousUnitValue;
        }
    }

    class AssetStatementWriterDatabase : AssetStatementWriter
    {
        private SqlConnection _connection;
        public AssetStatementWriterDatabase(SqlConnection connection, ExcelBookHolder bookHolder) :
            base(bookHolder) 
        {
            _connection = connection;
        }

        private double _GetMemberSubscription(DateTime dtValuation, string member)
        {
            double dSubscription = 0d;
            using (var command = new SqlCommand("sp_GetMemberSubscriptionAmount", _connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@Member", member));
                command.Parameters.Add(new SqlParameter("@ValuationDate", dtValuation));
                dSubscription = (double)command.ExecuteScalar();
            }
            return dSubscription;
        }

        private IEnumerable<KeyValuePair<string, double>> _GetMemberAccountData(DateTime dtValuation)
        {
            using (var command = new SqlCommand("sp_GetMembersCapitalAccount", _connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@ValuationDate", dtValuation));

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var member = (string)reader["Member"];
                        var units = (double)reader["Units"];
                        yield return new KeyValuePair<string, double>(member, units);
                    }
                    reader.Close();
                }
            }
        }

        protected override double UpdateMembersCapitalAccount(double dPreviousUnitValue, DateTime? dtPreviousValution, DateTime valuationDate)
        {
            //get total number of shares allocated for previous month
            //get list of all members who have made a deposit for current month
            double dResult = 0d;
            var memberAccountData = _GetMemberAccountData(dtPreviousValution ?? valuationDate).ToList();
            foreach(var member in memberAccountData)
            {
                double dSubscription = _GetMemberSubscription(valuationDate, member.Key);
                double dNewAmount = member.Value + (dSubscription  * (1 / dPreviousUnitValue));
                dResult += dNewAmount;
                using (var updateCommand = new SqlCommand("sp_UpdateMembersCapitalAccount", _connection))
                {
                    updateCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    updateCommand.Parameters.Add(new SqlParameter("@ValuationDate", valuationDate));
                    updateCommand.Parameters.Add(new SqlParameter("@Member", member.Key));
                    updateCommand.Parameters.Add(new SqlParameter("@Units", dNewAmount));
                    updateCommand.ExecuteNonQuery();
                }
            }
            return dResult;
        }

        protected override double GetPreviousUnitValuation(DateTime valuationDate, DateTime? previousDate)
        {
            using (var command = new SqlCommand("sp_GetUnitValuation", _connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@valuationDate", previousDate.Value));
                return (double)command.ExecuteScalar();
            }
        }

        protected override void SaveNewUnitValue(DateTime valuationDate)
        {
            //calculate , then extract the new unit value
            double newUnitValue = 0d;
            if (_TryGetUnitValue(Months[valuationDate.Month - 1], ref newUnitValue) == false)
            {
                throw new ApplicationException("unable to retrieve new unit value");
            }

            //TODO - save new unit value
            using (var command = new SqlCommand("sp_AddNewUnitValuation", _connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@valuationDate", valuationDate));
                command.Parameters.Add(new SqlParameter("@unitValue", newUnitValue));

                command.ExecuteNonQuery();
            }
        }
    }
}
