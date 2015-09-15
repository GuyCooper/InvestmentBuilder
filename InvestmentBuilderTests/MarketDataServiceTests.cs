using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Microsoft.Practices.Unity;
using MarketDataServices;

namespace InvestmentBuilderTests
{
    [TestFixture]
    public class MarketDataServiceTests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            
        }

        private bool _AreEqual(double d1, double d2)
        {
            return Math.Abs(d1 - d1) < double.Epsilon;
        }

        [Test]
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
                                    "Bank Of America",
                                    "USD",
                                    "GBP",
                                    1,
                                    null,
                                    out dResult);

                Assert.IsTrue(success);
                Assert.IsTrue(_AreEqual(TestMarketDataSource.TestPrice * TestMarketDataSource.TestFxRate, dResult));
            }
        }

        [Test]
        public void When_getting_a_closing_price_from_yahoo()
        {
            using (var container = new UnityContainer())
            {
                container.RegisterType<IMarketDataSource, YahooMarketDataSource>();
                container.RegisterType<IMarketDataService, MarketDataService>();

                double dResult;
                bool success = container.Resolve<IMarketDataService>().TryGetClosingPrice(
                                    "VOD",
                                    "LSE",
                                    "Vodaphone",
                                    "GBP",
                                    "GBP",
                                    1,
                                    null,
                                    out dResult);

                Assert.IsTrue(success);
                Assert.IsTrue(dResult > 0d);
            }
        }

        [Test]
        public void When_getting_a_closing_price_from_yahoo_with_fx()
        {
            using (var container = new UnityContainer())
            {
                container.RegisterType<IMarketDataSource, YahooMarketDataSource>();
                container.RegisterType<IMarketDataService, MarketDataService>();

                double dResult;
                bool success = container.Resolve<IMarketDataService>().TryGetClosingPrice(
                                    "BAC",
                                    null,
                                    "Bank Of America",
                                    "USD",
                                    "GBP",
                                    1,
                                    null,
                                    out dResult);

                Assert.IsTrue(success);
                Assert.IsTrue(dResult > 0d);
            }
            
        }

        [Test]
        public void When_getting_a_closing_price_from_aggregate()
        {
            using (var container = new UnityContainer())
            {
                container.RegisterType<IMarketDataSource, AggregatedMarketDataSource>();
                container.RegisterType<IMarketDataService, MarketDataService>();

                double dResult;
                bool success = container.Resolve<IMarketDataService>().TryGetClosingPrice(
                                    "BAC",
                                    null,
                                    "Bank Of America",
                                    "USD",
                                    "GBP",
                                    1,
                                    null,
                                    out dResult);

                Assert.IsTrue(success);
                Assert.IsTrue(dResult > 0d);
            }

        }
    }
}
