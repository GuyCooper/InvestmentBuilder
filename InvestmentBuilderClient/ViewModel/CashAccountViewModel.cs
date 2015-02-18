using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using InvestmentBuilderClient.DataModel;

namespace InvestmentBuilderClient.ViewModel
{
    abstract class Transaction
    {
        public DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public string Parameter { get; set; }
        public double Amount { get; set; }
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
        public double Withdrawls { get; set; }
        public double Purchases { get; set; }
        public double Other { get; set; }
    }

    internal abstract class CashAccountViewModel
    {
        protected InvestmentDataModel _dataModel;
        protected DateTime? _latestValuationDate;

        public CashAccountViewModel(InvestmentDataModel dataModel) 
        {
            _dataModel = dataModel;
            _latestValuationDate = _dataModel.GetLatestValuationDate();
        }

        public abstract double GetTransactionData(DateTime dtValuationDate, string transactionMneomic);

        public abstract double AddTransaction(DateTime dtTransactionDate, string type, string parameter, double dAmount);

        public abstract double DeleteTransaction(Transaction transaction);

        public abstract void CommitData(DateTime dtValuation);

        protected double _DeleteTransactionImpl(Transaction transaction, BindingList<Transaction> bindingList)
        {
            DateTime dtValuation = bindingList.Last().TransactionDate;
            bindingList.RemoveAt(bindingList.Count - 1);
            //can only remove receipts that have been added
            if (transaction != null && transaction.Added == true)
            {
                bindingList.Remove(transaction);
            }
            return _AddTotalRow(dtValuation);
        }

        protected abstract double _AddTotalRow(DateTime dtValuationDate);

    }
}
