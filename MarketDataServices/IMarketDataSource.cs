using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderCore;
using System.Diagnostics.Contracts;

namespace MarketDataServices
{
    public class MarketDataPrice
    {
        public MarketDataPrice(string name, string symbol, double price,
                               string currency = null, string exchange = null)
        {
            Name = name;
            Symbol = symbol;
            Price = price;
            Currency = currency;
            Exchange = exchange;
        }

        public string Name { get; private set; }
        public string Symbol { get; private set; }
        public double Price { get; private set; }
        public string Currency { get; private set; }
        public string Exchange { get; private set; }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", Symbol, Price, Currency, Exchange);
        }

        public void DecimalisePrice()
        {
            //deciamlise price if required
            if (Currency[Currency.Length - 1] == 'p')
            {
                Price = Price / 100d;
                Currency = Currency.ToUpper();
            }
        }

        [ContractInvariantMethod]
        protected void ObjectInvarianceCheck()
        {
            Contract.Invariant(string.IsNullOrEmpty(Name) == false);
            Contract.Invariant(Price > 0);
            Contract.Invariant(string.IsNullOrEmpty(Symbol) == false);
        }
    }

    [ContractClass(typeof(MarketDataSourceContract))]
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
        bool TryGetFxRate(string baseCurrency, string contraCurrency, string exchange, string source, out double dFxRate);
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
        /// <summary>
        /// property setter for injecting datasource 
        /// </summary>
        IMarketDataReader DataReader { get; set; }
    }

    [ContractClassFor(typeof(IMarketDataSource))]
    internal abstract class MarketDataSourceContract : IMarketDataSource
    {
        public IMarketDataReader DataReader { get; set; }

        public string Name
        { get
            {
                Contract.Ensures(string.IsNullOrEmpty(Contract.Result<string>()) == false);
                return null;
            } }

        public int Priority
        { get
            {
                Contract.Ensures(Contract.Result<int>() > 0);
                return 0;
            } }

        public IEnumerable<HistoricalData> GetHistoricalData(string instrument, string exchange, string source, DateTime dtFrom)
        {
            Contract.Requires(string.IsNullOrEmpty(instrument) == false);
            throw new NotImplementedException();
        }

        public IList<string> GetSources()
        {
            Contract.Ensures(Contract.Result<IList<string>>() != null);
            throw new NotImplementedException();
        }

        public bool TryGetFxRate(string baseCurrency, string contraCurrency, string exchange, string source, out double dFxRate)
        {
            Contract.Requires(string.IsNullOrEmpty(baseCurrency) == false);
            Contract.Requires(string.IsNullOrEmpty(contraCurrency) == false);
            dFxRate = 0;
            return false;
        }

        public bool TryGetMarketData(string symbol, string exchange, string source, out MarketDataPrice marketData)
        {
            Contract.Requires(string.IsNullOrEmpty(symbol) == false);
            marketData = null;
            return false;
        }
    }

}
