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

    class CompanyData
    {
        public string sName;
        public DateTime? dtLastBrought;
        public double iNumberOfShares;
        public double dAveragePricePaid;
        public double dTotalCost;
        public double dSharePrice;
        public double dNetSellingValue;
        public double dMonthChange;
        public double dMonthChangeRatio;
        public double dDividend;
    }

    interface ICompanyDataReader
    {
        IEnumerable<CompanyData> GetCompanyData(DateTime dtValuationDate, DateTime? dtPreviousValuationDate);
    }

    /// <summary>
    /// class returns all the investment record data from the investment record
    /// </summary>
    class CompanyDataReaderExcel : ICompanyDataReader
    {
        private ExcelBookHolder _bookHolder;
        public CompanyDataReaderExcel(ExcelBookHolder bookHolder)
        {
            _bookHolder = bookHolder;
        }

        public IEnumerable<CompanyData> GetCompanyData(DateTime dtValuationDate, DateTime? dtPreviousValuationDate)
        {
            //var spreadsheetLocation = Path.Combine(Directory.GetCurrentDirectory(), @"C:\Users\Guy\Documents\Guy\Investments\Investment Club\accounts\Investment Record-2014.xls");
            //var spreadsheetLocation = @"C:\Users\Guy\Documents\Guy\Investments\Investment Club\accounts\Investment Record-2014.xls";

            Console.WriteLine("getting company data...");
            for (int index = 1; index <= _bookHolder.GetInvestmentRecordBook().Worksheets.Count; ++index)
            {
                _Worksheet sheet = _bookHolder.GetInvestmentRecordBook().Worksheets[index];
                var title = sheet.get_Range("A3").Value as string;                
                if (title != null && title.ToUpper() == "NAME OF COMPANY")
                {
                    var bValid = (bool)sheet.get_Range("B7").Value;
                    if(!bValid)
                    {
                        continue;
                    }

                    int count = 10;
                    bool bValidRow = true;
                    CompanyData companyData = new CompanyData();
                    companyData.sName = sheet.Name; //sheet.get_Range("B3").Value as string;
                    while (bValidRow)
                    {
                        bValidRow = sheet.IsCellPopulated("J", count);
                        if (bValidRow)
                        {
                            if (companyData.dtLastBrought.HasValue == false)
                            {
                                companyData.dtLastBrought = sheet.GetValueDateTime("A", count );
                            }
                            sheet.GetValueDouble("F", count, ref companyData.iNumberOfShares);
                            sheet.GetValueDouble("H", count, ref companyData.dAveragePricePaid);
                            sheet.GetValueDouble("G", count, ref companyData.dTotalCost);
                            sheet.GetValueDouble("J", count, ref companyData.dSharePrice);
                            sheet.GetValueDouble("M", count, ref companyData.dNetSellingValue);
                            sheet.GetValueDouble("N", count, ref companyData.dMonthChange);
                            sheet.GetValueDouble("O", count, ref companyData.dMonthChangeRatio);
                            sheet.GetValueDouble("P", count, ref companyData.dDividend);
                        }
                        count++;
                    }
                    yield return companyData;
                }
            }
        }
    }

    class CompanyDataReaderDatabase : ICompanyDataReader
    {
        private SqlConnection _connection;
        public CompanyDataReaderDatabase(SqlConnection connection)
        {
            _connection = connection;
        }

        private double _GetNetSellingValue(double dSharesHeld, double dPrice)
        {
            double dGrossValue = dSharesHeld * dPrice;
            if (dGrossValue > 750d)
                return dGrossValue - (dGrossValue * 0.01);
            return dGrossValue - 7.5d;
        }

        private IEnumerable<CompanyData> _GetCompanyDataImpl(DateTime dtValuationDate)
        {
            using (var command = new SqlCommand("sp_GetLatestInvestmentRecords", _connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    double dTotalCost = (double)reader["TotalCost"];
                    double dSharesHeld = (int)reader["Bought"] + (int)reader["Bonus"] - (int)reader["Sold"];
                    double dAveragePrice = dTotalCost / dSharesHeld;
                    double dSharePrice = (double)reader["Price"];

                    yield return new CompanyData
                    {
                        sName = (string)reader["Name"],
                        dtLastBrought = (DateTime)reader["ValuationDate"],
                        iNumberOfShares = dSharesHeld,
                        dAveragePricePaid = dAveragePrice,
                        dTotalCost = dTotalCost,
                        dSharePrice = dSharePrice,
                        dNetSellingValue = _GetNetSellingValue(dSharesHeld, dSharePrice)
                    };
                }
                reader.Close();
            }
        }

        private void _updateMonthlyData(CompanyData currentData, CompanyData previousData)
        {
            currentData.dMonthChange = currentData.dNetSellingValue - previousData.dNetSellingValue;
            currentData.dMonthChangeRatio = currentData.dMonthChange / previousData.dNetSellingValue * 100;
        }

        public IEnumerable<CompanyData> GetCompanyData(DateTime dtValuationDate, DateTime? dtPreviousValuationDate)
        {
            var lstPreviousData = _GetCompanyDataImpl(dtPreviousValuationDate.Value).ToList();
            var lstCurrentData = _GetCompanyDataImpl(dtValuationDate).ToList();

            foreach(var company in lstCurrentData)
            {
                var previousData = lstPreviousData.Find(c => c.sName == company.sName);
                _updateMonthlyData(company, previousData);
            }

            return lstCurrentData;
        }
    }
}
