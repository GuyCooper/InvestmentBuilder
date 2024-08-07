﻿using System;
using System.Collections.Generic;
using System.Linq;
using InvestmentBuilderCore;
using MarketDataServices;
using SQLServerDataLayer;
using System.IO;
using InvestmentBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PerformanceBuilderLib;
using InvestmentBuilderAuditLogger;
using InvestmentBuilderCore.Schedule;
using Unity;

namespace InvestmentBuilderMSTests
{
    internal class FunctionalTestContainer
    {
        public FunctionalTestContainer(IConfigurationSettings settings,
                                       IDataLayer dataLayer,
                                       InvestmentBuilder.InvestmentBuilder investmentBuilder,
                                       IAuthorizationManager authorizationManager,
                                       BrokerManager brokerManager,
                                       CashAccountTransactionManager cashAccountManager,
                                       PerformanceBuilder performanceBuilder,
                                       AccountManager accountManager)
        {
            ConfigSettings = settings;
            DataLayer = dataLayer;
            InvestmentBuilder = investmentBuilder;
            AuthorizationManager = authorizationManager;
            BrokerManager = brokerManager;
            CashAccountManager = cashAccountManager;
            PerformanceBuilder = performanceBuilder;
            AccountManager = accountManager;  
        }

        public IConfigurationSettings ConfigSettings { get; private set; }
        public IDataLayer DataLayer { get; private set; }
        public InvestmentBuilder.InvestmentBuilder InvestmentBuilder { get; private set; }
        public IAuthorizationManager AuthorizationManager { get; private set; }
        public BrokerManager BrokerManager { get; private set; }
        public CashAccountTransactionManager CashAccountManager { get; private set; }
        public PerformanceBuilder PerformanceBuilder { get; private set; }
        public AccountManager AccountManager { get; private set; }
    }
    ///  <summary>
    /// This class contains full acceptance tests of InvestmentBuilder
    /// suite. not unit tests. Requires a db restore. starts from an empty database
    /// </summary>
    [TestClass]
    public class FullFunctionalTests
    {
        private bool m_bOk = false;

        private static string _TestAccount = "TestAccount";
        private static string _TestUser = "TestUser";
        private static string _TestUser1 = "TestUser1";
        private static string _TestUser2 = "TestUser2";
        private static string _InvalidUser = "xxxxxx";
        private static string _TestCurrency = "GBP";
        private static string _TestValuationDate1 = "11/09/2015";
        private static string _TestValuationDate2 = "14/10/2015";
        private static string _TestTradeName = "ACME";
        private static string _TestTradeSymbol = "ACM.L";
        private readonly double _TestTradeAmount = 100;
        private readonly double _TestTradePrice = 1d;

        private readonly DateTime _dtValuationDate1 = DateTime.Parse(_TestValuationDate1);
        private readonly DateTime _dtValuationDate2 = DateTime.Parse(_TestValuationDate2);
        private readonly DateTime _dtTransactionDate1 = DateTime.Parse("06/09/2015");
        private readonly double _testSubscription1 = 50.0d;

        private FunctionalTestContainer _interfaces;
        private IUnityContainer _childContainer;

        [TestInitialize]
        public void Setup()
        {
            _childContainer = ContainerManager.CreateChildContainer();

            //first drop the unit testdatabase andrefresh it from the unit testbackup

            ContainerManager.RegisterType(typeof(IConfigurationSettings), typeof(ConfigurationSettings), true, @"TestFiles\UnitTestBuilderConfig1.xml");

            m_bOk = InitialiseDatabase();
            if (m_bOk == false)
            {
                Console.WriteLine("failed to initialise unit test database");
                return;
            }

            Console.WriteLine("database refresh succeded...");
            ContainerManager.RegisterType(typeof(IAuthorizationManager), typeof(SQLAuthorizationManager), true);
            ContainerManager.RegisterType(typeof(IMarketDataService), typeof(MarketDataService), true);
            MarketDataRegisterService.RegisterServices();
            //todo,use servicelocator
            ContainerManager.RegisterType(typeof(IDataLayer), typeof(SQLServerDataLayer.SQLServerDataLayer));
            ContainerManager.RegisterType(typeof(IInvestmentRecordDataManager), typeof(InvestmentRecordBuilder));
            ContainerManager.RegisterType(typeof(FunctionalTestContainer), typeof(FunctionalTestContainer));
            ContainerManager.RegisterType(typeof(IInvestmentReportWriter), typeof(InvestmentReportGenerator.InvestmentReportWriter));
            ContainerManager.RegisterType(typeof(IMessageLogger), typeof(FileMessageLogger));
            ContainerManager.RegisterType(typeof(CashAccountTransactionManager));
            ContainerManager.RegisterType(typeof(ScheduledTaskFactory));

            _interfaces = ContainerManager.ResolveValueOnContainer<FunctionalTestContainer>(_childContainer);
        }

