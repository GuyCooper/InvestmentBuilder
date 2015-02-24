using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using NLog;

namespace MarketDataServices
{
    public class HistoricalData
    {
        public DateTime Date { get; set; }
        public double Price { get; set; }
    }

    /// <summary>
    /// class provides market data services. provides closing prices and currency conversion for stock symbols
    /// </summary>
    public class MarketDataService
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static Dictionary<string,double> _fxLookup = new Dictionary<string,double>();
        //method downloads the specified url and reutrns the contents as a string
        private static IEnumerable<string> _GetData(string url)
        {
            HttpWebRequest request = null;
            var result = new List<string>();
            try
            {
                request = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
                request.Timeout = 30000;

                using (var response = (HttpWebResponse)request.GetResponse())
                using (StreamReader input = new StreamReader(
                    response.GetResponseStream()))
                {
                    while(input.EndOfStream == false)
                    {
                        result.Add(input.ReadLine());
                    }
                }
            }
            catch(Exception e)
            {
                logger.Log(LogLevel.Error, string.Format("unable to get data from url: {0}, error: {1}", url, e.Message));
                //Console.WriteLine("unable to get data from url: {0}, error: {1}",url,e.Message);
                throw;
            }
            return result;
        }

        private static double _GetFxRate(string currency)
        {
            if(!_fxLookup.ContainsKey(currency))
            {
                string fxurl = string.Format("http://finance.yahoo.com/d/quotes.csv?s={0}GBP=X&f=sl1", currency);
                var result = _GetData(fxurl).ToList().First();
                var dFx = _GetDoubleFromResult(result,1);
                _fxLookup.Add(currency,dFx);
                return dFx;
            }
            return _fxLookup[currency];
        }
       private static double _GetDoubleFromResult(string result, int index)
        {
            string[] arr = result.Split(',');
            return double.Parse(arr[index]);
        }
        /// <summary>
        /// get previous closing price for symbol. convert to sterling if required
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="name"></param>
        /// <param name="exchange"></param>
        /// <returns></returns>
        public static bool TryGetClosingPrice(string symbol, string name, string currency, double dScaling, out double dClosing)
        {
            dClosing = 0d;
            string closingurl = String.Format(
                "http://finance.yahoo.com/d/quotes.csv?s={0}&f=pnx", symbol);

            logger.Log(LogLevel.Info, string.Format("getting closing price for : {0}", name));
            //Console.WriteLine("getting closing price for : {0}", name);
            try
            {
                string result = _GetData(closingurl).ToList().First();
                dClosing = _GetDoubleFromResult(result,0);
                //string resName = arr[1].Trim(new [] {'\n','\r',' ','\"'});
                //string resExchange = arr[2].Trim(new[] { '\n', '\r', ' ', '\"' });

                //now determine if we need to do an fx conversion to get this price in sterling
                if(currency == "GBP")
                {
                    //sterling stocks are price in pence, but displayed in £ on the spreadsheet as it is easier to read
                    dClosing = dClosing / dScaling;
                }
                else
                {
                    //download the fx mid price rate for currency / GBP
                    var dFx = _GetFxRate(currency);
                    dClosing = dClosing * dFx * dScaling;
                }
                
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
                return true;
            }
            catch(Exception e)
            {
                logger.Log(LogLevel.Error, string.Format("unable to get closing price for {0}: {1}", symbol, e.Message));
                //Console.WriteLine("unable to get closing price for {0}: {1}", symbol, e.Message);
            }
            return false;
        }
        public static IEnumerable<HistoricalData> GetHistoricalData(string instrument, DateTime dtFrom)
        {
            string url = string.Format("http://ichart.yahoo.com/table.csv?s={0}&a={1}&b={2}&c={3}&g=m&ignore=.csv", 
                instrument, dtFrom.Month - 1, dtFrom.Day, dtFrom.Year);
            var data = _GetData(url);

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

    }
}
