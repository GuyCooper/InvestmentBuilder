using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilder;

namespace InvestmentBuilderTests
{
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

        public void When_loading_Trade_File()
        {
            var trades = TradeLoader.GetTrades(Path.Combine(_path,_loadFile));
            var stock = trades.Buys.FirstOrDefault();
            
        }

        public void When_Saving_Trades()
        {
            var trades = new Trades();
            trades.Buys = new[] { new Stock
            {
                 Name = _Stock1,
                 Currency = _TestCurrency,
                 TotalCost = _TestCost,
                 Symbol = _TestSymbol,
                 Number = _TestAmount1,
                 ScalingFactor = _TestScalingFactor
            }};

            trades.Changed = new[] {
                new Stock
                {
                    Name = _Stock2,
                    Number = _TestAmount2
                }
            };

            var outFile = Path.Combine(_path, _saveFile);     
            TradeLoader.SaveTrades(trades, outFile);
        }
    }
}
