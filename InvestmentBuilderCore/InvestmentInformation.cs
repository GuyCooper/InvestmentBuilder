﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;

namespace InvestmentBuilderCore
{
    /// <summary>
    /// Class defines  a single instrument (i.e. company) including the details of its source
    /// information
    /// </summary>
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

    /// <summary>
    /// CashAccountData class. Defines the cash flow for an account.
    /// </summary>
    public class CashAccountData
    {
        public CashAccountData()
        {
            Dividends = new Dictionary<string, double>();
        }
        public Dictionary<string, double> Dividends { get; private set; }
        public double BankBalance { get; set; }
    }

    /// <summary>
    /// Class defines the valuation of a single instrument (company) on a specific date
    /// </summary>
    public class CompanyData
    {
        public string Name { get; set; }
        public DateTime ValuationDate { get; set; }
        public DateTime LastBrought {get;set;}
        public double Quantity {get;set;}
        public double AveragePricePaid { get; set; }
        public double TotalCost { get; set; }
        public double SharePrice { get; set; }
        public double NetSellingValue { get; set; }
        public double ProfitLoss { get; set; }
        public double MonthChange { get; set; }
        public double MonthChangeRatio { get; set; }
        public double Dividend { get; set; }
        public string ManualPrice { get; set; }
        public double TotalReturn { get; set; }
    }

    //this class represents a data point in a performance graph. the
    //point may represent an historical data point in which case the
    //date property will be populated or it may just contain a key 
    //point (i.e. the average yield for a company) in which case the
    //key property will be populated and the date property will be null
    public class HistoricalData
    {
        private HistoricalData()
        {
        }

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

        /// <summary>
        /// Rebase the price from the base price.
        /// </summary>
        public HistoricalData RebasePrice(double basePrice)
        {
            return new HistoricalData
            {
                Date = Date,
                Key = Key,
                Price = 1 + ((Price - basePrice) / basePrice)
            };
        }

        public HistoricalData YieldAdjustment(Dictionary<int, double> montlyAdjustmentLookup)
        {
            double yieldAdjustment = 0;  
            if(Date != null && montlyAdjustmentLookup != null)
            {
                montlyAdjustmentLookup.TryGetValue(Date.Value.Year, out yieldAdjustment);
            }

            return new HistoricalData
            {
                Date = Date,
                Key = Key,
                Price = Price + (Price * yieldAdjustment)
            };
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

    /// <summary>
    /// Manual Prices class. lookup of investment name to manual price. curency is 
    /// always considered to be the same as the reporting currency for the account
    /// </summary>
    public class ManualPrices : Dictionary<string, double>
    {
        public ManualPrices() : base(StringComparer.InvariantCultureIgnoreCase) { }
    }

    /// <summary>
    /// TradeType Enum. 
    /// </summary>
    public enum TradeType
    {
        BUY,
        SELL,
        MODIFY
    } 

    /// <summary>
    /// Redemption Status.
    /// </summary>
    public enum RedemptionStatus
    {
        Pending,
        Complete,
        Failed
    }

    /// <summary>
    /// Redemption Class. Contains details about a requested redemetion. Redemptions have to be
    /// recoreded as requested because they cannot be issued until the account has been valued.
    /// </summary>
    public class Redemption
    {
        public Redemption(int id, string user, double amount, DateTime date, RedemptionStatus status)
        {
            Id = id;
            User = user;
            Amount = amount;
            TransactionDate = date;
            Status = status;
        }

        public int Id { get; private set; }
        public string User { get; private set; }
        public double Amount { get; private set; }
        public DateTime TransactionDate { get; private set; }
        public double RedeemedUnits { get; set; }
        public RedemptionStatus Status { get; set; }

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

    /// <summary>
    /// Transaction class. Defines a single investment transaction. 
    /// </summary>
    public class Transaction
    {
        public string InvestmentName { get; set; }
        public TradeType TransactionType { get; set; }
        public double Quantity { get; set; }
        public double Amount { get; set; }
    }
}
