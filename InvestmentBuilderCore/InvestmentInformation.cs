using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;

namespace InvestmentBuilderCore
{
    public class InvestmentInformation
    {
        public InvestmentInformation(string symbol, 
                                     string exchange,
                                     string currency,
                                     double scalingFactor)
        {
            Symbol = symbol;
            Exchange = exchange;
            Currency = currency;
            ScalingFactor = scalingFactor;
        }

        public string Symbol { get; private set; }
        public string Exchange { get; private set; }
        public string Currency { get; private set; }
        public double ScalingFactor { get; private set; }

        [ContractInvariantMethod]
        protected void ObjectInvariantMethod()
        {
            Contract.Invariant(string.IsNullOrEmpty(Symbol) == false);
            Contract.Invariant(string.IsNullOrEmpty(Currency) == false);
        }
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
        public UserAccountData(string name, string currency, string description, string broker)
        {
            Name = name;
            Currency = currency;
            Description = description;
            Broker = broker;
        }

        public string Name { get; private set; }
        public string Currency { get; private set; }
        public string Description { get; private set; }
        public string Broker { get; private set; }

        [ContractInvariantMethod]
        protected void ObjectInvariantMethod()
        {
            Contract.Invariant(string.IsNullOrEmpty(Name) == false);
            Contract.Invariant(string.IsNullOrEmpty(Currency) == false);
        }

    }

    //this class represents a data point in a performance graph. the
    //point may represent an historical data point in which case the
    //date property will be populated or it may just contain a key 
    //point (i.e. the average yield for a company) in which case the
    //key property will be populated and the date property will be null
    public class HistoricalData
    {
        public HistoricalData(string key, double price)
        {
            Key = key;
            Price = price;
        }

        public HistoricalData(DateTime? date, double price)
        {
            Date = date;
            Price = price;
        }

        public DateTime? Date { get; private set; }
        public string Key { get; private set; }
        public double Price { get; private set; }

        public override string ToString()
        {
            return string.Format("{0}={1}", Date.Value.ToString("dd/MM/yyyy"), Price);
        }

        [ContractInvariantMethod]
        protected void ObjectInvariantMethod()
        {
            Contract.Invariant(string.IsNullOrEmpty(Key) == false ||
                               Date.HasValue == true);
        }
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
        public Redemption(string user, double amount, DateTime date, RedemptionStatus status)
        {
            User = user;
            Amount = amount;
            TransactionDate = date;
            Status = status;
        }

        public string User { get; private set; }
        public double Amount { get; private set; }
        public DateTime TransactionDate { get; private set; }
        public RedemptionStatus Status { get; private set; }

        public void UpdateAmount(double amount)
        {
            Amount = amount;
        }

        [ContractInvariantMethod]
        protected void ObjectInvariantMethod()
        {
            Contract.Invariant(string.IsNullOrEmpty(User) == false);
            Contract.Invariant(Amount > 0);
        }
    }
}
