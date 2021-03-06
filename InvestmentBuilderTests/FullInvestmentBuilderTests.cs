﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using InvestmentBuilderCore;
using MarketDataServices;
using SQLServerDataLayer;
using System.IO;
using InvestmentBuilder;

namespace InvestmentBuilderTests
{
    [TestFixture]
    public class FullInvestmentBuilderTests
    {
        private bool m_bOk = false;

        private static string _TestAccount = "Guy SIPP";
        private static string _TestUser = "TestUser";
        private static string _NewTestTradeName = "Acme Plc";
        private static double _NewTradeTotalCost = 1284.45;

        private UserAccountToken _userToken = new UserAccountToken(
                                                    _TestUser,
                                                    _TestAccount,
                                                    AuthorizationLevel.ADMINISTRATOR);

        private static string _strValuationDate = "11/09/2015";
        private static DateTime _TradeValuationDate = DateTime.Parse("03/09/2015 13:54:23");
        private static DateTime _TransactionDate = DateTime.Parse("03/09/2015");
        private static DateTime _ValuationDate = DateTime.Parse(_strValuationDate);

        private Stock _TestNewTrade = new Stock
            {
                Name = _NewTestTradeName,
                TransactionDate = DateTime.Parse("01/09/2015"),
                Symbol = "ACME.L",
                Exchange = "LSE",
                Currency = "GBP",
                Quantity = 2500,
                TotalCost = _NewTradeTotalCost,
                ScalingFactor = 100
            };

        private static string _NewTestAccount = "TestAcc";
        private static string _NewTestUser = "bobby bob";

        private UserAccountToken _newTestToken = new UserAccountToken(
            _NewTestUser, _NewTestAccount, AuthorizationLevel.ADMINISTRATOR);

        private static DateTime _TransactionDateNextMonth = DateTime.Parse("05/10/2015");
        private static DateTime _ValuationDateNextMonth = DateTime.Parse("15/10/2015");
        private static DateTime _TradeValuationDateNextMonth = DateTime.Parse("05/10/2015 18:43:45");

        //[OneTimeSetUp]
        public void Setup()
        { 
            //first drop the unit testdatabase andrefresh it from the unit testbackup

            ContainerManager.RegisterType(typeof(IConfigurationSettings), typeof(ConfigurationSettings), false, @"TestFiles\UnitTestBuilderConfig.xml");

            var dbParams = ContainerManager.ResolveValue<IConfigurationSettings>().DatasourceString.Split(';').Select(x =>
                {
                    int i = x.IndexOf('=');
                    if(i > -1)
                    {
                        return new KeyValuePair<string,string>(x.Substring(0, i), x.Substring(i+1));
                    }
                    return new KeyValuePair<string,string>(x, string.Empty);
                });
         
            var server = dbParams.First(x => string.Equals(x.Key, "Data Source",StringComparison.InvariantCultureIgnoreCase)).Value;
            var database = dbParams.First(x => string.Equals(x.Key, "Initial Catalog",StringComparison.InvariantCultureIgnoreCase)).Value;
            
            Console.WriteLine("restoring unit test database against server {0}, database {1}...", server, database);
            var process = new System.Diagnostics.Process();
            //"sqlcmd -S %ServerName% -E -d %DBName% -i sp_AddNewShares.sql"
            process.StartInfo.FileName = "sqlcmd.exe";
            process.StartInfo.Arguments = string.Format(@" -S {0} -E -d master -i TestFiles\RestoreTestDatabase.sql", server);
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.ErrorDialog = true;
            process.StartInfo.UseShellExecute = true;
            process.Start();
            process.WaitForExit();
            var result = process.ExitCode;

            m_bOk = result == 0;

            if (m_bOk == false)
                return;

            //todo actupon result;

            Console.WriteLine("database refresh succeded...");

            ContainerManager.RegisterType(typeof(IMarketDataSource), typeof(TestFileMarketDataSource), false, @"TestFiles\UnitTestMarketData.txt");
            ContainerManager.RegisterType(typeof(IMarketDataService), typeof(MarketDataService), false);
            
            //todo,use servicelocator
            ContainerManager.RegisterType(typeof(IDataLayer), typeof(SQLServerDataLayer.SQLServerDataLayer), false);
            ContainerManager.RegisterType(typeof(InvestmentBuilder.InvestmentBuilder), typeof(InvestmentBuilder.InvestmentBuilder), false);

            ContainerManager.RegisterType(typeof(IAuthorizationManager), typeof(SQLAuthorizationManager), false);

            ContainerManager.RegisterType(typeof(InvestmentBuilder.BrokerManager), typeof(InvestmentBuilder.BrokerManager), false);

            ContainerManager.RegisterType(typeof(InvestmentBuilder.CashAccountTransactionManager), typeof(InvestmentBuilder.CashAccountTransactionManager), false);
        }

