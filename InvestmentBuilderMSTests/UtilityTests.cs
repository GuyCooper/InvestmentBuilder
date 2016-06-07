using System.Linq;
using InvestmentBuilderCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InvestmentBuilderMSTests
{
    [TestClass]
    public class UtilityTests
    {
        //private const string _path = @"C:\Projects\TestData\InvestmentBuilder";
        //private const string _TestTradeFile = "testAggregateTrades.xml";

        [TestMethod]
        public void When_AggregatingTradeList(string tradeFile)
        {
            var trades = TradeLoader.GetTrades(tradeFile);
            var result = InvestmentUtils.AggregateStocks(trades.Buys).ToList();
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(result.Select(x => x.Name).Count(), result.Select(x => x.Name).Distinct().Count());
        }
    }
}
