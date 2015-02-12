using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace InvestmentBuilderClient
{
    class PaymentsDataViewModel : CashAccountViewModel
    {
        private IList<PaymentTransaction> _paymentsList;
        private static Dictionary<string, Action<PaymentTransaction, double>> _paymentTransactionLookup =
        new Dictionary<string, Action<PaymentTransaction, double>>(StringComparer.CurrentCultureIgnoreCase) {
                {"Admin Fee", (transaction, amount) => { transaction.Other  = amount; }},
                {"Purchase", (transaction, amount) => { transaction.Purchases  = amount; }},
                {"Redemption", (transaction, amount) => { transaction.Withdrawls  = amount; }},
                {"BalanceInHandCF", (transaction, amount) => { transaction.Other  = amount; }},
            };

        public PaymentsDataViewModel(InvestmentDataModel dataModel) :
            base(dataModel)
        {
            _paymentsList = new List<PaymentTransaction>();
            Payments = new BindingList<PaymentTransaction>(_paymentsList);
        }

        public BindingList<PaymentTransaction> Payments { get; private set; }

        public override double GetTransactionData(DateTime dtValuationDate, string transactionMneomic)
        {
            Payments.Clear();
            _dataModel.GetCashAccountData(dtValuationDate, transactionMneomic, (reader) =>
            {
                var transaction = new PaymentTransaction
                {
                    TransactionDate = (DateTime)reader["TransactionDate"],
                    Parameter = (string)reader["Parameter"]
                };

                var amount = (double)reader["Amount"];
                string transactionType = (string)reader["TransactionType"];
                if (_paymentTransactionLookup.ContainsKey(transactionType))
                {
                    _paymentTransactionLookup[transactionType](transaction, amount);
                }
                Payments.Add(transaction);
            });

            return _AddTotalRow(dtValuationDate);
        }

        public override double AddTransaction(DateTime dtTransactionDate, string type, string parameter, double dAmount)
        {
            var transaction = new PaymentTransaction
            {
                TransactionDate = dtTransactionDate.Date,
                Parameter = parameter,
                Added = true
            };

            if (_paymentTransactionLookup.ContainsKey(type))
            {
                _paymentTransactionLookup[type](transaction, dAmount);
            }

            //beforeadding,remove the total row(last row)
            DateTime dtValuation = Payments.Last().TransactionDate;
            Payments.RemoveAt(Payments.Count - 1);
            Payments.Add(transaction);
            //now readd the totals row
            return _AddTotalRow(dtValuation);
        }


        public override double DeleteTransaction(Transaction transaction)
        {
            DateTime dtValuation = Payments.Last().TransactionDate;
            Payments.RemoveAt(Payments.Count - 1);
            //can only remove receipts that have been added
            var payment = transaction as PaymentTransaction;
            //can only remove receipts that have been added
            if (payment != null && payment.Added == true)
            {
                Payments.Remove(payment);
            }
            return _AddTotalRow(dtValuation);
        }

        public override void CommitData()
        {

        }

        protected override double _AddTotalRow(DateTime dtValuationDate)
        {
            //add a totals row at the bottom
            //ReceiptTransaction total = new ReceiptTransaction();
            PaymentTransaction total = Payments.Aggregate(new PaymentTransaction(), (agg, transaction) =>
            {
                agg.Other += transaction.Other;
                agg.Purchases += transaction.Purchases;
                agg.Withdrawls += transaction.Withdrawls;
                return agg;
            });
            total.Parameter = "TOTAL";
            total.TransactionDate = dtValuationDate;
            Payments.Add(total);
            return total.Withdrawls + total.Other + total.Purchases;
        }
    }
}
