using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MarketDataServices;
using InvestmentBuilderCore;

namespace InvestmentBuilderMSTests
{
    internal static class MarketDataSourceTestData
    {
        public static readonly string SourceName1 = "Foo";
        public static readonly string SourceName2 = "Bar";
        public static readonly int Source1Priority = 3;
        public static readonly int Source2Prioroty = 2;
        public static readonly double Source1FxRate = 1.543;
        public static readonly double Source2FxRate = 2.971;
        public static readonly string TestName = "TestCompany1";
        public static readonly double TestPrice = 162.98;
        public static readonly double TestPrice2 = 13.21;
        public static string HistoricalDateString = "15/11/2015";

        public static readonly MarketDataPrice _Source2MarketPrice = new MarketDataPrice
        {
            Currency = "EUR",
            Name = TestName,
            Price = TestPrice,
        };

        public static HistoricalData _TestHistoricalData = new HistoricalData
        {
            Date = DateTime.Parse(HistoricalDateString),
            Price = TestPrice2
        };
    }

    internal class TestMarketSource1 : IMarketDataSource
    {
        public string Name {  get { return MarketDataSourceTestData.SourceName1; } }

        public int Priority {  get { return MarketDataSourceTestData.Source1Priority; } }

        public IEnumerable<HistoricalData> GetHistoricalData(string instrument, string exchange, string source, DateTime dtFrom)
        {
            return new List<HistoricalData> { MarketDataSourceTestData._TestHistoricalData };
        }

        public IList<string> GetSources()
        {
            return new List<string> { Name };
        }

        public bool TryGetFxRate(string baseCurrency, string contraCurrency, string exchange, string source, out double dFxRate)
        {
            dFxRate = MarketDataSourceTestData.Source1FxRate;
            return true;
        }

        public bool TryGetMarketData(string symbol, string exchange, string source, out MarketDataPrice marketData)
        {
            marketData = null;
            return false;
        }

        public IMarketDataReader DataReader { get; set; }
    }

    internal class TestMarketSource2 : IMarketDataSource
    {
        public string Name { get { return MarketDataSourceTestData.SourceName2; } }

        public int Priority { get { return MarketDataSourceTestData.Source2Prioroty; } }

        public IEnumerable<HistoricalData> GetHistoricalData(string instrument, string exchange, string source, DateTime dtFrom)
        {
            return null;
        }

        public IList<string> GetSources()
        {
            return new List<string> { Name };
        }

        public bool TryGetFxRate(string baseCurrency, string contraCurrency, string exchange, string source, out double dFxRate)
        {
            dFxRate = MarketDataSourceTestData.Source2FxRate;
            return true;
        }

        public bool TryGetMarketData(string symbol, string exchange, string source, out MarketDataPrice marketData)
        {
            marketData = MarketDataSourceTestData._Source2MarketPrice;
            return true;
        }

        public IMarketDataReader DataReader { get; set; }
    }

    internal class TestMarketSourceLocater : IMarketSourceLocator
    {
        public IEnumerable<IMarketDataSource> Sources
        {
            get
            {
                return new List<IMarketDataSource>
               {
                   new TestMarketSource1(),
                   new TestMarketSource2()
               };
            }
        }
    }

    /// <summary>
    /// these tests test the functionality of aggregated market data services
    /// </summary>
    [TestClass]
    public class AggregatedMarketDataServiceTests
    {
        private AggregatedMarketDataSource _datasource;
        [TestInitialize]
        public void Setup()
        {
            _datasource = new AggregatedMarketDataSource(new TestMarketSourceLocater());
        }

        [TestMethod]
        public void When_getting_agregated_data_sources()
        {
            var result = _datasource.GetSources();
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(MarketDataSourceTestData.SourceName1, result[0]);
            Assert.AreEqual(MarketDataSourceTestData.SourceName2, result[1]);
        }

        [TestMethod]
        public void When_getting_aggregated_market_data()
        {
            MarketDataPrice price;
            var result = _datasource.TryGetMarketData(null, null, null, out price);
            Assert.IsTrue(result);
            Assert.AreEqual(MarketDataSourceTestData._Source2MarketPrice.Price, price.Price);
        }

        [TestMethod]
        public void When_getting_aggregated_market_data_from_source()
        {
            MarketDataPrice price;
            var result = _datasource.TryGetMarketData(null, null, MarketDataSourceTestData.SourceName1, out price);
            Assert.IsFalse(result);
            Assert.IsNull(price);
        }

        [TestMethod]
        public void When_getting_aggregated_fxRate()
        {
            double rate;
            var result = _datasource.TryGetFxRate(null, null,null, null, out rate);
            Assert.IsTrue(result);
            Assert.AreEqual(MarketDataSourceTestData.Source2FxRate, rate);
        }

        [TestMethod]
        public void When_getting_aggregated_fxRate_from_source()
        {
            double rate;
            var result = _datasource.TryGetFxRate(null, null, null, MarketDataSourceTestData.SourceName1, out rate);
            Assert.IsTrue(result);
            Assert.AreEqual(MarketDataSourceTestData.Source1FxRate, rate);
        }

        [TestMethod]
        public void When_getting_aggregated_historical_rates()
        {
            var result = _datasource.GetHistoricalData(null, null, null, DateTime.Now).ToList();
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(MarketDataSourceTestData._TestHistoricalData.Price, result[0].Price);
        }

        [TestMethod]
        public void When_getting_aggregated_historical_rate_for_source()
        {
            var result = _datasource.GetHistoricalData(null, null, MarketDataSourceTestData.SourceName2, DateTime.Now);
            Assert.IsNull(result);
        }
    }
}
