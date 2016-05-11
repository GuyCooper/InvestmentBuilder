using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestmentBuilderCore
{
    public class InvestmentInformation
    {
        public string Symbol { get; set; }
        public string Exchange { get; set; }
        public string Currency { get; set; }
        public double ScalingFactor { get; set; }
    }

    public class CashAccountData
    {
        public CashAccountData()
        {
            Dividends = new Dictionary<string, double>();
        }
        public Dictionary<string, double> Dividends { get; private set; }
        public double BankBalance { get; set; }
    }

    public class CompanyData
    {
        public string Name { get; set; }
        public DateTime ValuationDate { get; set; }
        public DateTime LastBrought {get;set;}
        public int Quantity {get;set;}
        public double AveragePricePaid { get; set; }
        public double TotalCost { get; set; }
        public double SharePrice { get; set; }
        public double NetSellingValue { get; set; }
        public double ProfitLoss { get; set; }
        public double MonthChange { get; set; }
        public double MonthChangeRatio { get; set; }
        public double Dividend { get; set; }
        public string ManualPrice { get; set; }
    }

    public class UserAccountData
    {
        public string Name { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
        public string Broker { get; set; }
    }

    public class HistoricalData
    {
        public DateTime Date { get; set; }
        public double Price { get; set; }
    }

    public class ManualPrices : Dictionary<string, double>
    {
        public ManualPrices() : base(StringComparer.InvariantCultureIgnoreCase) { }
    }

    public enum TradeType
    {
        BUY,
        SELL,
        MODIFY
    } 

    public enum RedemptionStatus
    {
        Pending,
        Complete,
        Failed
    }

    public class Redemption
    {
        public string User { get; set; }
        public double Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public RedemptionStatus Status { get; set; }
    }
}
