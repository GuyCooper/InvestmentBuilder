using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using InvestmentBuilderCore;
using System.IO;

namespace MarketDataServices
{
    /// <summary>
    /// this class stores a cache of the market data. delegates
    /// the retrieval of market data to the aggregated market data
    /// source. Will cache data once retrieved and only request data
    /// from the source if it is not already cached. a timer
    /// will periodically update the cache
    /// </summary>
    public class CachedMarketDataSource : IMarketDataSource, IDisposable
    {
        //name of file to persist the cache to on shutdown

        private string _fileName = null;

        //source of the market data. 
        private IMarketDataSource _sourceMarketData = null;

        private Dictionary<string, MarketDataPrice> _marketDataPriceCache;
        private Dictionary<string, double> _fxPriceCache;
        private Dictionary<string, IList<HistoricalData>> _historicalDataCache;

        public CachedMarketDataSource(string fileName)
        {
            _fileName = fileName;
            _sourceMarketData = new AggregatedMarketDataSource();
            _marketDataPriceCache = new Dictionary<string, MarketDataPrice>();
            _fxPriceCache = new Dictionary<string, double>();
            _historicalDataCache = new Dictionary<string, IList<HistoricalData>>();
        }

        public string Name
        {
            get { return "CachedMarketData"; }
        }

        public void Dispose()
        {
            if(_fileName != null)
            {
                using (var writer = new StreamWriter(_fileName))
                {
                    //dump market data
                    foreach (var marketItem in _marketDataPriceCache)
                    {
                        writer.WriteLine("M,{0},{1},{2}", marketItem.Value.Symbol,
                                                          marketItem.Value.Price,
                                                          marketItem.Value.Currency);
                    }

                    //dump fx data
                    foreach (var fxItem in _fxPriceCache)
                    {
                        writer.WriteLine("F,{0},{1}", fxItem.Key, fxItem.Value);
                    }

                    //dump historical data. has format
                    //instrument,name,date1=val1:date2=val2:etc...
                    foreach(var historicalData in _historicalDataCache)
                    {
                        var prices = historicalData.Value.Select(x =>
                                        string.Format("{0}={1}", x.Date.Value.ToString("dd/MM/yyyy"), x.Price));

                        writer.WriteLine("H,{0},{1}", historicalData.Key,
                        string.Join(":", prices));
                    }
                }
            }
        }

        public IEnumerable<HistoricalData> GetHistoricalData(string instrument, DateTime dtFrom)
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
                    cache = _sourceMarketData.GetHistoricalData(instrument, dtFrom).ToList();
                    _historicalDataCache[instrument] = cache;
                }
            }

            //retrieve the data from the data source and populate it with the cache
            cache = _sourceMarketData.GetHistoricalData(instrument, dtFrom).ToList();
            _historicalDataCache.Add(instrument, cache);
            return cache;
        }

        public bool TryGetFxRate(string baseCurrency, string contraCurrency, out double dFxRate)
        {
            if (_fxPriceCache.TryGetValue(baseCurrency + contraCurrency, out dFxRate) == true)
                return true;

            if(_sourceMarketData.TryGetFxRate(baseCurrency, contraCurrency, out dFxRate) == true)
            {
                _fxPriceCache.Add(baseCurrency + contraCurrency, dFxRate);
                return true;
            }

            return false;
        }

        public bool TryGetMarketData(string symbol, string exchange, out MarketDataPrice marketData)
        {
            if (_marketDataPriceCache.TryGetValue(symbol + exchange, out marketData) == true)
                return true;

            if (_sourceMarketData.TryGetMarketData(symbol, exchange, out marketData) == true)
            {
                _marketDataPriceCache.Add(symbol + exchange, marketData);
                return true;
            }

            return false;

        }
    }
}