        //[Test]
        public void RunFullTests()
        {
            Console.WriteLine("run full tests...");

            //first remove any generated files from previous tests

            var outfolder = ContainerManager.ResolveValue<IConfigurationSettings>().GetOutputPath(_NewTestAccount);

            var files = Directory.EnumerateFiles(outfolder);
            foreach (var file in files)
            {
                File.Delete(file);
            }

            if (m_bOk == true)
            {
                When_adding_a_new_account(_newTestToken);
                When_adding_a_new_trade(_newTestToken);
                When_undoing_last_transaction(_newTestToken);
                When_building_asset_report(_newTestToken, _TransactionDate, _ValuationDate);
                When_ViewingTransactions(_newTestToken, _ValuationDate);
                When_ViewingTransactionsNextMonth(_newTestToken, _ValuationDateNextMonth, _ValuationDate);
                When_Redeeming_units_too_much(_newTestToken, _NewTestUser, _TransactionDateNextMonth);
                When_Redeeming_units(_newTestToken, _NewTestUser, _TransactionDateNextMonth);
                When_building_asset_report_next_month(_newTestToken, _TransactionDateNextMonth, _ValuationDateNextMonth);
                When_building_asset_report_next_month_repeat(_newTestToken, _TransactionDateNextMonth, _ValuationDateNextMonth);
            }
        }

        private void When_adding_a_new_account(UserAccountToken userToken)
        {
            Console.WriteLine("adding a new account...");

            var account = new AccountModel
            {
                Name = userToken.Account,
                Description = "Unit Test Account",
                Enabled = true,
                ReportingCurrency = "GBP",
                Type = "Personal",
                Password = "psst",
                Broker = "ShareCentre"
            };
            ContainerManager.ResolveValue<SQLServerDataLayer.SQLServerDataLayer>().ClientData.CreateAccount(userToken, account);

            ContainerManager.ResolveValue<SQLServerDataLayer.SQLServerDataLayer>().ClientData.UpdateMemberForAccount(userToken,
                                                                                         userToken.User, AuthorizationLevel.UPDATE, true);
                                                                                         
            var result = ContainerManager.ResolveValue<SQLServerDataLayer.SQLServerDataLayer>().ClientData.GetAccount(userToken);

            Assert.IsNotNull(result);

            Assert.AreEqual("Personal", result.Type);
            Assert.AreEqual("ShareCentre", result.Broker);
        }
 
        private Trades toTradeBuys(Stock stock)
        {
            return new Trades
            {
                Sells = Enumerable.Empty<Stock>().ToArray(),
                Changed = Enumerable.Empty<Stock>().ToArray(),
                Buys = new List<Stock> { stock }.ToArray()
            };
  
        }

