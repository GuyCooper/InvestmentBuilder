﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using InvestmentBuilderCore;
using System.IO;

namespace InvestmentBuilderTests
{
    [TestFixture]
    public class UtilityTests
    {
        //private const string _path = @"C:\Projects\TestData\InvestmentBuilder";
        //private const string _TestTradeFile = "testAggregateTrades.xml";

        [Test]
        public void When_AggregatingTradeList(string tradeFile)
        {
            var trades = TradeLoader.GetTrades(tradeFile);
            var result = InvestmentUtils.AggregateStocks(trades.Buys).ToList();
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(result.Select(x => x.Name).Count(), result.Select(x => x.Name).Distinct().Count());
        }
    }
}
