using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using InvestmentBuilder;
using InvestmentBuilderClient.DataModel;
using NLog;

namespace InvestmentBuilderClient.ViewModel
{
    internal class TradeViewModel
    {
        private string _tradeFile;
        private List<TradeDetails> _tradesList;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public TradeViewModel(string tradefile)
        {
            _tradesList = new List<TradeDetails>();

            _tradeFile = tradefile;
            var trades = TradeLoader.GetTrades(_tradeFile);
            _LoadTrades(trades.Buys, TradeType.BUY);
            _LoadTrades(trades.Sells, TradeType.SELL);
            _LoadTrades(trades.Changed, TradeType.MODIFY);
            Trades = new BindingList<TradeDetails>(_tradesList);
        }

        private void _LoadTrades(IEnumerable<Stock> trades, TradeType type)
        {
            if(trades == null)
            {
                return;
            }

            foreach(var trade in trades)
            {
                _tradesList.Add(new TradeDetails
                    {
                        TransactionDate = trade.TransactionDate,
                        Action  = type,
                        Name = trade.Name,
                        Quantity = trade.Quantity,
                        ScalingFactor = trade.ScalingFactor,
                        Symbol = trade.Symbol,
                        Exchange = trade.Exchange,
                        Currency = trade.Currency,
                        TotalCost = trade.TotalCost
                    });
            }
        }

        public BindingList<TradeDetails> Trades { get; private set; }

        public void AddTrade(TradeDetails trade)
        {
            Trades.Add(trade);
        }

        public void RemoveTrade(TradeDetails trade)
        {
            Trades.Remove(trade);
        }

        public void CommitTrades()
        {
            logger.Log(LogLevel.Info, "commiting trade data...");
            var trades = new Trades();
            trades.Buys = Trades.Where(t => t.Action == TradeType.BUY).Select(t => new Stock(t)).ToArray();
            trades.Sells = Trades.Where(t => t.Action == TradeType.SELL).Select(t => new Stock(t)).ToArray();
            trades.Changed = Trades.Where(t => t.Action == TradeType.MODIFY).Select(t => new Stock(t)).ToArray();
            TradeLoader.SaveTrades(trades, _tradeFile);
        }
    }
}
