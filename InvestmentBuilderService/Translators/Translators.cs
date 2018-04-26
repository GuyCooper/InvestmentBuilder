using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderService.Dtos;
using InvestmentBuilderCore;

namespace InvestmentBuilderService.Translators
{
    internal static class Translators
    {
        public static InvestmentSummaryModel ToInvestmentSummaryModel(this AssetReport report)
        {
            //return _CloneObject<InvestmentSummaryModel>(report.GetType(), report, () => new InvestmentSummaryModel());
            return new InvestmentSummaryModel
            {
                AccountName = report.AccountName,
                BankBalance = report.BankBalance.ToString("#0.00"),
                MonthlyPnL = report.MonthlyPnL.ToString("#0.00"),
                NetAssets = report.NetAssets.ToString("#0.00"),
                ReportingCurrency = report.ReportingCurrency,
                TotalAssets = report.TotalAssets.ToString("#0.00"),
                TotalAssetValue = report.TotalAssetValue.ToString("#0.00"),
                ValuationDate = report.ValuationDate,
                ValuePerUnit = report.ValuePerUnit.ToString("#0.00")
            };
        }

        public static Trades ToInternalTrade(this Stock tradeItem, TransactionType action)
        {
            var trades = new Trades();
            var arrStock = new List<Stock> { tradeItem }.ToArray();
            switch (action)
            {
                case TransactionType.Buy:
                    trades.Buys = arrStock;
                    break;
                case TransactionType.Sell:
                    trades.Sells = arrStock;
                    break;
                case TransactionType.Change:
                    trades.Changed = arrStock;
                    break;
            }
            return trades;
        }

        public static Trades ToInternalTrade(this TradeItemDto tradeItem)
        {
            var stock = new Stock
            {
                Currency = tradeItem.Currency,
                Exchange = tradeItem.Exchange,
                Name = tradeItem.ItemName,
                Quantity = tradeItem.Quantity,
                Symbol = tradeItem.Symbol,
                TotalCost = tradeItem.TotalCost,
                TransactionDate = tradeItem.TransactionDate
            };
            return stock.ToInternalTrade(tradeItem.Action);
        }
    }
}
