using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using ExcelAccountsManager;
using System.Data.SqlClient;

namespace InvestmentBuilder
{
    interface IAssetStatementWriter
    {
        void WriteAssetStatement(IEnumerable<CompanyData> companyData, CashAccountData cashData, DateTime? dtPreviousValution, DateTime valuationDate);
    }

    internal abstract class AssetStatementWriter : IAssetStatementWriter
    {
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
        }

        protected abstract double UpdateMembersCapitalAccount(double dPreviousUnitValue, DateTime? dtPreviousValution, DateTime valuationDate);

        //override save unit value for databasse inplementation. otherwise do nothing
        protected virtual void SaveNewUnitValue(DateTime valuationDate) { }

        private _Worksheet _FindWorksheet(_Workbook book, string name)
        {
            foreach(_Worksheet sheet in book.Worksheets)
            {
                if(sheet.Name.Equals(name,StringComparison.CurrentCultureIgnoreCase))
                {
                    return sheet;
                }
            }
            return null;
        }

        private string _GetPreviousMonth(DateTime valuationDate)
        {
            var previousMonth = valuationDate.Month - 1;
            if (previousMonth < 1)
                previousMonth = 12;
            return Months[previousMonth - 1];
        }

        protected bool _TryGetUnitValue(string month, ref double unitValue)
        {
            //_assetBook.Worksheets.
            _Worksheet assetSheet = _FindWorksheet(_bookHolder.GetAssetSheetBook(), month);
            if (assetSheet != null)
            {
                int row = 0; ;
                if (assetSheet.TryGetRowReference("I", "VALUE PER UNIT", ref row))
                {
                    unitValue = (double)assetSheet.get_Range("K" + row).Value;
                    return true;
                }
            }
            return false;
        }

        public void WriteAssetStatement(IEnumerable<CompanyData> companyData, CashAccountData cashData, DateTime? dtPreviousValution, DateTime valuationDate)
        {
            Console.WriteLine("writing asset statement sheet...");
            //var spreadsheetLocation = @"C:\Users\Guy\Documents\Guy\Investments\Investment Club\accounts\Monthly Assets Statement-Test.xls";
            //var templateLocation = @"C:\Users\Guy\Documents\Guy\Investments\Investment Club\accounts\Template.xls";
           
            //first create a copy of the template sheet and add it at the beginning of the MAS sheet
            //var exWbk = exApp.Workbooks.Open(spreadsheetLocation);

            //get the previous months unit value amount to calculate the total units allocated
            double previousUnitValue = 0d;
            if (_TryGetUnitValue(_GetPreviousMonth(valuationDate), ref previousUnitValue) == false)
            {
                throw new ApplicationException("unable to retrieve previous unit value");
            }

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
                Console.WriteLine("Adding {0} to asset sheet", company.sName);

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
            var dAllocatedUnits = UpdateMembersCapitalAccount(previousUnitValue * 100d, dtPreviousValution, valuationDate);
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
            Console.WriteLine("updating members capital account...");
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
    }

    class AssetStatementWriterDatabase : AssetStatementWriter
    {
        private SqlConnection _connection;
        public AssetStatementWriterDatabase(SqlConnection connection, ExcelBookHolder bookHolder) :
            base(bookHolder) 
        {
            _connection = connection;
        }

        protected override double UpdateMembersCapitalAccount(double dPreviousUnitValue, DateTime? dtPreviousValution, DateTime valuationDate)
        {
            //get total number of shares allocated for previous month
            //get list of all members who have made a deposit for current month
            //calculate new unit count for each member = previous month units + deposit * 1/previousunitvalue
            //return new total amount
            double dResult = 0d;
            using (var command = new SqlCommand("sp_UpdateMembersCapitalAccount", _connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@valuationDate", valuationDate));
                command.Parameters.Add(new SqlParameter("@previousValuation", dtPreviousValution ?? valuationDate));
                command.Parameters.Add(new SqlParameter("@previousUnitValue", dPreviousUnitValue));

                dResult = (double)command.ExecuteScalar();
            }
            return dResult;
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
