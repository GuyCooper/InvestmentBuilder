using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using MarketDataServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InvestmentBuilderMSTests
{
    [TestClass]
    public class MarketDataServiceTests
    {
        [TestInitialize]
        public void Setup()
        {
            
        }

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
                                    "Bank Of America",
                                    "USD",
                                    "GBP",
                                    null,
                                    out dResult);

                Assert.IsTrue(success);
                Assert.IsTrue(_AreEqual(TestMarketDataSource.TestPrice * TestMarketDataSource.TestFxRate, dResult));
            }
        }

        [TestMethod]
        public void When_getting_a_closing_price_from_yahoo()
        {
            using (var container = new UnityContainer())
            {
                container.RegisterType<IMarketDataSource, YahooMarketDataSourceOld>();
                container.RegisterType<IMarketDataService, MarketDataService>();

                double dResult;
                bool success = container.Resolve<IMarketDataService>().TryGetClosingPrice(
                                    "VOD",
                                    "LSE",
                                    "Vodaphone",
                                    "GBP",
                                    "GBP",
                                    null,
                                    out dResult);

                //the result is dependant on if there is connectivity or not
                //Assert.IsTrue(success);
                //Assert.IsTrue(dResult > 0d);
            }
        }

        [TestMethod]
        public void When_getting_a_closing_price_from_yahoo_with_fx()
        {
            using (var container = new UnityContainer())
            {
                container.RegisterType<IMarketDataSource, YahooMarketDataSourceOld>();
                container.RegisterType<IMarketDataService, MarketDataService>();

                double dResult;
                bool success = container.Resolve<IMarketDataService>().TryGetClosingPrice(
                                    "BAC",
                                    null,
                                    "Bank Of America",
                                    "USD",
                                    "GBP",
                                    null,
                                    out dResult);

                //Assert.IsTrue(success);
                //Assert.IsTrue(dResult > 0d);
            }
            
        }

        [TestMethod]
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
                                    null,
                                    out dResult);

                //success depends on if we are online or not!!!

            }
        }
    }
}
