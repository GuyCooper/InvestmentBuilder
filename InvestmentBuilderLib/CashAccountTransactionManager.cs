using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderCore;
using System.Data;
using NLog;

namespace InvestmentBuilder
{
    //transaction class.used for binding the cash account transactions to a displayable
    //view.
    public abstract class Transaction
    {
        public DateTime ValuationDate { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public string Parameter { get; set; }
        public double Amount { get; set; }
        public bool Added { get; set; }
        public bool IsTotal { get; set; }
    }

    public class ReceiptTransaction : Transaction
    {
        public double Subscription { get; set; }
        public double Sale { get; set; }
        public double Dividend { get; set; }
        public double Other { get; set; }
    }

    public class PaymentTransaction : Transaction
    {
        public double Withdrawls { get; set; }
        public double Purchases { get; set; }
        public double Other { get; set; }
    }

    public class CashAccountTransactionManager
    {
        private ICashAccountInterface _cashAccountData;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        //this structure maps a transaction type onto its transaction property
        private Dictionary<string, string> _TransactionLookup =
                new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase) {
                {"Subscription", "Subscription"}, 
                {"BalanceInHand", "Subscription"},
                {"Sale", "Sale"},
                {"Dividend", "Dividend"},
                {"Interest", "Other"},
                {"Admin Fee", "Other"},
                {"Purchase", "Purchases"},
                {"Redemption", "Withdrawls"},
                {"BalanceInHandCF", "Other"}
            };

        public CashAccountTransactionManager(IDataLayer dataLayer)
        {
            _cashAccountData = dataLayer.CashAccountData;
        }

        public string PaymentMnemomic { get { return "P"; } }
        public string ReceiptMnemomic { get { return "R"; } }

        public IList<PaymentTransaction> GetPaymentTransactions(UserAccountToken userToken, DateTime dtValuationDate, out double dTotal)
        {
            var transactions = new List<PaymentTransaction>();
            _GetTransactionsImpl<PaymentTransaction>(userToken, dtValuationDate, PaymentMnemomic,
                                                        transactions);
            dTotal = _AddTotalRow<PaymentTransaction>(dtValuationDate, transactions);
            return transactions;
        }

        public IList<ReceiptTransaction> GetReceiptTransactions(UserAccountToken userToken, DateTime dtValuationDate, DateTime? dtPreviousValuationDate, 
                                            out double dTotal)
        {
            var transactions = new List<ReceiptTransaction>();
            _GetTransactionsImpl<ReceiptTransaction>(userToken, dtValuationDate, ReceiptMnemomic,
                                                        transactions);
            //add the balance in handfrom the previous monthif it is not already there
            if (dtPreviousValuationDate.HasValue && dtValuationDate > dtPreviousValuationDate)
            {
                if (transactions.FirstOrDefault(r => r.TransactionType == "BalanceInHand") == null)
                {
                    var dAmount = _cashAccountData.GetBalanceInHand(userToken, dtPreviousValuationDate.Value);
                    
                    var transaction = new ReceiptTransaction
                    {
                        Parameter = "BalanceInHand",
                        Added = true,
                        TransactionDate = dtValuationDate,
                        TransactionType = "BalanceInHand",
                        Subscription = dAmount,
                        Amount = dAmount
                    };
                    transactions.Add(transaction);
                    //we also need to add the balance in hand transaction to the database
                    //so the validation will work
                    AddTransaction(userToken, dtValuationDate, dtValuationDate, transaction.TransactionType,
                        transaction.Parameter, transaction.Subscription);
                }
            }
            dTotal = _AddTotalRow<ReceiptTransaction>(dtValuationDate, transactions);
            return transactions;
        }