        private void VerifyTradeTransactionResults(UserAccountToken userToken)
        {
            var results = ContainerManager.ResolveValue<InvestmentBuilder.InvestmentBuilder>().GetCurrentInvestments(userToken, null).ToList();
            Assert.AreEqual(1, results.Count);

            var companyData = results.First();
            Assert.AreEqual(2500, companyData.Quantity);
            MatchDoubleVal(companyData.NetSellingValue, "1297.6425");
            MatchDoubleVal(companyData.ProfitLoss, "13.192");
            MatchDoubleVal(companyData.MonthChange, "0.0");
            MatchDoubleVal(companyData.MonthChangeRatio, "0.0");
        }

        private void When_adding_a_new_trade(UserAccountToken userToken)
        {
            Console.WriteLine("adding a new trade...");

            var trades = toTradeBuys(_TestNewTrade);

            ContainerManager.ResolveValue<InvestmentBuilder.InvestmentBuilder>().UpdateTrades(userToken,
                                                                                              trades, null, _TradeValuationDate);
            VerifyTradeTransactionResults(userToken);
        }

        private void When_undoing_last_transaction(UserAccountToken userToken)
        {
            Console.WriteLine("undoing last transaction...");

            _TestNewTrade.Quantity = 500;
            _TestNewTrade.TotalCost = 234.43d;
            var trades = toTradeBuys(_TestNewTrade);
            ContainerManager.ResolveValue<InvestmentBuilder.InvestmentBuilder>().UpdateTrades(userToken,
                                                            trades, null, _TradeValuationDate.AddSeconds(120));

            var results = ContainerManager.ResolveValue<InvestmentBuilder.InvestmentBuilder>().GetCurrentInvestments(userToken, null).ToList();
            Assert.AreEqual(1, results.Count);
            var companyData = results.First();
            Assert.AreEqual(3000, companyData.Quantity);

            var dataLayer = ContainerManager.ResolveValue<IDataLayer>();
            dataLayer.ClientData.UndoLastTransaction(userToken);

            VerifyTradeTransactionResults(userToken);
        }

        private void When_building_asset_report(UserAccountToken userToken, DateTime dtTransactionDate, DateTime dtValuationDate)
        {
            Console.WriteLine("building full asset report...");
            ICashAccountInterface cashAccountData = ContainerManager.ResolveValue<IDataLayer>().CashAccountData;
            cashAccountData.AddCashAccountTransaction(
                userToken, dtValuationDate, dtTransactionDate, "BalanceInHand", "BalanceInHand", 2000);

            cashAccountData.AddCashAccountTransaction(
                userToken, dtValuationDate, dtTransactionDate, "Subscription", userToken.User, 200.0);

            cashAccountData.AddCashAccountTransaction(
               userToken, dtValuationDate, dtTransactionDate, "Purchase", _NewTestTradeName, _NewTradeTotalCost);

            cashAccountData.AddCashAccountTransaction(
                 userToken, dtValuationDate, dtTransactionDate, "BalanceInHandCF", "BalanceInHandCF", 913.05);

            cashAccountData.AddCashAccountTransaction(
                 userToken, dtValuationDate, dtTransactionDate, "Admin Fee", "Admin Fee", 2.50);

            var report = ContainerManager.ResolveValue<InvestmentBuilder.InvestmentBuilder>().BuildAssetReport(userToken,
                                                                                                  dtValuationDate.AddHours(14),
                                                                                                  true,
                                                                                                  null);
            Assert.IsNotNull(report);

            var company = report.Assets.FirstOrDefault(x => x.Name == _TestNewTrade.Name);
            Assert.IsNotNull(company);
            MatchDoubleVal(company.MonthChange, "0.0");
            MatchDoubleVal(company.NetSellingValue, "1297.6425");
            MatchDoubleVal(company.ProfitLoss, "13.1924");
            MatchDoubleVal(company.SharePrice, "0.5243");
            MatchDoubleVal(company.TotalCost, "1284.45");

            MatchDoubleVal(report.BankBalance, "913.05");
            MatchDoubleVal(report.IssuedUnits, "2210.6925");
            MatchDoubleVal(report.MonthlyPnL, "0.0");
            MatchDoubleVal(report.TotalAssetValue, "1297.6425");
            MatchDoubleVal(report.ValuePerUnit, "1.0");
            MatchDoubleVal(report.YearToDatePerformance, "0.0");
        }

