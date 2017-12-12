using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
//using System.Net;
using System.IO;
using NLog;
using InvestmentBuilderCore;

namespace MarketDataServices
{
    /// <summary>
    /// class gets market data from yahoo
    /// </summary>
    [Export(typeof(IMarketDataSource))]
    internal class YahooMarketDataSource : TestFileMarketDataSource
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public YahooMarketDataSource() 
        {
        }

        public override string Name { get { return "Yahoo"; } }

        public override int Priority { get { return 1; } }

        public override Task<MarketDataPrice> RequestPrice(string symbol, string exchange, string source)
        {
            return Task.Factory.StartNew(() =>
            {
                MarketDataPrice price = null;
                //first check if price is already stored. no point in requesting again
                if(TryGetMarketData(symbol, exchange, source, out price) == true)
                {
                    //it is, just return stored price
                    return price;
                }

                //run external php script to download price
                var outputFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "InvestmentRecordBuilder", "result.txt");
                File.Delete(outputFile);
                var process = new System.Diagnostics.Process();
                process.StartInfo.FileName = "php.exe";
                process.StartInfo.Arguments = string.Format(@"MarketDataLoader.php --n:{0} --o:{1}", symbol, outputFile);
                process.StartInfo.CreateNoWindow = false;
                process.StartInfo.ErrorDialog = true;
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.WorkingDirectory = @"C:\Projects\InvestmentBuilder\php";
                process.Start();
                process.WaitForExit();

                //add result to cache
                ProcessFileName(outputFile);
                //now retrieve the newly found price and return it
                TryGetMarketData(symbol, exchange, source, out price);
                return price;
            });
        }
    }
}