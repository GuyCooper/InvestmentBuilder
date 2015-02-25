using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using InvestmentBuilder;
using InvestmentBuilderClient.DataModel;
using NLog;

namespace InvestmentBuilderClient.ViewModel
{
    enum TradeType
    {
        BUY,
        SELL,
        MODIFY
    }

    class TradeDetails : Stock
    {
        public TradeType Type { get; set; }
    }    

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
            foreach(var trade in trades)
            {
                _tradesList.Add(new TradeDetails
                    {
                        TransactionDate = trade.TransactionDate,
                        Type  = type,
                        Name = trade.Name,
                        Number = trade.Number,
                        ScalingFactor = trade.ScalingFactor,
                        Symbol = trade.Symbol,
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
            trades.Buys = Trades.Where(t => t.Type == TradeType.BUY).ToArray();
            trades.Sells = Trades.Where(t => t.Type == TradeType.SELL).ToArray();
            trades.Changed = Trades.Where(t => t.Type == TradeType.MODIFY).ToArray();
            TradeLoader.SaveTrades(trades, _tradeFile);
        }
    }
}
