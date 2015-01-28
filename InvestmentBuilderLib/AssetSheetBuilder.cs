using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestmentBuilder
{
    public enum DataFormat
    {
        EXCEL,
        DATABASE
    }

    public static class AssetSheetBuilder
    {
        /// <summary>
        /// factory method to create the correct factory class
        /// </summary>
        /// <param name="format"></param>
        /// <param name="path"></param>
        /// <param name="connectionstr"></param>
        /// <param name="bTest"></param>
        /// <returns></returns>
        private static IInvestmentFactory BuildFactory(DataFormat format, string path, string connectionstr, DateTime dtValuation, bool bTest)
        {
            switch(format)
            {
                case DataFormat.EXCEL:
                    return new ExcelInvestmentFactory(path, dtValuation, bTest);
                case DataFormat.DATABASE:
                    return new DatabaseInvestmentFactory(path, connectionstr, dtValuation, bTest);
            }

            throw new ArgumentException("invalid dataformat");
             
        }
          /// <summary>
        /// build asset sheet for current month
        /// </summary>
        /// <param name="bTest"></param>
        /// <param name="path"></param>
        public static void BuildAssetSheet(string tradeFile, string path, string connectionstr, bool bTest, DateTime valuationDate, DataFormat format)
        {
            var factory = BuildFactory(format, path, connectionstr, valuationDate, bTest);
            try
            {
                var trades = TradeLoader.GetTrades(tradeFile);

                var recordBuilder = factory.CreateInvestmentRecordBuilder();
                var dataReader = factory.CreateCompanyDataReader();
                var assetWriter = factory.CreateAssetStatementWriter();
                var cashAccountReader = factory.CreateCashAccountReader();

                var dtPreviousValuation = cashAccountReader.GetPreviousValuationDate();
                //first extract the cash account data
                var cashAccountData = cashAccountReader.GetCashAccountData(valuationDate);
                //parse the trade file for any trades for this month and update the investment record
                //var trades = TradeLoader.GetTrades(tradeFile);
                //recordBuilder.BuildInvestmentRecords(trades, cashAccountData, valuationDate, dtPreviousValuation);

                //now extract the latest data from the investment record
                var lstData = dataReader.GetCompanyData(valuationDate, dtPreviousValuation).ToList();
                foreach (var val in lstData)
                {
                    Console.WriteLine("{0} : {1} : {2} : {3} : {4}", val.sName, val.dSharePrice, val.dNetSellingValue, val.dMonthChange, val.dMonthChangeRatio);
                }

                //finally, build the asset statement
                assetWriter.WriteAssetStatement(lstData, cashAccountData, dtPreviousValuation, valuationDate);

                factory.CommitData();
            }
            finally
            {
                //app.Workbooks.Close();
                factory.Close();
            }
        }
    }
}
