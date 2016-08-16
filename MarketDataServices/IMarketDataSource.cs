using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderCore;

namespace MarketDataServices
{
    public class MarketDataPrice
    {
        public string Name { get; set; }
        public string Symbol { get; set; }
        public double Price { get; set; }
        public string Currency { get; set; }
        public string Exchange { get; set; }
    }

    public interface IMarketDataSource
    {
        /// <summary>
        /// returns list of source names for this market data source
        /// </summary>
        /// <returns></returns>
        IList<string> GetSources();
        /// <summary>
        /// try  to get market price for symbol
        /// </summary>
        /// <param name="symbol">name of instrument</param>
        /// <param name="dData">price</param>
        /// <returns>true success, false fail</returns>
        bool TryGetMarketData(string symbol, string exchange, string source, out MarketDataPrice marketData);
        /// <summary>
        /// try to fx rate for ccy pair, return true for success, false for fail 
        /// </summary>
        /// <param name="baseCurrency"></param>
        /// <param name="contraCurrency"></param>
        /// <param name="dFxRate"></param>
        /// <returns></returns>
        bool TryGetFxRate(string baseCurrency, string contraCurrency, string source, out double dFxRate);
        /// <summary>
        /// try to retrieve historical data for instrument, return data if success, null for fail
        /// </summary>
        /// <param name="instrument"></param>
        /// <param name="dtFrom"></param>
        /// <returns></returns>
        IEnumerable<HistoricalData> GetHistoricalData(string instrument, string exchange, string source, DateTime dtFrom);
        /// <summary>
        /// name ofsource
        /// </summary>
        string Name { get; }
        /// <summary>
        /// for multiple data sources this value orders them in priority order
        /// 1 = highest
        /// </summary>
        int Priority { get; }
    }

}