        private bool InitialiseDatabase()
        {
            var ret = ProcessLauncher.RunProcessAndWaitForCompletion(@"C:\Projects\InvestmentBuilder\scripts\GenerateUnitTestDatabase.bat", "");
            Console.WriteLine("return code from initialisedatabase : {0}", ret);
            return true;
        }

        [TestCleanup]
        public void Cleanup()
        {
            _childContainer.Dispose();
        }

        [TestMethod]
        public void RunFunctionalTests()
        {
            if(!m_bOk)
            {
                return;
            }

            Console.WriteLine("run functional tests...");

            if (m_bOk == true)
            {
                AddUsers();
                Console.WriteLine("setup successful");
                When_getting_users();


                var token = When_adding_a_new_account(_TestAccount);

                //now remove any generated files from previous tests
                var outfolder = _interfaces.ConfigSettings.GetOutputPath(token.Account.GetPathName());
                var files = Directory.EnumerateFiles(outfolder);
                foreach (var file in files)
                {
                    File.Delete(file);
                }

                //now copy the templates file into the output folder templates folder
                //var templatesFolder = Path.Combine(_interfaces.ConfigSettings.OutputFolder, "Templates");
                //Directory.CreateDirectory(templatesFolder);
                //File.Copy(@"..\..\..\Templates\template.xls", Path.Combine(templatesFolder,"template.xls"), true);

                When_adding_members_to_account(token);
                When_adding_subscription_amounts(token);
                When_generating_first_Asset_report(token);
                When_adding_an_investment_1(token);
            }
        }

        private void AddUsers()
        {
            Console.WriteLine($"adding users  to database");
            _interfaces.DataLayer.UserAccountData.AddUser(_TestUser, "test user");
            _interfaces.DataLayer.UserAccountData.AddUser(_TestUser1, "test user 1");
            _interfaces.DataLayer.UserAccountData.AddUser(_TestUser2, "test user 2");
        }

        private void When_getting_users()
        {
            Console.WriteLine("Validating users...");
            Assert.IsTrue(_interfaces.DataLayer.UserAccountData.GetUserId(_TestUser) > 0);
            Assert.IsTrue(_interfaces.DataLayer.UserAccountData.GetUserId(_TestUser1) > 0);
            Assert.IsTrue(_interfaces.DataLayer.UserAccountData.GetUserId(_TestUser2) > 0);
            Assert.IsTrue(_interfaces.DataLayer.UserAccountData.GetUserId(_InvalidUser) == -1);
        }

        private UserAccountToken When_adding_a_new_account(string accountName)
        {
            Console.WriteLine("When_adding_a_new_account");
            var member = new AccountMember(_TestUser, AuthorizationLevel.ADMINISTRATOR);
            var members = new List<AccountMember> { member };

            var accountIdentifer = new AccountIdentifier
            {
                Name = accountName
            };

            var account = new AccountModel(accountIdentifer, accountName, _TestCurrency, "Club", true, "ShareCentre", members);

            bool added =_interfaces.AccountManager.CreateUserAccount(_TestUser, account, _dtValuationDate1);
            Assert.IsTrue(added);

            var token = _interfaces.AuthorizationManager.GetUserAccountToken(_TestUser, accountIdentifer);
            var result = _interfaces.AccountManager.GetAccountData(token, _dtValuationDate1);
            Assert.AreEqual(accountName, result.Identifier.Name);
            Assert.AreEqual(accountIdentifer.AccountId, result.Identifier.AccountId);
            Assert.AreEqual(1, result.Members.Count);

            return token;
        }

        private void When_adding_members_to_account(UserAccountToken userToken)
        {
            Console.WriteLine("When_adding_members_to_account");
            var account = _interfaces.AccountManager.GetAccountData(userToken, _dtValuationDate1);
            account.AddMember(_TestUser1, AuthorizationLevel.UPDATE);
            account.AddMember(_TestUser2, AuthorizationLevel.READ);
            bool updated = _interfaces.AccountManager.UpdateUserAccount(userToken.User, account, _dtValuationDate1);
            Assert.IsTrue(updated);
            var result = _interfaces.AccountManager.GetAccountData(userToken, _dtValuationDate1);
            Assert.AreEqual(userToken.Account.Name, result.Identifier.Name);
            Assert.AreEqual(userToken.Account.AccountId, result.Identifier.AccountId);
            Assert.AreEqual(3, result.Members.Count);
        }

