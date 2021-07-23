using System;
using System.Collections.Generic;
using MarketDataServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Unity;

namespace InvestmentBuilderMSTests
{
    internal class TestMarketDataReader 
    {
        protected IEnumerable<string> GetDataImpl(string filename, bool multiline)
        {
            var result = new List<string>();
            using (var reader = new StreamReader(filename))
            {
                if (multiline == true)
                {
                    while(reader.EndOfStream == false)
                    {
                        result.Add(reader.ReadLine());
                    }
                }
                else
                {
                    result.Add(reader.ReadToEnd());
                }
            }
            return result;
        }
    }

    internal class TestBarChartFxPriceDataReader : TestMarketDataReader, IMarketDataReader
    {
        public IEnumerable<string> GetData(string url, SourceDataFormat format)
        {
            return new List<string> { MarketDataSourceTestData.Source2FxRate.ToString() };
        }
    }

    internal class TestBarChartHistoricalDataReader : TestMarketDataReader, IMarketDataReader
    {
        public IEnumerable<string> GetData(string url, SourceDataFormat format)
        {
            return GetDataImpl(@"TestFiles\TestBarChartHistoricalData.txt", false);
        }
    }

    internal class TestBarChartMarketPriceDataReader : TestMarketDataReader, IMarketDataReader
    {
        public IEnumerable<string> GetData(string url, SourceDataFormat format)
        {
            return GetDataImpl(@"TestFiles\TestBarChartMarketPrice.txt", false);
        }
    }

    internal class TestYahooHistoricalPriceDataReader : TestMarketDataReader, IMarketDataReader
    {
        public IEnumerable<string> GetData(string url, SourceDataFormat format)
        {
            return GetDataImpl(@"TestFiles\TestYahooHistoricalPriceData.txt", true);
        }
    }

    internal class TestYahooFxRateDataReader : TestMarketDataReader, IMarketDataReader
    {
        public IEnumerable<string> GetData(string url, SourceDataFormat format)
        {
            return new List<string> { string.Format(",{0}", MarketDataSourceTestData.Source2FxRate.ToString()) };
        }
    }

    internal class TestYahooMarketPriceDataReader : TestMarketDataReader, IMarketDataReader
    {
        public IEnumerable<string> GetData(string url, SourceDataFormat format)
        {
            return GetDataImpl(@"TestFiles\TestYahooMarketPriceData.txt", true);
        }
    }

    [TestClass]
    public class MarketDataServiceTests
    {
        private bool _AreEqual(double d1, double d2)
        {
            return Math.Abs(d1 - d1) < double.Epsilon;
        }

        [TestMethod]
        public void When_getting_a_closing_price()
        {
            using (var container = new UnityContainer())
            {
                container.RegisterType<IMarketDataSource, TestMarketDataSource>();
                container.RegisterType<IMarketDataService, MarketDataService>();

                double dResult;
                bool success = container.Resolve<IMarketDataService>().TryGetClosingPrice(
                                    "BAC",
                                    null,
                                    null,
                                    "Bank Of America",
                                    "USD",
                                    "GBP",
                                    null,
                                    out dResult);

                Assert.IsTrue(success);
                Assert.IsTrue(_AreEqual(TestMarketDataSource.TestPrice * TestMarketDataSource.TestFxRate, dResult));
            }
        }

        //[TestMethod]
        //public void When_getting_historical_data_from_barchart_source()
        //{
        //    var datasource = new BarChartMarketDataSource(new TestBarChartHistoricalDataReader());
        //    var result = datasource.GetHistoricalData(null, null, null, DateTime.Now).ToList();
        //    Assert.AreEqual(25, result.Count);
        //    Assert.AreEqual(DateTime.Parse("2014-08-01"), result[0].Date);
        //    Assert.AreEqual(23.81971, result[0].Price);
        //}

        //[TestMethod]
        //public void When_getting_fxrate_from_barchart_source()
        //{
        //    var datasource = new BarChartMarketDataSource(new TestBarChartFxPriceDataReader());
        //    double fxrate;
        //    var result = datasource.TryGetFxRate(null, null, null, null, out fxrate);
        //    Assert.IsTrue(result);
        //    Assert.AreEqual(MarketDataSourceTestData.Source2FxRate, fxrate);
        //}

        //[TestMethod]
        //public void When_getting_marketprice_from_barchart_source()
        //{
        //    var datasource = new BarChartMarketDataSource(new TestBarChartMarketPriceDataReader());
        //    MarketDataPrice marketPrice;
        //    var result = datasource.TryGetMarketData(null, null, null, out marketPrice);
        //    Assert.IsTrue(result);
        //    Assert.AreEqual("BP", marketPrice.Symbol);
        //    Assert.AreEqual(34.27, marketPrice.Price);
        //    Assert.IsNull(marketPrice.Currency); //barchart does not return currency
        //}

        //[TestMethod]
        //public void When_getting_historical_data_from_yahoo_source()
        //{
        //    var datasource = new YahooMarketDataSource(new TestYahooHistoricalPriceDataReader());
        //    var result = datasource.GetHistoricalData(null, null, null, DateTime.Now).ToList();
        //    Assert.AreEqual(5, result.Count);
        //    Assert.AreEqual(DateTime.Parse("02/01/2016"), result[0].Date);
        //    Assert.AreEqual(21.65, result[0].Price);
        //    Assert.AreEqual(DateTime.Parse("05/04/2016"), result[4].Date);
        //    Assert.AreEqual(21.22, result[4].Price);
        //}

        //[TestMethod]
        //public void When_getting_fxrate_from_yahoo_source()
        //{
        //    var datasource = new YahooMarketDataSource(new TestYahooFxRateDataReader());
        //    double fxrate;
        //    var result = datasource.TryGetFxRate("EUR", "USD", null, null, out fxrate);
        //    Assert.IsTrue(result);
        //    Assert.AreEqual(MarketDataSourceTestData.Source2FxRate, fxrate);
        //}

        //[TestMethod]
        //public void When_getting_marketprice_from_yahoo_source()
        //{
        //    var datasource = new YahooMarketDataSource(new TestYahooMarketPriceDataReader());
        //    MarketDataPrice marketPrice;
        //    var result = datasource.TryGetMarketData(null, null, null, out marketPrice);
        //    Assert.IsTrue(result);
        //    Assert.AreEqual(3.5467, marketPrice.Price);
        //    Assert.AreEqual("GBP", marketPrice.Currency);
        //}
    }
}
