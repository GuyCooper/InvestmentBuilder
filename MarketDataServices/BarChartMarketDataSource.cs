using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderCore;
using NLog;
using Newtonsoft.Json;

namespace MarketDataServices
{
    internal class BarchartStatus
    {
        public int code { get; set; }
        public string message { get; set; }
    }

    internal class BarchartResult
    {
		public string symbol { get; set; }
		public DateTime timestamp { get; set; }
		public DateTime tradingDay { get; set; }
		public double open { get; set; }
		public double high { get; set; }
		public double low { get; set; }
		public double close { get; set; }
		public long volume { get; set; }
    }

    internal class BarchartHistoricalData
    {
        public BarchartStatus status { get; set; }
        public IList<BarchartResult> results { get; set; }
    }

    class BarChartMarketDataSource :  IMarketDataSource
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static readonly string AccessKey = "791ff2b78eefaceb1acb721c900f2f64";
        private static readonly string GetQuoteUrl = "http://marketdata.websol.barchart.com/getQuote.json?key={0})&symbol={1}";
        private static readonly string GetHistoryUrl = "http://marketdata.websol.barchart.com/getHistory.json?key={0}&symbol={1}&type=monthly&startDate={2}";

        //examples
        //GET http://ondemand.websol.barchart.com/getQuote.xml?symbols=AAPL%2CGOOG&fields=fiftyTwoWkHigh%2CfiftyTwoWkHighDate%2CfiftyTwoWkLow%2CfiftyTwoWkLowDate&mode=I 
        //GET http://ondemand.websol.barchart.com/getHistory.xml?symbol=AAPL&type=minutes&startDate=20100101&endDate=20130101&maxRecords=10&interval=60&order=asc&sessionFilter=EFK&splits=true÷nds=true&volume=sum&nearby=1&exchange=NYSE%2CAMEX%2CNASDAQ 

        //responses. GetQuote()
        //{"status":{"code":200,"message":"Success."},"results":[{"symbol":"FXI","exchange":"AMEX","name":"FTSE China 25 Index Fund Ishares","dayCode":"B","serverTimestamp":"2016-08-12T15:44:45-05:00","mode":"d","lastPrice":37.12,"tradeTimestamp":"2016-08-12T16:15:00-05:00","netChange":0.08,"percentChange":0.22,"unitCode":"2","open":37.09,"high":37.21,"low":37.04,"close":37.12,"flag":"s","volume":15657138}]}
        public string Name
        {
            get { return "BarchartMarketDatasource"; }
        }

        public int Priority
        {
            get { return 5; }
        }

        public IList<string> GetSources()
        {
            return new List<string> { Name };
        }

        public IEnumerable<HistoricalData> GetHistoricalData(string instrument, string exchange, string source, DateTime dtFrom)
        {
            var url = string.Format(GetHistoryUrl, AccessKey, instrument, dtFrom.ToString("yyyyMMdd"));
            var data = WebDataHandler.GetData(url, SourceDataFormat.JSON);
            //TODO convert into historical data (use JSON.net)
            if (data != null)
            {
                var result = JsonConvert.DeserializeObject<BarchartHistoricalData>(data.First());
                return result.results.Select(x => new HistoricalData
                {
                     Date = x.timestamp,
                     Price = x.close
                });
            }
            return null;
        }

        public bool TryGetFxRate(string baseCurrency, string contraCurrency, string source, out double dFxRate)
        {
            string quoteSymbol = "^" + baseCurrency + contraCurrency;
            try
            {
                var url = string.Format(GetQuoteUrl, AccessKey, quoteSymbol);
                var data = WebDataHandler.GetData(url, SourceDataFormat.JSON);
                //TODO, convert into market rate
                dFxRate = double.Parse(data.First());
                return true;
            }
            catch(Exception e)
            {
                logger.Log(LogLevel.Error, "unable to retrieve {0} from Barchart data source. error {1}", quoteSymbol, e.Message);
            }

            dFxRate = 0d;
            return false;
        }

        public bool TryGetMarketData(string symbol, string exchange, string source, out MarketDataPrice marketData)
        {
            string quoteSymbol = symbol + exchange;
            try
            {
                var url = string.Format(GetQuoteUrl, AccessKey, quoteSymbol);
                var data = WebDataHandler.GetData(url, SourceDataFormat.JSON);
                //TODO, convert into market rate
                marketData = null;
                return true;
            }
            catch (Exception e)
            {
                logger.Log(LogLevel.Error, "unable to retrieve {0} from Barchart data source. error {1}", quoteSymbol, e.Message);
            }

            marketData = null;
            return false;
        }
    }
}
