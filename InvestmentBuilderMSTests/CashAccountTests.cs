using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InvestmentBuilder;
using InvestmentBuilderCore;

namespace InvestmentBuilderMSTests
{
    internal static class CashAccountTestData
    {
        public static int _TransactionID = 1234;
        public static DateTime _transactionDate = DateTime.Parse("12/12/2015");
        public static string _PaymentParameter = "Purchase";
        public static string _PaymentTransactionType = "Purchase";
        public static double _PaymentAmount = 925.65d;
        public static string _ReceiptParameter = "Subscription";
        public static string _ReceiptTransactionType = "Subscription";
        public static double _ReceiptAmount = 300d;
        public static double _BalanceInHand = 1254.76d;
    }

    internal class CashAccountTransactionDataTest : CashAccountInterfaceTest
    {
        public override void GetCashAccountTransactions(UserAccountToken userToken, string side, DateTime valuationDate, Action<System.Data.IDataReader> fnAddTransaction)
        {
            var reader = new TestDataReader();

            if (side == "P")
            {
                reader.AddEntry("TransactionID", CashAccountTestData._TransactionID); 
                reader.AddEntry("TransactionDate", CashAccountTestData._transactionDate);
                reader.AddEntry("Parameter", CashAccountTestData._PaymentParameter);
                reader.AddEntry("TransactionType", CashAccountTestData._PaymentTransactionType);
                reader.AddEntry("Amount", CashAccountTestData._PaymentAmount);
            }
            else
            {
                reader.AddEntry("TransactionID", CashAccountTestData._TransactionID);
                reader.AddEntry("TransactionDate", CashAccountTestData._transactionDate);
                reader.AddEntry("Parameter", CashAccountTestData._ReceiptParameter);
                reader.AddEntry("TransactionType", CashAccountTestData._ReceiptTransactionType);
                reader.AddEntry("Amount", CashAccountTestData._ReceiptAmount);
            }
            fnAddTransaction(reader);
        }

        public override double GetBalanceInHand(UserAccountToken userToken, DateTime valuationDate)
        {
            return CashAccountTestData._BalanceInHand;
        }

        public override int AddCashAccountTransaction(UserAccountToken userToken, DateTime valuationDate, DateTime transactionDate, string type, string parameter, double amount)
        {
            return 0;
        }
    }

    public class CashAccountTestsBase
    {
        protected static readonly UserAccountToken _usertoken = new UserAccountToken("testUser", new AccountIdentifier { Name = "testAccount", AccountId = 2 }, AuthorizationLevel.ADMINISTRATOR);
        protected static readonly DateTime _dtValuation = DateTime.Parse("10/12/2015");

        protected CashAccountTransactionManager _manager;
    }

    [TestClass]
    public class CashAccountTests : CashAccountTestsBase
    {
        [TestInitialize]
        public void Setup()
        {
            var datalayer = new DataLayerTest(null
                                              , null
                                              , new CashAccountTransactionDataTest()
                                              , null
                                              , null);

            _manager = new CashAccountTransactionManager(datalayer, null);
        }

        [TestMethod]
        public void When_getting_payment_transactions()
        {
            double dTotal;
            var transactions = _manager.GetPaymentTransactions(_usertoken, _dtValuation, out dTotal).ToList();

            Assert.AreEqual(2, transactions.Count);
            Assert.AreEqual(CashAccountTestData._PaymentAmount, transactions[0].Amount);
            Assert.AreEqual(CashAccountTestData._PaymentAmount, transactions[0].Purchases);
            Assert.AreEqual(CashAccountTestData._PaymentAmount, transactions[1].Amount);
            Assert.AreEqual(CashAccountTestData._PaymentAmount, transactions[1].Purchases);
            Assert.AreEqual(CashAccountTestData._PaymentAmount, dTotal);
        }

        [TestMethod]
        public void When_getting_receipt_transactions()
        {
            double dTotal;
            var transactions = _manager.GetReceiptTransactions(_usertoken, _dtValuation, DateTime.Parse("14/11/2015"), out dTotal).ToList();

            Assert.AreEqual(3, transactions.Count);
            Assert.AreEqual(CashAccountTestData._ReceiptAmount, transactions[0].Amount);
            Assert.AreEqual(CashAccountTestData._ReceiptAmount, transactions[0].Subscription);
            Assert.AreEqual(CashAccountTestData._BalanceInHand, transactions[1].Amount);
            Assert.AreEqual(CashAccountTestData._BalanceInHand, transactions[1].Subscription);
            Assert.AreEqual(CashAccountTestData._ReceiptAmount + CashAccountTestData._BalanceInHand, transactions[2].Amount);
            Assert.AreEqual(CashAccountTestData._ReceiptAmount + CashAccountTestData._BalanceInHand, transactions[2].Subscription);
            Assert.AreEqual(CashAccountTestData._ReceiptAmount + CashAccountTestData._BalanceInHand, dTotal);
        }

        [TestMethod]
        public void When_validating_cash_account()
        {
            var ret = _manager.ValidateCashAccount(_usertoken, _dtValuation);
            Assert.IsFalse(ret);
        }
    }

    [TestClass]
    public class CashAccountEmptyTests : CashAccountTestsBase
    {
        [TestInitialize]
        public void Setup()
        {
            var datalayer = new DataLayerTest(null
                                                     , null
                                                     , new CashAccountEmptyInterfaceTest()
                                                     , null
                                                     , null);

            _manager = new CashAccountTransactionManager(datalayer, null);
        }

        [TestMethod]
        public void When_getting_empty_payment_transactions()
        {
            double dTotal;
            var transactions = _manager.GetPaymentTransactions(_usertoken, _dtValuation, out dTotal).ToList();

            Assert.AreEqual(1, transactions.Count);
            Assert.AreEqual(0d, dTotal);

            ValidateTotalTransaction(transactions[0]);
        }

        [TestMethod]
        public void When_getting_empty_receipt_transactions()
        {
            double dTotal;
            var transactions = _manager.GetReceiptTransactions(_usertoken, _dtValuation, DateTime.Parse("14/11/2015"), out dTotal).ToList();
            Assert.AreEqual(2, transactions.Count);
            Assert.AreEqual(0d, dTotal);
       
            var transaction = transactions[0];
            Assert.AreEqual("BalanceInHand", transaction.Parameter);
            Assert.AreEqual(_dtValuation.ToString("yyyy-MM-dd"), transaction.TransactionDate);
            Assert.AreEqual(0d, transaction.Amount);
            Assert.IsFalse(transaction.IsTotal);

            ValidateTotalTransaction(transactions[1]);
        }

        private void ValidateTotalTransaction(InvestmentBuilder.CashTransaction transaction)
        {
            Assert.AreEqual("TOTAL", transaction.Parameter);
            Assert.AreEqual(_dtValuation.ToString("yyyy-MM-dd"), transaction.TransactionDate);
            Assert.AreEqual(0d, transaction.Amount);
            Assert.IsTrue(transaction.IsTotal);
        }
    }
}