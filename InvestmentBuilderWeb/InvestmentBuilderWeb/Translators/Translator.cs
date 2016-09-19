using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InvestmentBuilder;
using InvestmentBuilderCore;
using InvestmentBuilderWeb.Models;
using System.Reflection;

namespace InvestmentBuilderWeb.Translators
{
    internal static class Translator
    {
        public static Trades ToTrades(this Stock stock, TransactionType transactionType)
        {
            var trades = new  Trades();
            var arrStock = new List<Stock>{stock}.ToArray();
            switch(transactionType)
            {
                case TransactionType.BUY:
                    trades.Buys = arrStock;
                    break;
                case TransactionType.SELL:
                    trades.Sells = arrStock;
                    break;
                case TransactionType.CHANGE:
                    trades.Changed = arrStock;
                    break;
            }
            return trades;
        }
       
        public static ManualPrices GetManualPrices(this TradeItemModel tradeItem)
        {
            if (tradeItem.ManualPrice.HasValue == true)
            {
                return new ManualPrices { { tradeItem.Name, tradeItem.ManualPrice.Value } };
            }
            return null;
        }

        private static T _CloneObject<T>(Type objType, object objFrom, Func<T> creator ) where T : class
        {
            var props = objType.GetProperties(/*BindingFlags.DeclaredOnly |*/
                                                            BindingFlags.Instance |
                                                            BindingFlags.Public);
            T objTo = creator();
            foreach (var prop in props)
            {
                var val = prop.GetValue(objFrom);
                if (val != null)
                {
                    if (objTo.GetType().GetProperty(prop.Name) != null)
                    {
                        prop.GetSetMethod().Invoke(objTo, new[] { val });
                    }
                }
            }
            return objTo;
        }

        public static CompanyDataModel ToCompanyDataModel(this CompanyData companyData)
        {
            return _CloneObject<CompanyDataModel>(companyData.GetType(), companyData, () => new CompanyDataModel());
        }

        public static TradeItemModel ToTradeItemModel(this Stock trade)
        {
            return _CloneObject<TradeItemModel>(trade.GetType(), trade, () => new TradeItemModel());
        }

        public static ReceiptCashFlowModel ToReceiptCashFlowModel(this ReceiptTransaction receipt)
        {
            return _CloneObject<ReceiptCashFlowModel>(receipt.GetType(), receipt, () => new ReceiptCashFlowModel());
        }

        public static PaymentCashFlowModel ToPaymentCashFlowModel(this PaymentTransaction payment)
        {
            return _CloneObject<PaymentCashFlowModel>(payment.GetType(), payment, () => new PaymentCashFlowModel());
        }

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
    }
}