        private void When_ViewingTransactions(UserAccountToken userToken, DateTime dtValuation)
        {
            double total, total1;
            var payments = ContainerManager.ResolveValue<InvestmentBuilder.CashAccountTransactionManager>().GetPaymentTransactions(
                userToken, dtValuation, out total);

            Assert.AreEqual(total, 1284.45d + 913.05d + 2.5d);
            Assert.AreEqual(4, payments.Count);

            var receipts = ContainerManager.ResolveValue<InvestmentBuilder.CashAccountTransactionManager>().GetReceiptTransactions(
                userToken, dtValuation, dtValuation, out total1);

            Assert.AreEqual(total, total1);
            Assert.AreEqual(3, receipts.Count); //balance in hand, subscription,total
        }

        private void When_ViewingTransactionsNextMonth(UserAccountToken userToken, DateTime dtValuation, DateTime dtCurrentDate)
        {
            double total;
            var receipts = ContainerManager.ResolveValue<InvestmentBuilder.CashAccountTransactionManager>().GetReceiptTransactions(
                userToken, dtValuation, dtCurrentDate, out total);

            Assert.AreEqual(total, 913.05d);
            Assert.AreEqual(2, receipts.Count);
 
        }

        private void _validate_report_next_month(AssetReport report)
        {
            Assert.IsNotNull(report);

            var companyData = report.Assets.FirstOrDefault();
            Assert.IsNotNull(companyData);

            Assert.AreEqual(_TestNewTrade.Name, companyData.Name);
            MatchDoubleVal(companyData.MonthChange, "-78.1204");
            MatchDoubleVal(companyData.MonthChangeRatio, "-6.020");
            MatchDoubleVal(companyData.ProfitLoss, "-64.9280");
            MatchDoubleVal(companyData.NetSellingValue, "1480.8420");

            MatchDoubleVal(report.BankBalance, "349.31");
            MatchDoubleVal(report.IssuedUnits, "1893.4");
            MatchDoubleVal(report.MonthlyPnL, "-78.120");
            MatchDoubleVal(report.NetAssets, "1830.152");
            MatchDoubleVal(report.TotalAssetValue, "1480.842");
            MatchDoubleVal(report.ValuePerUnit, "0.966");

        }

        private void When_building_asset_report_next_month(UserAccountToken userToken, DateTime dtTransactionDate, DateTime dtValuationDate)
        {
            Console.WriteLine("building full asset report next month...");

            const double dUpdateTradeCost = 261.32d;

            //first update the new trade to test month change value is correct
            _TestNewTrade.TransactionDate = DateTime.Parse("21/09/2015");
            _TestNewTrade.Quantity = 500;
            _TestNewTrade.TotalCost = dUpdateTradeCost;
            var trades = new Trades
            {
                Sells = Enumerable.Empty<Stock>().ToArray(),
                Changed = Enumerable.Empty<Stock>().ToArray(),
                Buys = new List<Stock> { _TestNewTrade }.ToArray()
            };

            ContainerManager.ResolveValue<InvestmentBuilder.InvestmentBuilder>().UpdateTrades(userToken,
                                                                                              trades, null, _TradeValuationDateNextMonth);

            //ContainerManager.ResolveValue<SQLServerDataLayer.SQLServerDataLayer>().ClientData.AddCashAccountData(
            //    userToken, dtValuationDate, dtTransactionDate, "BalanceInHand", "BalanceInHand", 913.05);

            ContainerManager.ResolveValue<CashAccountTransactionManager>().AddTransaction(
                userToken, dtValuationDate, dtTransactionDate, "Subscription", userToken.User, 200.0);

            ContainerManager.ResolveValue<CashAccountTransactionManager>().AddTransaction(
                userToken, dtValuationDate, dtTransactionDate, "Interest", userToken.User, 0.08);

            ContainerManager.ResolveValue<CashAccountTransactionManager>().AddTransaction(
                userToken, dtValuationDate, dtTransactionDate, "Admin Fee", "Admin Fee", 2.50);

            ContainerManager.ResolveValue<CashAccountTransactionManager>().AddTransaction(
               userToken, dtValuationDate, dtTransactionDate, "Purchase", _NewTestTradeName, dUpdateTradeCost);

            ContainerManager.ResolveValue<CashAccountTransactionManager>().AddTransaction(
                userToken, dtValuationDate, dtTransactionDate, "BalanceInHandCF", "BalanceInHandCF", 849.31);

            var manualPrices = new ManualPrices
            {
                {_TestNewTrade.Name, 0.4986}
            };

            var report = ContainerManager.ResolveValue<InvestmentBuilder.InvestmentBuilder>().BuildAssetReport(userToken,
                                                                                      dtValuationDate.AddHours(13),
                                                                                      true,
                                                                                      manualPrices);

            _validate_report_next_month(report);
        }

