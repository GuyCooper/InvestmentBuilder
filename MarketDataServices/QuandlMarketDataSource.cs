using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using InvestmentBuilderCore;

namespace MarketDataServices
{
    internal class QuandlMarketDataSource : IMarketDataSource
    {
        //help: https://www.quandl.com/help/api
        //https://www.quandl.com/api/v1/datasets/WIKI/AAPL.csv?

        //vodaphone url
        //
        private const string urlTemplate = @"https://www.quandl.com/api/v1/datasets/{0}/{1}.csv";

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public string Name { get { return "Quandl"; } }

        public IList<string> GetSources()
        {
            return new List<string> { Name };
        }

        public bool TryGetMarketData(string symbol, string exchange, string source, out MarketDataPrice marketData)
        {
            //still work to do this 
            if (string.IsNullOrEmpty(exchange))
            {
                exchange = "WIKI";
            }

            string url = string.Format(urlTemplate, exchange, symbol);
            //try
            //{
            //    var data = .GetData(url, SourceDataFormat.CSV);
            //    return _TryParseResult(data, out marketData);
            //}
            //catch (Exception e)
            //{
            //    logger.Log(LogLevel.Error, "unable to retrieve {0} from quandl: {1}", symbol, e.Message);
            //}
            marketData = null;
            return false;
            
        }

        public bool TryGetFxRate(string baseCurrency, string contraCurrency, string exchange, string source, out double dFxRate)
        {
            dFxRate = 0d;
            return false;
        }

        public IEnumerable<HistoricalData> GetHistoricalData(string instrument, string exchange, string source, DateTime dtFrom)
        {
            return null;
        }

        private bool _TryParseResult(IEnumerable<string> data, out MarketDataPrice marketData)
        {
            marketData = null;
            return false;
        }
        public int Priority { get { return 10; } }

        public void Initialise(IConfigurationSettings settings) { }
        //public IMarketDataReader DataReader { get; set; }
    }
}
