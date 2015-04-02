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
        private IUnityContainer _container;

        [TestFixtureSetUp]
        public void Setup()
        {
            _container = new UnityContainer();
            _container.RegisterType<IMarketDataSource, TestMarketDataSource>();
            _container.RegisterType<IMarketDataService, MarketDataService>();
        }

        private bool _AreEqual(double d1, double d2)
        {
            return Math.Abs(d1 - d1) < double.Epsilon;
        }

        [Test]
        public void When_getting_a_closing_price()
        {
            double dResult;
            bool success = _container.Resolve<IMarketDataService>().TryGetClosingPrice(
                                "BOA",
                                "Vodaphone",
                                "USD",
                                "GBP",
                                1,
                                out dResult);

            Assert.IsTrue(success);
            Assert.IsTrue(_AreEqual(TestMarketDataSource.TestPrice * TestMarketDataSource.TestFxRate, dResult));
        }
    }
}
