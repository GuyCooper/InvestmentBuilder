using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderCore;
using NUnit.Framework;

namespace InvestmentBuilderTests
{
    [TestFixture]
    public class TradeLoaderTests
    {
        private const string _path = @"C:\Projects\TestData\InvestmentBuilder";
        private const string _loadFile = "Trades.xml";
        private const string _saveFile = "SavedTrades.xml";

        private const string _Stock1 = "Acme Co";
        private const string _Stock2 = "Bby Ltd";
        private const string _TestCurrency = "CAD";
        private const int _TestAmount1 = 159;
        private const int _TestAmount2 = 54;
        private const double _TestCost = 3932.87d;
        private const string _TestSymbol = "ACM.CA";
        private const double _TestScalingFactor = 1;
        private static DateTime _TestDate =   DateTime.Parse("30/03/2015");

        private const string _TestCompany = "iShares FTSE100 ETF";

        [Test]
        public void When_loading_Trade_File(string tradeFile)
        {
            var trades = TradeLoader.GetTrades(tradeFile);
            var stock = trades.Buys.FirstOrDefault();

            Assert.IsNotNull(stock);
            Assert.AreEqual(stock.Name, _TestCompany);
        }

        private Stock _CreateTrade()
        {
            return new Stock
            {
                Name = _Stock1,
                Currency = _TestCurrency,
                TotalCost = _TestCost,
                Symbol = _TestSymbol,
                Quantity = _TestAmount1,
                ScalingFactor = _TestScalingFactor,
                TransactionDate = _TestDate
            };
        }

        [Test]
        public void When_Saving_Trades()
        {
            var trades = new Trades();
            trades.Buys = new[] { _CreateTrade() };

            trades.Changed = new[] {
                new Stock
                {
                    Name = _Stock2,
                    Quantity = _TestAmount2
                }
            };

            var outFile = Path.Combine(_path, _saveFile);     
            TradeLoader.SaveTrades(trades, outFile);
        }

        [Test]
        public void When_Saving_Trades1()
        {
            var trades = new Trades();
            trades.Buys = new List<Stock>().ToArray();
            trades.Sells = new List<Stock>().ToArray();
            trades.Changed = new List<Stock>().ToArray();

            var outFile = Path.Combine(_path, _saveFile);
            TradeLoader.SaveTrades(trades, outFile);
        }

        [Test]
        public void When_Saving_Trades2()
        {
            var trades = new Trades();
            trades.Buys = new[] { _CreateTrade() };

            var outFile = Path.Combine(_path, _saveFile);
            TradeLoader.SaveTrades(trades, outFile);
        }

    }
}
