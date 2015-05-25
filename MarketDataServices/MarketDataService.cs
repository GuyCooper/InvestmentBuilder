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
    public interface IMarketDataService
    {
        bool TryGetClosingPrice(string symbol, string exchange, string name, string currency, string reportingCurrency, double dScaling, out double dClosing);
    }

    /// <summary>
    /// class provides market data services. provides closing prices and currency conversion for stock symbols
    /// </summary>
    public class MarketDataService : IMarketDataService
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private IMarketDataSource _marketSource;

        public MarketDataService(IMarketDataSource marketSource)
        {
            _marketSource = marketSource;
        }

        /// <summary>
        /// get previous closing price for symbol. convert to sterling if required
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="name"></param>
        /// <param name="exchange"></param>
        /// <returns></returns>
        public bool TryGetClosingPrice(string symbol, string exchange, string name, string currency, string reportingCurrency, double dScaling, out double dClosing)
        {
            logger.Log(LogLevel.Info, string.Format("getting closing price for : {0}", name));

            dClosing = 0d;
            if(_marketSource.TryGetMarketData(symbol, exchange, out dClosing))
            {
                if (currency != reportingCurrency)
                {
                    //need todo an fx conversion to get correct price
                    double dFx;
                    if(_marketSource.TryGetFxRate(currency, reportingCurrency, out dFx ))
                    {
                        dClosing = dClosing * dFx;
                    }
                    else
                    {
                        return false;
                    }
                }
                //now scale price todisplay price.i.e. exchange may price in pence but we want it displayed
                //in pounds
                dClosing = dClosing / dScaling;
                return true;
            }
            return false;
        }
    }
}