        private void _GetTransactionsImpl<T>(UserAccountToken userToken, DateTime dtValuationDate, string mnenomic,
             IList<T> transactions) where T : Transaction, new()
        {
            _cashAccountData.GetCashAccountTransactions(userToken, mnenomic, dtValuationDate, (reader) =>
            {
                var transaction = new T();
                transaction.ValuationDate = dtValuationDate;
                transaction.TransactionDate = (DateTime)reader["TransactionDate"];
                transaction.Parameter = (string)reader["Parameter"];
                transaction.TransactionType = (string)reader["TransactionType"];

                var amount = (double)reader["Amount"];
                transaction.Amount = amount;

                string transactionProperty;
                if (_TransactionLookup.TryGetValue(transaction.TransactionType, out transactionProperty))
                {
                    //map the transaction type to the transaction property
                    var propInfo = transaction.GetType().GetProperty(transactionProperty);
                    if (propInfo != null)
                    {
                        propInfo.SetValue(transaction, amount);
                    }
                    transactions.Add(transaction);
                }
            });
        }

        private double _AddTotalRow<T>(DateTime dtValuationDate, IList<T> transactions) where T : Transaction, new()
        {
            //add a totals row at the bottom
            //ReceiptTransaction total = new ReceiptTransaction();
            var props = typeof(T).GetProperties();
            T total = transactions.Aggregate(new T(), (agg, transaction) =>
            {
                foreach(var prop in props)
                {
                    if(prop.PropertyType.Name == "Double")
                    {
                        double dVal = (double)prop.GetValue(agg) + (double)prop.GetValue(transaction);
                        prop.SetValue(agg, dVal);
                    }
                }
                return agg;
            });
            total.Parameter = "TOTAL";
            total.IsTotal = true;
            total.TransactionDate = dtValuationDate;
            transactions.Add(total);
            return total.Amount;
            //return props.Where(x => x.PropertyType.Name == "Double").Sum(x => (double)x.GetValue(total)); //total.Withdrawls + total.Other + total.Purchases;
        }

        public void AddTransaction(UserAccountToken userToken, DateTime dtValuationDate, DateTime dtTransactionDate,
                                        string type, string parameter, double amount)
        {
            logger.Log(LogLevel.Info, "adding cash transaction. type: {0}, parameter: {1}, amount {2}", type, parameter, amount);
            _cashAccountData.AddCashAccountTransaction(userToken, dtValuationDate, dtTransactionDate, type,
                                                parameter, amount);
        }

        public void RemoveTransaction(UserAccountToken userToken, DateTime dtValuationDate, DateTime dtTransactionDate,
                                        string type, string parameter)
        {
            logger.Log(LogLevel.Info, "removing cash transaction. type: {0}, parameter: {1}", type, parameter);
            _cashAccountData.RemoveCashAccountTransaction(userToken, dtValuationDate, dtTransactionDate, type, parameter);    
        }

        //just compare to the third decimal place
        private static bool _Match(double d1, double d2)
        {
            return Math.Abs(Math.Round(d1,2) - Math.Round(d2,2)) <= double.Epsilon;
        }

        public bool ValidateCashAccount(UserAccountToken userToken, DateTime dtValuationDate)
        {
            logger.Log(LogLevel.Info, "validating cash account for valuation date {0}", dtValuationDate);

            var receipts = new List<ReceiptTransaction>();
            _GetTransactionsImpl<ReceiptTransaction>(userToken, dtValuationDate, ReceiptMnemomic,
                                                        receipts);
            
            var payments = new List<PaymentTransaction>();
            _GetTransactionsImpl<PaymentTransaction>(userToken, dtValuationDate, PaymentMnemomic,
                                                        payments);

            var receiptTotal = receipts.Sum(x => x.Amount);
            var paymentsTotal = payments.Sum(x => x.Amount);

            if(_Match(receiptTotal, paymentsTotal) == false )
            {
                logger.Log(LogLevel.Error, "cash account validation failed!. receipts {0}, payments {1}", receiptTotal, paymentsTotal);
                return false;
            }

            logger.Log(LogLevel.Info, "cash account validation succeded!");
            return true;
        }
    }
}
