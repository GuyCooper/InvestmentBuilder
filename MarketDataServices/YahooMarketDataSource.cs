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
    public class YahooMarketDataSource : IMarketDataSource
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private Dictionary<string, double> _fxLookup = new Dictionary<string, double>();

        private static Dictionary<string, string> _exchangeMapper = new Dictionary<string, string>()
        {
            {"LSE","L"},
            {"DAX","DE"},
            {"MIL","MI"}
        };

        public string Name { get { return "Yahoo"; } }

        public bool TryGetMarketData(string symbol, string exchange, out double dData)
        { 
            if(string.IsNullOrEmpty(exchange) == false &&
               symbol.Contains('.') == false && 
               _exchangeMapper.ContainsKey(exchange))
            {
                symbol = string.Format("{0}.{1}", symbol, _exchangeMapper[exchange]);
            }

            string url = String.Format("http://finance.yahoo.com/d/quotes.csv?s={0}&f=pnx", symbol);
            try
            {
                string result = WebDataHandler.GetData(url).ToList().First();
                dData = _GetDoubleFromResult(result, 0);
                return true;
            }
            catch(Exception e)
            {
                logger.Log(LogLevel.Error, "unable to retrieve {0} from yahoo: {1}", symbol, e.Message);
            }
            dData = 0d;
            return false;
        }

        public bool TryGetFxRate(string baseCurrency, string contraCurrency, out double dFxRate)
        {
            var ccypair = baseCurrency + contraCurrency;
            if (_fxLookup.ContainsKey(ccypair))
            {
                dFxRate = _fxLookup[ccypair];
                return true;
            }

            try
            {
                string fxurl = string.Format("http://finance.yahoo.com/d/quotes.csv?s={0}{1}=X&f=sl1", baseCurrency, contraCurrency);
                var result = WebDataHandler.GetData(fxurl).ToList().First();
                dFxRate = _GetDoubleFromResult(result, 1);
                _fxLookup.Add(ccypair, dFxRate);
                return true;
            }
            catch(Exception e)
            {
                logger.Log(LogLevel.Error, "unable to retrieve FX rate for {0} from yahoo: {1}", ccypair, e.Message);
            }

            dFxRate = 0d;
            return false;
        }

        public IEnumerable<HistoricalData> GetHistoricalData(string instrument, DateTime dtFrom)
        {
            logger.Log(LogLevel.Info, "retrieving historical data for {0} from yahoo.", instrument);

            string url = string.Format("http://ichart.yahoo.com/table.csv?s={0}&a={1}&b={2}&c={3}&g=m&ignore=.csv",
                instrument, dtFrom.Month - 1, dtFrom.Day, dtFrom.Year);

            try
            {
                var data = WebDataHandler.GetData(url);

                return data.Select(x =>
                {
                    var elems = x.Split(',');
                    var strDate = elems[0];
                    var strClosing = elems[4];
                    DateTime dtDate;
                    double dPrice;
                    if (DateTime.TryParse(strDate, out dtDate) && double.TryParse(strClosing, out dPrice))
                    {
                        return new HistoricalData
                        {
                            Date = DateTime.Parse(strDate),
                            Price = double.Parse(strClosing)
                        };
                    }
                    return null;
                }).Where(x => x != null);
            }
            catch(Exception e)
            {
                logger.Log(LogLevel.Error, "unable to retrieve historical data for {0} from yahoo: {1}", instrument, e.Message);
            }

            return null;
        }

        /// <summary>
        /// helper method for retrieving data from url. supports multiline responses such as
        /// historical data. If unable to rerieve data, will throw an exception
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        //private IEnumerable<string> _GetData(string url)
        //{
        //    HttpWebRequest request = null;
        //    var result = new List<string>();
        //    request = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
        //    request.Timeout = 30000;

        //    using (var response = (HttpWebResponse)request.GetResponse())
        //    using (StreamReader input = new StreamReader(
        //        response.GetResponseStream()))
        //    {
        //        while (input.EndOfStream == false)
        //        {
        //            result.Add(input.ReadLine());
        //        }
        //    }
        //    return result;
        //}
 
        /// <summary>
        /// helpermethod parses the result and extracts the price
        /// </summary>
        /// <param name="result"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private double _GetDoubleFromResult(string result, int index)
        {
            string[] arr = result.Split(',');
            return double.Parse(arr[index]);
        }
    }
}

//common yahoo finance urls
//http://download.finance.yahoo.com/d/quotes.csv?s=GBPEUR=X&f=sl1d1t1ba&e=.csv
//http://finance.yahoo.com/d/quotes.csv?s=GBPEUR=X&f=sl1d1t1
//http://finance.yahoo.com/d/quotes.csv?s=GBPEUR=X&f=sl1
//verify name and exchange
//if ((resName.Equals(name, StringComparison.CurrentCultureIgnoreCase) == false)
//    || (resExchange.Equals(exchange, StringComparison.CurrentCultureIgnoreCase) == false))
//{
//    throw new ApplicationException("incorrect name or exchange");
//}
//historical:
//http://ichart.yahoo.com/table.csv?s=BAS.DE&a=0&b=1&c=2000 &d=0&e=31&f=2010&g=w&ignore=.csv	
//this downloads FTSE 100 historical since 1/1/2010:
//http://ichart.yahoo.com/table.csv?s=^FTSE&a=0&b=1&c=2010&g=m&ignore=.csv