using System;
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
        private static string _NewTestTradeName = "Acme Plc";
        private static double _NewTradeTotalCost = 1284.45;

        private Stock _TestNewTrade = new Stock
            {
                Name = _NewTestTradeName,
                TransactionDate = "11/09/2015",
                Symbol = "ACME.L",
                Exchange = "LSE",
                Currency = "GBP",
                Quantity = 2500,
                TotalCost = _NewTradeTotalCost,
                ScalingFactor = 100
            };

        private static string _NewTestAccount = "TestAcc";
        private static string _NewTestUser = "bobby bob";
        private static DateTime _TransactionDate = DateTime.Parse("03/09/2015");
        private static DateTime _ValuationDate = DateTime.Parse("11/09/2015");

        private static DateTime _TransactionDateNextMonth = DateTime.Parse("05/10/2015");
        private static DateTime _ValuationDateNextMonth = DateTime.Parse("15/10/2015");

        [TestFixtureSetUp]
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
        }

        [Test]
        public void When_getting_current_investments()
        {
            Console.WriteLine("getting current investments...");

            if (m_bOk == true)
            {
                var results = ContainerManager.ResolveValue<InvestmentBuilder.InvestmentBuilder>().GetCurrentInvestments(_TestAccount, null);
                VerifyCompanyDataResults(results.ToList(), @"TestFiles\TestResultData.txt", 22);
            }
        }

        [Test]
        public void When_getting_current_investments_with_price_overrides()
        {
            Console.WriteLine("getting current investments with price override...");

            if (m_bOk == true)
            {
                var testCompany = "British Land Plc";
                double dTestValue = 894.6; 
                var prices = new ManualPrices()
                {
                    {testCompany, dTestValue }
                };

                var results = ContainerManager.ResolveValue<InvestmentBuilder.InvestmentBuilder>().GetCurrentInvestments(_TestAccount, prices);
                var resultData = results.First(x => string.Equals(x.Name, testCompany));
                Assert.IsNotNull(resultData);
                if(resultData != null)
                {
                    MatchDoubleVal(resultData.SharePrice, (dTestValue/100).ToString());
                    MatchDoubleVal(resultData.NetSellingValue, "6394.42188");
                    MatchDoubleVal(resultData.ProfitLoss, "1398.2918");
                }
            }
        }

        [Test]
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
                When_adding_a_new_account(_NewTestAccount);
                When_adding_a_new_trade(_NewTestAccount);
                When_building_asset_report(_NewTestAccount, _NewTestUser, _TransactionDate, _ValuationDate);
                When_building_asset_report_next_month(_NewTestAccount, _NewTestUser, _TransactionDateNextMonth, _ValuationDateNextMonth);
            }
        }

        private void When_adding_a_new_account(string accName)
        {
            Console.WriteLine("adding a new account...");

            var account = new AccountModel
            {
                Name = accName,
                Description = "Unit Test Account",
                Enabled = true,
                ReportingCurrency = "GBP",
                Type = "Personal",
                Password = "psst"
            };
            ContainerManager.ResolveValue<SQLServerDataLayer.SQLServerDataLayer>().ClientData.CreateAccount(account);

            ContainerManager.ResolveValue<SQLServerDataLayer.SQLServerDataLayer>().ClientData.UpdateMemberForAccount(accName,
                                                                                         _NewTestUser, true);
                                                                                         
            var result = ContainerManager.ResolveValue<SQLServerDataLayer.SQLServerDataLayer>().ClientData.GetAccount(accName);

            Assert.IsNotNull(result);

            Assert.AreEqual("Personal", result.Type);
        }
        
        private void When_adding_a_new_trade(string accName)
        {
            Console.WriteLine("adding a new trade...");

            var trades = new Trades
            {
                Sells = Enumerable.Empty<Stock>().ToArray(),
                Changed = Enumerable.Empty<Stock>().ToArray(),
                Buys = new List<Stock> { _TestNewTrade}.ToArray()
            };
            ContainerManager.ResolveValue<InvestmentBuilder.InvestmentBuilder>().UpdateTrades(accName,
                                                                                              trades, null);

            var results = ContainerManager.ResolveValue<InvestmentBuilder.InvestmentBuilder>().GetCurrentInvestments(accName, null).ToList();
            Assert.AreEqual(1, results.Count);

            var companyData = results.First();
            Assert.AreEqual(2500, companyData.Quantity);
            MatchDoubleVal(companyData.NetSellingValue, "1297.6425");
            MatchDoubleVal(companyData.ProfitLoss, "13.192");
            MatchDoubleVal(companyData.MonthChange, "0.0");
            MatchDoubleVal(companyData.MonthChangeRatio, "0.0");
        }

        private void When_building_asset_report(string account, string user, DateTime dtTransactionDate, DateTime dtValuationDate)
        {
            Console.WriteLine("building full asset report...");
            ContainerManager.ResolveValue<SQLServerDataLayer.SQLServerDataLayer>().ClientData.AddCashAccountData(
                account, dtValuationDate, dtTransactionDate, "BalanceInHand", "BalanceInHand", 2000);

            ContainerManager.ResolveValue<SQLServerDataLayer.SQLServerDataLayer>().ClientData.AddCashAccountData(
                account, dtValuationDate, dtTransactionDate, "Subscription", user, 200.0);

            ContainerManager.ResolveValue<SQLServerDataLayer.SQLServerDataLayer>().ClientData.AddCashAccountData(
               account, dtValuationDate, dtTransactionDate, "Purchase", _NewTestTradeName, _NewTradeTotalCost);

            ContainerManager.ResolveValue<SQLServerDataLayer.SQLServerDataLayer>().ClientData.AddCashAccountData(
                 account, dtValuationDate, dtTransactionDate, "BalanceInHandCF", "BalanceInHandCF", 913.05);

            ContainerManager.ResolveValue<SQLServerDataLayer.SQLServerDataLayer>().ClientData.AddCashAccountData(
                 account, dtValuationDate, dtTransactionDate, "Admin Fee", "Admin Fee", 2.50);

            var report = ContainerManager.ResolveValue<InvestmentBuilder.InvestmentBuilder>().BuildAssetReport(account,
                                                                                                  dtValuationDate,
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

        private void When_building_asset_report_next_month(string account, string user, DateTime dtTransactionDate, DateTime dtValuationDate)
        {
            Console.WriteLine("building full asset report next month...");

            ContainerManager.ResolveValue<SQLServerDataLayer.SQLServerDataLayer>().ClientData.AddCashAccountData(
                account, dtValuationDate, dtTransactionDate, "BalanceInHand", "BalanceInHand", 913.05);

            ContainerManager.ResolveValue<SQLServerDataLayer.SQLServerDataLayer>().ClientData.AddCashAccountData(
                account, dtValuationDate, dtTransactionDate, "Subscription", user, 200.0);

            ContainerManager.ResolveValue<SQLServerDataLayer.SQLServerDataLayer>().ClientData.AddCashAccountData(
                account, dtValuationDate, dtTransactionDate, "Interest", user, 0.08);

            ContainerManager.ResolveValue<SQLServerDataLayer.SQLServerDataLayer>().ClientData.AddCashAccountData(
                account, dtValuationDate, dtTransactionDate, "Admin Fee", "Admin Fee", 2.50);

            ContainerManager.ResolveValue<SQLServerDataLayer.SQLServerDataLayer>().ClientData.AddCashAccountData(
                account, dtValuationDate, dtTransactionDate, "BalanceInHandCF", "BalanceInHandCF", 1110.63);

            var manualPrices = new ManualPrices
            {
                {_TestNewTrade.Name, 49.86}
            };

            var report = ContainerManager.ResolveValue<InvestmentBuilder.InvestmentBuilder>().BuildAssetReport(account,
                                                                                      dtValuationDate,
                                                                                      true,
                                                                                      manualPrices);
            Assert.IsNotNull(report);

            var companyData = report.Assets.FirstOrDefault();
            Assert.IsNotNull(companyData);

            Assert.AreEqual(_TestNewTrade.Name, companyData.Name);
            MatchDoubleVal(companyData.MonthChange, "-63.6075");
            MatchDoubleVal(companyData.MonthChangeRatio, "-4.9017738");
            MatchDoubleVal(companyData.ProfitLoss, "-50.415");
            MatchDoubleVal(companyData.NetSellingValue, "1234.035");

            MatchDoubleVal(report.BankBalance, "1110.63");
            MatchDoubleVal(report.IssuedUnits, "2410.6925");
            MatchDoubleVal(report.MonthlyPnL, "-63.6075");
            MatchDoubleVal(report.NetAssets, "2344.665");
            MatchDoubleVal(report.TotalAssetValue, "1234.035");
            MatchDoubleVal(report.ValuePerUnit, "0.9726");
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
