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
    class CashAccountData
    {
        public CashAccountData()
        {
            Dividends = new Dictionary<string, double>();
        }
        public Dictionary<string, double> Dividends { get; private set; }
        public double BankBalance { get; set; }
    }

    interface ICashAccountReader
    {
        CashAccountData GetCashAccountData(DateTime valuationDate);
    }

    //this class is used for extracting the cash account data. this includes any dividends payed for the current month and the
    //latest bank balance
    class CashAccountReaderExcel : ICashAccountReader
    {
        private ExcelBookHolder _bookHolder;
        public CashAccountReaderExcel(ExcelBookHolder bookHolder)
        {
            _bookHolder = bookHolder;
        }

        public CashAccountData GetCashAccountData(DateTime valuationDate)
        {
            _Worksheet cashSheet = _bookHolder.GetCashBook().Worksheets["Cash Account"];
            int month = valuationDate.Month;
            int monthsFound = 0;
            int previousMonthRow = 0;
            int maxRows = cashSheet.UsedRange.Rows.Count;
            for (int row = 1; row <= maxRows; row++)
            {
                var cell = cashSheet.get_Range("A" + row).Value as string;
                if (cell != null && cell.ToUpper() == "SIGN OFF")
                {
                    monthsFound++;
                    if (monthsFound == month)
                    {
                        //we are now at the correct row, extract the balance in hand
                        int balanceRow = row - 1;
                        var oRes = cashSheet.get_Range("M" + balanceRow).Value;
                        if (oRes != null)
                        {
                            //now extract the dividend data for the current month
                            //TODO
                            //return (double)oRes;
                            var cashData = new CashAccountData();
                            for(int dividendRow = previousMonthRow+1; dividendRow != row-1; ++dividendRow )
                            {
                                double dDividend = 0;
                                if(cashSheet.GetValueDouble("E", dividendRow, ref dDividend ))
                                {
                                    var company = cashSheet.get_Range("B" + dividendRow).Value as string;
                                    cashData.Dividends.Add(company, dDividend);
                                }
                            }
                            cashData.BankBalance = (double)oRes;
                            return cashData;
                        }
                    }
                    previousMonthRow = row;
                }
            }
            //if we get here then there is an error in the spreadsheet
            throw new ApplicationException("error finding bank balance for current month");
        }
    }

    class CashAccountReaderDatabase : ICashAccountReader
    {
        //private DateTime _dtPreviousDate;
        private SqlConnection _conn;

        public CashAccountReaderDatabase(SqlConnection conn)
        {
            //_dtPreviousDate = dtPreviousDate;
            _conn = conn;
        }

        public CashAccountData GetCashAccountData(DateTime valuationDate)
        {
            var cashData = new CashAccountData();
            
            //retrieve the current bank balance
            using (SqlCommand cmdBankBalance = new SqlCommand("sp_GetBankBalance", _conn))
            {
                cmdBankBalance.CommandType = System.Data.CommandType.StoredProcedure;
                cmdBankBalance.Parameters.Add(new SqlParameter("valuationDate", System.Data.SqlDbType.DateTime) { Value = valuationDate });

                //var balanceParam = new SqlParameter("@balance", System.Data.SqlDbType.Float);
                //balanceParam.Direction = System.Data.ParameterDirection.Output;
                //cmdBankBalance.Parameters.Add(balanceParam);
                //cmdBankBalance.ExecuteNonQuery();

                //cashData.BankBalance = balanceParam.Value is double ? (double)balanceParam.Value : 0d;
                cashData.BankBalance = (double)cmdBankBalance.ExecuteScalar();

                using (SqlCommand cmdDividends = new SqlCommand("sp_GetDividends", _conn))
                {
                    cmdDividends.CommandType = System.Data.CommandType.StoredProcedure;
                    cmdDividends.Parameters.Add(new SqlParameter("@valuationDate", System.Data.SqlDbType.DateTime) { Value = valuationDate });
                    using (SqlDataReader reader = cmdDividends.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cashData.Dividends.Add((string)reader["Company"], (double)reader["Dividend"]);
                        }
                    }
                }
            }
            return cashData;
        }
    }
}
