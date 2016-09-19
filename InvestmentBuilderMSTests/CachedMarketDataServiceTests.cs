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
    internal class TestMarketDataSerialiser : IMarketDataSerialiser
    {
        public TestMarketDataSerialiser()
        {
            Data = new List<string>();
        }

        public IList<string> Data { get; private set; }
        public void EndSerialiser() { }
        public void SerialiseData(string data, params object[] prm)
        {
            Data.Add(string.Format(data, prm));
        }
        public void StartSerialiser() { }
    }

    [TestClass]
    public class CachedMarketDataServiceTests
    {
        private TestMarketDataSerialiser _dataserialiser;
        private CachedMarketDataSource _datasource;

        [TestInitialize]
        public void Setup()
        {
            _dataserialiser = new TestMarketDataSerialiser();
            _datasource = new CachedMarketDataSource(_dataserialiser, new TestMarketSourceLocater());
        }

        [TestMethod]
        public void When_getting_cached_datasources()
        {
            var result = _datasource.GetSources();
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(MarketDataSourceTestData.SourceName1, result[0]);
            Assert.AreEqual(MarketDataSourceTestData.SourceName2, result[1]);
        }

        [TestMethod]
        public void When_getting_cached_historical_data()
        {
            var result = _datasource.GetHistoricalData(MarketDataSourceTestData.TestName, null, null, DateTime.Parse(MarketDataSourceTestData.HistoricalDateString)).ToList();
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(MarketDataSourceTestData._TestHistoricalData.Price, result[0].Price);
        }

        [TestMethod]
        public void When_getting_cached_historical_data1()
        {
            var result = _datasource.GetHistoricalData(MarketDataSourceTestData.TestName, null, null, DateTime.Parse(MarketDataSourceTestData.HistoricalDateString)).ToList();
            var result1 = _datasource.GetHistoricalData(MarketDataSourceTestData.TestName, null, null, DateTime.Parse(MarketDataSourceTestData.HistoricalDateString).AddDays(1)).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(0, result1.Count);
        }

        [TestMethod]
        public void When_getting_cached_historical_data2()
        {
            var result = _datasource.GetHistoricalData(MarketDataSourceTestData.TestName, null, null, DateTime.Parse(MarketDataSourceTestData.HistoricalDateString)).ToList();
            var result1 = _datasource.GetHistoricalData(MarketDataSourceTestData.TestName, null, null, DateTime.Parse(MarketDataSourceTestData.HistoricalDateString).AddDays(-1)).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result1.Count);
            Assert.AreEqual(result[0].Price, result1[0].Price);
        }

        [TestMethod]
        public void When_getting_all_cached_data()
        {
            MarketDataPrice price1, price2;
            IList<HistoricalData> historicalData1, historicalData2;
            double fxRate1, fxRate2;

            var result = _datasource.TryGetMarketData(null, null, null, out price1);
            Assert.IsTrue(result);
            result = _datasource.TryGetMarketData(null, null, null, out price2);
            Assert.IsTrue(result);

            Assert.AreEqual(price1.Price, price2.Price);

            result = _datasource.TryGetFxRate(null, null, null, null, out fxRate1);
            Assert.IsTrue(result);

            result = _datasource.TryGetFxRate(null, null, null, null, out fxRate2);
            Assert.IsTrue(result);

            Assert.AreEqual(fxRate1, fxRate2);

            var testDate = DateTime.Parse(MarketDataSourceTestData.HistoricalDateString);
            historicalData1 = _datasource.GetHistoricalData(MarketDataSourceTestData.TestName, null, null, testDate).ToList();
            Assert.IsNotNull(historicalData1);

            historicalData2 = _datasource.GetHistoricalData(MarketDataSourceTestData.TestName, null, null, testDate).ToList();
            Assert.IsNotNull(historicalData2);

            Assert.AreEqual(historicalData1[0].Price, historicalData2[0].Price);

            //now test serialisation
            _datasource.Dispose();

            Assert.AreEqual(3, _dataserialiser.Data.Count);
            var expected1 = MarketDataSourceTestData._Source2MarketPrice.ToString();
            var expected2 = "," + MarketDataSourceTestData.Source2FxRate;
            var expected3 = string.Format("{0},{1}", MarketDataSourceTestData.TestName, MarketDataSourceTestData._TestHistoricalData.ToString());
            var result1 = _dataserialiser.Data[0].Substring(_dataserialiser.Data[0].IndexOf(',')+1);
            var result2 = _dataserialiser.Data[1].Substring(_dataserialiser.Data[1].IndexOf(',') + 1);
            var result3 = _dataserialiser.Data[2].Substring(_dataserialiser.Data[2].IndexOf(',') + 1);
            //var result2 = _dataserialiser.Data[1].Substring(_dataserialiser.Data[0].IndexOf(',') + 1);
            Assert.IsTrue(result1.Equals(expected1));
            Assert.IsTrue(result2.Equals(expected2));
            Assert.IsTrue(result3.Equals(expected3));
        }
    }
}
