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
        bool TryGetClosingPrice(string symbol, string exchange, string name, string currency, string reportingCurrency, double? dOverride, out double dClosing);
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
        /// get previous closing price for symbol. convert to reportng currency if required
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="name"></param>
        /// <param name="exchange"></param>
        /// <returns></returns>
        public bool TryGetClosingPrice(string symbol, string exchange, string name, string currency, string reportingCurrency, double? dOverride, out double dClosing)
        {
            logger.Log(LogLevel.Info, string.Format("getting closing price for : {0}", name));

            dClosing = 0d;
            string localCurrency = currency;
            if(dOverride.HasValue)
            {
                dClosing = dOverride.Value;
            }
            else
            {
                MarketDataPrice marketData;
                if(_marketSource.TryGetMarketData(symbol, exchange, out marketData) == false)
                {
                    return false;
                }
                dClosing = marketData.Price;
                if (marketData.Currency != null)
                {
                    localCurrency = marketData.Currency;
                }
            }
            if (localCurrency != reportingCurrency)
            {
                //need todo an fx conversion to get correct price
                double dFx;
                if (_marketSource.TryGetFxRate(localCurrency, reportingCurrency, out dFx))
                {
                    dClosing = dClosing * dFx;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
    }
}
