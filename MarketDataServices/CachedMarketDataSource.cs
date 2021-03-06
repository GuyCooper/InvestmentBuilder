﻿using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using InvestmentBuilderCore;
using System.Threading.Tasks;
using InvestmentBuilderCore.Schedule;

namespace MarketDataServices
{
    /// <summary>
    /// this class stores a cache of the market data. delegates
    /// the retrieval of market data to the aggregated market data
    /// source. Will cache data once retrieved and only request data
    /// from the source if it is not already cached. a timer
    /// will periodically update the cache on shutdown the cache is then
    /// serialised to disk in a format that can be used by the test
    /// market data source
    /// </summary>
    internal class CachedMarketDataSource : IMarketDataSource, IDisposable
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public CachedMarketDataSource(IMarketDataSerialiser dataSerialiser, IMarketSourceLocator sourceLocator)
        {
            _dataSerialiser = dataSerialiser;
            _sourceMarketData = new AggregatedMarketDataSource(sourceLocator);
            _marketDataPriceCache = new Dictionary<string, MarketDataPrice>();
            _fxPriceCache = new Dictionary<string, double>();
            _historicalDataCache = new Dictionary<string, IList<HistoricalData>>();
        }

        #endregion

        #region IMarketDataSource

        /// <summary>
        /// Name of datasource.
        /// </summary>
        public string Name
        {
            get { return "CachedMarketData"; }
        }

        /// <summary>
        /// returns list of source names for this market data source
        /// </summary>
        public IList<string> GetSources()
        {
            return _sourceMarketData.GetSources();
        }

        /// <summary>
        /// Try to retrieve historical data for instrument, return data if success, null for fail.
        /// </summary>
        public IEnumerable<HistoricalData> GetHistoricalData(string instrument, string exchange, string source, DateTime dtFrom)
        {
            IList<HistoricalData> cache;
            if (_historicalDataCache.TryGetValue(instrument, out cache) == true)
            {
                if(cache.FirstOrDefault(x => x.Date.HasValue && x.Date.Value <= dtFrom) != null)
                {
                    //the cache does contain historical data for this instrument and date
                    return cache.Where(x => x.Date.Value >= dtFrom).ToList();
                }
                else
                {
                    cache = _sourceMarketData.GetHistoricalData(instrument, exchange, source, dtFrom).ToList();
                    _historicalDataCache[instrument] = cache;
                    return cache;
                }
            }

            //retrieve the data from the data source and populate it with the cache
            var enCache = _sourceMarketData.GetHistoricalData(instrument, exchange, source, dtFrom);
            if(enCache != null)
            {
                cache = enCache.ToList();
                _historicalDataCache.Add(instrument, cache);
            }

            return cache;
        }

        /// <summary>
        /// Try to fx rate for ccy pair, return true for success, false for fail. 
        /// </summary>
        public bool TryGetFxRate(string baseCurrency, string contraCurrency, string exchange, string source, out double dFxRate)
        {
            if (_fxPriceCache.TryGetValue(baseCurrency + contraCurrency, out dFxRate) == true)
                return true;

            if(_sourceMarketData.TryGetFxRate(baseCurrency, contraCurrency, exchange, source, out dFxRate) == true)
            {
                _fxPriceCache.Add(baseCurrency + contraCurrency, dFxRate);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Try to get market price for symbol.
        /// </summary>
        public bool TryGetMarketData(string symbol, string exchange, string source, out MarketDataPrice marketData)
        {
            if (_marketDataPriceCache.TryGetValue(symbol + exchange, out marketData) == true)
                return true;

            if (_sourceMarketData.TryGetMarketData(symbol, exchange, source, out marketData) == true)
            {
                _marketDataPriceCache.Add(symbol + exchange, marketData);
                return true;
            }

            return false;

        }

        /// <summary>
        /// for multiple data sources this value orders them in priority order
        /// 1 = highest
        /// </summary>
        public int Priority { get { return 0; } }

        /// <summary>
        /// Initialise datasource with configuration settings.
        /// </summary>
        public void Initialise(IConfigurationSettings settings, ScheduledTaskFactory scheduledTaskFactory) { }

        /// <summary>
        /// Asynchronously request a price from the data source.
        /// </summary>
        public async Task<MarketDataPrice> RequestPrice(string symbol, string exchange, string source)
        {
            var key = $"{symbol}{exchange}";
            MarketDataPrice marketData;
            if(_marketDataPriceCache.TryGetValue(key, out marketData) == true)
            {
                return marketData;
            }

            marketData = await _sourceMarketData.RequestPrice(symbol, exchange, source);
            if(marketData != null)
            {
                _marketDataPriceCache.Add(key, marketData);
            }
            return marketData;
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Persist the contents of the cache to file so it can be loaded on the next startup.
        /// </summary>
        public void Dispose()
        {
            if (_dataSerialiser != null)
            {
                try
                {
                    _dataSerialiser.StartSerialiser();
                    //dump market data
                    foreach (var marketItem in _marketDataPriceCache)
                    {
                        _dataSerialiser.SerialiseData("M,{0}", marketItem.Value.ToString());
                    }

                    //dump fx data
                    foreach (var fxItem in _fxPriceCache)
                    {
                        _dataSerialiser.SerialiseData("F,{0},{1}", fxItem.Key, fxItem.Value);
                    }

                    //dump historical data. has format
                    //instrument,name,date1=val1:date2=val2:etc...
                    foreach (var historicalData in _historicalDataCache)
                    {
                        var prices = historicalData.Value.Select(x => x.ToString());
                        _dataSerialiser.SerialiseData("H,{0},{1}", historicalData.Key,
                        string.Join(":", prices));
                    }
                }
                finally
                {
                    _dataSerialiser.EndSerialiser();
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Load market data from the dataserialiser.
        /// </summary>
        private void LoadSerialisedData()
        {
            //now load in any serialised data
            if (_dataSerialiser != null)
            {
                _dataSerialiser.LoadData((record) =>
                {
                    var elems = record.Split(',');
                    if (elems.Length > 1)
                    {
                        var ch = elems[0];
                        var symbol = elems[1];
                        if (ch == "M")
                        {
                            //_marketDataPriceCache
                        }
                        else if (ch == "F")
                        {

                        }
                        else if (ch == "H")
                        {

                        }
                    }
                });
            }
        }

        #endregion

        #region Private Data

        //name of file to persist the cache to on shutdown
        private static Logger logger = LogManager.GetCurrentClassLogger();

        //source of the market data. 
        private IMarketDataSource _sourceMarketData = null;
        private IMarketDataSerialiser _dataSerialiser;

        // The caches...
        private Dictionary<string, MarketDataPrice> _marketDataPriceCache;
        private Dictionary<string, double> _fxPriceCache;
        private Dictionary<string, IList<HistoricalData>> _historicalDataCache;

        #endregion
    }

}
