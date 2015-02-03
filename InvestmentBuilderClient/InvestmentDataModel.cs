using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.ComponentModel;

namespace InvestmentBuilderClient
{
    abstract class Transaction
    {
        public DateTime TransactionDate { get; set; }
        //public string TransactionType { get; set; }
        public string Parameter { get; set; }
        //public double Amount { get; set; }
    }

    class ReceiptTransaction : Transaction
    {
        public double Subscription { get; set; }
        public double Sale         { get; set; }
        public double Dividend     { get; set; }
        public double Other        { get; set;}
    }

    class PaymentTransaction : Transaction
    {

    }

    //ObservableCollection
    //BindingList

    class InvestmentDataModel : IDisposable
    {
        private SqlConnection _connection;

        private IList<ReceiptTransaction> _receiptsList;

        private static Dictionary<string, Action<ReceiptTransaction, double>> _receiptTransactionLookup =
            new Dictionary<string, Action<ReceiptTransaction, double>> {
                {"Subscription", (transaction, amount) => { transaction.Subscription  = amount; }},
                {"BalanceInHand", (transaction, amount) => { transaction.Subscription  = amount; }},
                {"Sale", (transaction, amount) => { transaction.Sale  = amount; }},
                {"Dividend", (transaction, amount) => { transaction.Dividend  = amount; }},
                {"Interest", (transaction, amount) => { transaction.Other  = amount; }}
            };

        public InvestmentDataModel() 
        {
            var connectstr = @"Data Source=TRAVELPC\SQLEXPRESS;Initial Catalog=InvestmentBuilderTest;Integrated Security=True";
            _connection = new SqlConnection(connectstr);
            _receiptsList = new List<ReceiptTransaction>();
            Receipts = new BindingList<ReceiptTransaction>(_receiptsList);
            _connection.Open();
        }

        public BindingList<ReceiptTransaction> Receipts { get; private set; }

        public IEnumerable<DateTime?> GetValuationDates()
        {
            using(var command = new SqlCommand("SELECT Valuation_Date FROM Valuations ORDER BY Valuation_Date ASC", _connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader.GetDateTime(0);
                    }
                }
            }
        }

        public void GetReceipts(DateTime dtValuationDate)
        {
            Receipts.Clear();
            _GetData(dtValuationDate, "R", (reader)=>
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
        }

        private void _GetData(DateTime dtValuationDate, string side, Action<SqlDataReader> fnAddTransaction)
        {
            using (var sqlCommand = new SqlCommand("sp_GetCashAccountData", _connection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@ValuationDate", dtValuationDate));
                sqlCommand.Parameters.Add(new SqlParameter("@Side", side));
                using(var reader = sqlCommand.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        fnAddTransaction(reader);
                    }
                }
            }
        }

        public void Dispose()
        {
            _connection.Close();
        }
    }
}
