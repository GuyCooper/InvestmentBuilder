using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace InvestmentBuilderClient
{
    abstract class Transaction
    {
        public DateTime TransactionDate { get; set; }
        //public string TransactionType { get; set; }
        public string Parameter { get; set; }
        //public double Amount { get; set; }
        public bool Added { get; set; }
    }

    class ReceiptTransaction : Transaction
    {
        public double Subscription { get; set; }
        public double Sale { get; set; }
        public double Dividend { get; set; }
        public double Other { get; set; }
    }

    class PaymentTransaction : Transaction
    {

    }

    class CashAccountViewModel
    {
        private IList<ReceiptTransaction> _receiptsList;
        private InvestmentDataModel _dataModel;

        private static Dictionary<string, Action<ReceiptTransaction, double>> _receiptTransactionLookup =
            new Dictionary<string, Action<ReceiptTransaction, double>>(StringComparer.CurrentCultureIgnoreCase) {
                {"Subscription", (transaction, amount) => { transaction.Subscription  = amount; }},
                {"BalanceInHand", (transaction, amount) => { transaction.Subscription  = amount; }},
                {"Sale", (transaction, amount) => { transaction.Sale  = amount; }},
                {"Dividend", (transaction, amount) => { transaction.Dividend  = amount; }},
                {"Interest", (transaction, amount) => { transaction.Other  = amount; }}
            };

        public CashAccountViewModel(InvestmentDataModel dataModel) 
        {
            _receiptsList = new List<ReceiptTransaction>();
            Receipts = new BindingList<ReceiptTransaction>(_receiptsList);
            _dataModel = dataModel;
        }

        public BindingList<ReceiptTransaction> Receipts { get; private set; }

        public double GetReceipts(DateTime dtValuationDate)
        {
            Receipts.Clear();

            _dataModel.GetCashAccountData(dtValuationDate, "R", (reader)=>
                {
                    var transaction = new ReceiptTransaction
                    {
                        TransactionDate = (DateTime)reader["TransactionDate"],
                        Parameter = (string)reader["Parameter"]
                    };

                    var amount =  (double)reader["Amount"];
                    string transactionType = (string)reader["TransactionType"];
                    if(_receiptTransactionLookup.ContainsKey(transactionType))
                    {
                        _receiptTransactionLookup[transactionType](transaction, amount);
                    }
                    Receipts.Add(transaction);
                });

            return AddTotalRow(dtValuationDate);
        }

        public double AddReceipt(DateTime dtTransactionDate, string type, string parameter, double dAmount)
        {
            var transaction = new ReceiptTransaction
                {
                    TransactionDate = dtTransactionDate.Date,
                    Parameter = parameter,
                    Added = true
                };

            if(_receiptTransactionLookup.ContainsKey(type))
            {
                _receiptTransactionLookup[type](transaction, dAmount);
            }

            //beforeadding,remove the total row(last row)
            DateTime dtValuation = Receipts.Last().TransactionDate;
            Receipts.RemoveAt(Receipts.Count - 1);
            Receipts.Add(transaction);
            //now readd the totals row
            return AddTotalRow(dtValuation);
        }

        public double DeleteTransaction(Transaction transaction)
        {
            DateTime dtValuation = Receipts.Last().TransactionDate;
            Receipts.RemoveAt(Receipts.Count - 1);
            var receipt = transaction as ReceiptTransaction;
            //can only remove receipts that have been added
            if(receipt != null && receipt.Added == true)
            {
                Receipts.Remove(receipt);
            }
            return AddTotalRow(dtValuation);
        }

        private double AddTotalRow(DateTime dtValuationDate)
        {
            //add a totals row at the bottom
            //ReceiptTransaction total = new ReceiptTransaction();
            ReceiptTransaction total = Receipts.Aggregate(new ReceiptTransaction(), (agg, transaction) =>
            {
                agg.Other += transaction.Other;
                agg.Dividend += transaction.Dividend;
                agg.Sale += transaction.Sale;
                agg.Subscription += transaction.Subscription;
                return agg;
            });
            total.Parameter = "TOTAL";
            total.TransactionDate = dtValuationDate;
            Receipts.Add(total);
            return total.Dividend + total.Other + total.Sale + total.Subscription;
        }
    }
}
