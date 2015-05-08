using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using InvestmentBuilderCore;

namespace MarketDataServices
{
    class QuandlMarketDataSource : IMarketDataSource
    {
        //help: https://www.quandl.com/help/api
        //https://www.quandl.com/api/v1/datasets/WIKI/AAPL.csv?

        //vodaphone url
        //
        private const string urlTemplate = @"https://www.quandl.com/api/v1/datasets/{0}/{1}.csv";

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public string Name { get { return "Quandl"; } }

        public bool TryGetMarketData(string symbol, string exchange, out double dData)
        {
            //still work to do this 
            if (string.IsNullOrEmpty(exchange))
            {
                exchange = "WIKI";
            }

            string url = string.Format(urlTemplate, exchange, symbol);
            try
            {
                var data = WebDataHandler.GetData(url);
                return _TryParseResult(data, out dData);
            }
            catch (Exception e)
            {
                logger.Log(LogLevel.Error, "unable to retrieve {0} from quandl: {1}", symbol, e.Message);
            }
            dData = 0d;
            return false;
            
        }

        public bool TryGetFxRate(string baseCurrency, string contraCurrency, out double dFxRate)
        {
            dFxRate = 0d;
            return false;
        }

        public IEnumerable<HistoricalData> GetHistoricalData(string instrument, DateTime dtFrom)
        {
            return null;
        }

        private bool _TryParseResult(IEnumerable<string> data, out double dResult)
        {
            dResult = 0d;
            return false;
        }
    }
}
