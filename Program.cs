using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestmentBuilder
{
    class Program
    {
        /// <summary>
        /// build asset sheet for current month
        /// </summary>
        /// <param name="bTest"></param>
        /// <param name="path"></param>
        static void AssetSheetBuilder(IInvestmentFactory factory, Trades trades, DateTime valuationDate)
        {
            try
            {
                var recordBuilder = factory.CreateInvestmentRecordBuilder();
                var dataReader = factory.CreateCompanyDataReader();
                var assetWriter = factory.CreateAssetStatementWriter();
                var cashAccountReader = factory.CreateCashAccountReader();

                //first extract the cash account data
                var cashAccountData = cashAccountReader.GetCashAccountData(valuationDate);
                //parse the trade file for any trades for this month and update the investment record
                //var trades = TradeLoader.GetTrades(tradeFile);
                var dtPreviousValuation = recordBuilder.BuildInvestmentRecords(trades, cashAccountData, valuationDate);

                //now extract the latest data from the investment record
                var lstData = dataReader.GetCompanyData(valuationDate, dtPreviousValuation).ToList();
                foreach (var val in lstData)
                {
                    Console.WriteLine("{0} : {1} : {2} : {3} : {4}", val.sName, val.dSharePrice, val.dNetSellingValue, val.dMonthChange, val.dMonthChangeRatio);
                }

                //finally, build the asset statement
                assetWriter.WriteAssetStatement(lstData, cashAccountData, valuationDate);

                factory.CommitData();
            }
            finally
            {
                //app.Workbooks.Close();
                factory.Close();
            }
        }

        //static void InvestmentRecordBuilder()
        //{
        //    //RecordBuilder builder = new RecordBuilder();
        //    //builder.DownloadData("SVT.L");
        //    double dClosing;
        //    string symbol = "SVT.L";
        //    if (MarketDataService.TryGetClosingPrice(symbol, "SEVERN TRENT", "LONDON", 100d,out dClosing))
        //    {
        //        Console.WriteLine("closing price for {0} : {1}", symbol, dClosing);
        //    }
        //}

        //static void LoadTrades()
        //{
        //    var trades = TradeLoader.GetTrades("D:\\Projects\\InvestmentBuilder\\Trades.xml");
        //    foreach(var trade in trades.Buys)
        //    {
        //        Console.WriteLine("name: {0}, number {1}, total cost {2}", trade.Name, trade.Number, trade.TotalCost);
        //    }
        //}
      
        static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                Console.WriteLine(@"syntax: InvestmentBuilder <-p:path> <-t> <-d:Valuation Date>. where -t = test -p = path and -d specifies valuation date");
                return;
            }

            string path = @"C:\Users\Guy\Documents\Guy\Investments\Investment Club\accounts\";
            bool bTest = false;
            DateTime? dtValuationDate = null;

            foreach(var arg in args)
            {
                if(arg[0] == '-')
                {
                    switch(arg[1])
                    {
                        case 'p':
                            if (arg[2] == ':')
                                path = arg.Substring(3);
                            break;
                        case 't':
                            bTest = true;
                            break;
                        case 'd':
                            if (arg[2] == ':')
                                dtValuationDate = DateTime.Parse(arg.Substring(3));
                            break;
                    }
                }
            }

            if (path.Last() != '\\')
            {
                path = path + "\\";
            }

            //create investment factory for either excel or database
            var investmentFactory = new ExcelInvestmentFactory(path, bTest);

            //load in any trades from the trades file
            string tradeFile = string.Format("{0}Trades.xml", path);
            var trades = TradeLoader.GetTrades(tradeFile);

            AssetSheetBuilder(investmentFactory, trades, dtValuationDate.HasValue ? dtValuationDate.Value : DateTime.Today);
        }
    }
}
