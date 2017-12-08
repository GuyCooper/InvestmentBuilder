using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using InvestmentBuilderCore;
using NLog;

namespace MarketDataServices
{
    /// <summary>
    /// aggregates all market data sources and iterates through each one to get 
    /// source data until succeeds
    /// </summary>
    internal class AggregatedMarketDataSource : IMarketDataSource
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private IMarketSourceLocator _sourceLocator;

        public string Name { get { return "Aggregated"; } }

        public AggregatedMarketDataSource(IMarketSourceLocator sourceLocator)
        {
            _sourceLocator = sourceLocator;
        }

        public IList<string> GetSources()
        {
            return _sourceLocator.Sources.SelectMany(x => x.GetSources()).ToList();
        }

        public bool TryGetMarketData(string symbol, string exchange, string source, out MarketDataPrice marketData)
        {
            foreach(var element in _GetOrderedDataSources(source))
            {
                if(element.TryGetMarketData(symbol, exchange, source, out marketData))
                {
                    return true;
                }
            }

            marketData = null;
            return false;
        }

        public bool TryGetFxRate(string baseCurrency, string contraCurrency, string exchange, string source, out double dFxRate)
        {
            foreach (var element in _GetOrderedDataSources(source))
            {
                if (element.TryGetFxRate(baseCurrency, contraCurrency, exchange, source, out dFxRate))
                {
                    return true;
                }
            }

            dFxRate = 0d;
            return false;
        }

        public IEnumerable<HistoricalData> GetHistoricalData(string instrument, string exchange, string source,  DateTime dtFrom)
        {
            foreach (var element in _GetOrderedDataSources(source))
            {
                var result = element.GetHistoricalData(instrument, exchange, source, dtFrom);
                if(result != null)
                {
                    return result;
                }
            }
            return null;
        }

        private IList<IMarketDataSource> _GetOrderedDataSources(string source)
        {
            if(string.IsNullOrEmpty(source) == false)
            {
                var element = _sourceLocator.Sources.FirstOrDefault(x => source.Equals(x.Name));
                if (element != default(IMarketDataSource))
                    return new List<IMarketDataSource> { element };

                logger.Log(LogLevel.Warn, "unable to locate market source {0}!", source);
            }

            return _sourceLocator.Sources.OrderBy(x => x.Priority).ToList();
            
        }
        public int Priority { get { return 0; } }

        public void Initialise(IConfigurationSettings settings) { }
    }
}
