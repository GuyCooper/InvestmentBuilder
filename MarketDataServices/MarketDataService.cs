using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using NLog;
using System.Diagnostics.Contracts;

namespace MarketDataServices
{
    [ContractClass(typeof(MarketDataServiceContract))]
    public interface IMarketDataService
    {
        bool TryGetClosingPrice(string symbol, string exchange, string source, string name, string currency, string reportingCurrency, double? dOverride, out double dClosing);
        IList<string> GetSources();
    }

    [ContractClassFor(typeof(IMarketDataService))]
    internal abstract class MarketDataServiceContract : IMarketDataService
    {
        public IList<string> GetSources()
        {
            Contract.Ensures(Contract.Result<IList<string>>() != null);
            return null;
        }

        public bool TryGetClosingPrice(string symbol, string exchange, string source, string name, string currency, string reportingCurrency, double? dOverride, out double dClosing)
        {
            Contract.Requires(string.IsNullOrEmpty(symbol) == false);
            Contract.Requires(string.IsNullOrEmpty(currency) == false);
            Contract.Requires(string.IsNullOrEmpty(reportingCurrency) == false);
            dClosing = 0;
            return false;
        }
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

        public IList<string> GetSources()
        {
            return _marketSource.GetSources();
        }

        /// <summary>
        /// get previous closing price for symbol. convert to reportng currency if required
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="name"></param>
        /// <param name="exchange"></param>
        /// <returns></returns>
        public bool TryGetClosingPrice(string symbol, string exchange, string source, string name, string currency, string reportingCurrency, double? dOverride, out double dClosing)
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
                if(_marketSource.TryGetMarketData(symbol, exchange, source, out marketData) == false)
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
                if (_marketSource.TryGetFxRate(localCurrency, reportingCurrency, exchange, source, out dFx))
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