        private void When_adding_subscription_amounts(UserAccountToken userToken)
        {
            Console.WriteLine("When_adding_subscription_amounts");
            _interfaces.CashAccountManager.AddTransaction(userToken, _dtValuationDate1, _dtTransactionDate1, TransactionTypes.SUBSCRIPTION, _TestUser, _testSubscription1, null);
            _interfaces.CashAccountManager.AddTransaction(userToken, _dtValuationDate1, _dtTransactionDate1, TransactionTypes.SUBSCRIPTION, _TestUser1, _testSubscription1 , null);
            _interfaces.CashAccountManager.AddTransaction(userToken, _dtValuationDate1, _dtTransactionDate1, TransactionTypes.SUBSCRIPTION, _TestUser2, _testSubscription1, null);
            _interfaces.CashAccountManager.AddTransaction(userToken, _dtValuationDate1, _dtTransactionDate1, TransactionTypes.BALANCEINHANDCF, _TestAccount, _testSubscription1 * 3, null);

            double receiptTotal;
            double paymentsTotal;
            var receipts = _interfaces.CashAccountManager.GetReceiptTransactions(userToken, _dtValuationDate1, null, out receiptTotal);
            var payments = _interfaces.CashAccountManager.GetPaymentTransactions(userToken, _dtValuationDate1, out paymentsTotal);

            Assert.AreEqual(4, receipts.Count);
            Assert.AreEqual(2, payments.Count);
            Assert.AreEqual(receiptTotal, paymentsTotal);

            bool validated = _interfaces.CashAccountManager.ValidateCashAccount(userToken, _dtValuationDate1);
            Assert.IsTrue(validated);
        }

        private void When_generating_first_Asset_report(UserAccountToken userToken)
        {
            Console.WriteLine("When_generating_first_Asset_report");
            var assetReport = _interfaces.InvestmentBuilder.BuildAssetReport(userToken, _dtValuationDate1, true, null, null);
            Assert.IsNotNull(assetReport);
            Assert.AreEqual(_TestAccount, assetReport.AccountName.Name);
            Assert.AreEqual(0, assetReport.Assets.Count());
            Assert.AreEqual(150d, assetReport.BankBalance);
            Assert.AreEqual(150d, assetReport.IssuedUnits);
            Assert.AreEqual(0d, assetReport.MonthlyPnL);
            Assert.AreEqual(150d, assetReport.NetAssets);
            Assert.AreEqual(150d, assetReport.TotalAssets);
            Assert.AreEqual(0, assetReport.TotalAssetValue);
            Assert.AreEqual(1d, assetReport.ValuePerUnit);
        }

        private void When_adding_an_investment_1(UserAccountToken userToken)
        {
            Console.WriteLine("When_adding_an_investment_1");
            var tradesList = new Trades();
            tradesList.Buys = new Stock[] {new Stock {
                 Name = _TestTradeName,
                 Symbol = _TestTradeSymbol,
                 Currency = _TestCurrency,
                 Quantity = _TestTradeAmount,
                 TotalCost =  _TestTradeAmount * _TestTradePrice,
                 TransactionDate = _dtValuationDate2
            } };

            var updated = _interfaces.InvestmentBuilder.UpdateTrades(userToken, tradesList, null, null, _dtValuationDate2);
            Assert.IsTrue(updated);

            var result = _interfaces.InvestmentBuilder.GetCurrentInvestments(userToken, null).ToList();
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(_TestTradeName, result[0].Name);
            CompareDoubles(1d, result[0].AveragePricePaid);
            CompareDoubles(0d, result[0].MonthChange);
            CompareDoubles(102.5d, result[0].NetSellingValue);
            CompareDoubles(2.5d, result[0].ProfitLoss);
            CompareDoubles(1.1, result[0].SharePrice);
            CompareDoubles(100d, result[0].TotalCost);
        }

        void CompareDoubles(double v1, double v2)
        {
            //match to 5dps
            var s1 = v1.ToString("#0.00000");
            var s2 = v2.ToString("#0.00000");
            Assert.AreEqual(s1, s2);
        }
    }
}