        private void When_building_asset_report_next_month_repeat(UserAccountToken userToken, DateTime dtTransactionDate, DateTime dtValuationDate)
        {
            Console.WriteLine("building full asset report next month repeat...");

            var manualPrices = new ManualPrices
            {
                {_TestNewTrade.Name, 0.4986}
            };

            var report = ContainerManager.ResolveValue<InvestmentBuilder.InvestmentBuilder>().BuildAssetReport(userToken,
                                                                                      dtValuationDate.AddHours(16),
                                                                                      true,
                                                                                      manualPrices);
            _validate_report_next_month(report);
        }

        private void When_Redeeming_units_too_much(UserAccountToken userToken, string user, DateTime dtValuationDate)
        {
            //sell 1000 units
            Console.WriteLine("When_Redeeming_units_too_much");

            var result = ContainerManager.ResolveValue<InvestmentBuilder.InvestmentBuilder>().RequestRedemption(
                userToken, user, 1000d, dtValuationDate);

            Assert.IsFalse(result);
        }

        private void When_Redeeming_units(UserAccountToken userToken, string user, DateTime dtValuationDate)
        {
            //sell 500 units
            Console.WriteLine("When_Redeeming_units");
            var result = ContainerManager.ResolveValue<InvestmentBuilder.InvestmentBuilder>().RequestRedemption(
                userToken, user, 500d, dtValuationDate);

            Assert.IsTrue(result);
        }

        private void VerifyCompanyDataResults(IList<CompanyData> results, string resultFile, int count )
        {
            List<string> listTestData = new List<string>();
            using(var reader = new StreamReader(resultFile))
            {
                string line;
                while((line = reader.ReadLine()) != null)
                {
                    listTestData.Add(line);
                }
            }

            Assert.AreEqual(count, results.Count);
            bool matchingSize = listTestData.Count == results.Count;
            Assert.IsTrue(matchingSize);
            if(matchingSize == true)
            {
                for(int i = 0; i < listTestData.Count; ++i)
                {
                    var testElems = listTestData[i].Split('\t');
                    var company = results[i];
                    Assert.AreEqual(testElems[0], company.Name);
                    Assert.AreEqual(testElems[1], company.Quantity.ToString());
                    MatchDoubleVal(company.SharePrice, testElems[3]);
                    MatchDoubleVal(company.NetSellingValue, testElems[4]);
                    MatchDoubleVal(company.ProfitLoss, testElems[5]);
                }
            }
        }

        private void MatchDoubleVal(double dVal, string match)
        {
            //just match integer part
            var d = dVal >= 0 ? Math.Floor(dVal).ToString() : Math.Ceiling(dVal).ToString();
            var s = match.Substring(0, match.IndexOf('.'));
            Assert.AreEqual(d, s);
        }
    }
}
