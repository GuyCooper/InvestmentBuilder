using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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

    internal abstract class CashAccountViewModel
    {
        protected InvestmentDataModel _dataModel;
        protected DateTime? _latestValuationDate;

        public CashAccountViewModel(InvestmentDataModel dataModel) 
        {
            
            _dataModel = dataModel;
            _latestValuationDate = _dataModel.GetLatestValuationDate();
        }

        public abstract double GetTransactionData(DateTime dtValuationDate);

        public abstract double AddTransaction(DateTime dtTransactionDate, string type, string parameter, double dAmount);

        public abstract double DeleteTransaction(Transaction transaction);

    }
}
