using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InvestmentBuilderCore;
using InvestmentBuilder;

namespace InvestmentBuilderMSTests
{
    internal static class TestDataCache
    {
        public static string _Name = "TestAccount";
        public static string _Currency = "GBP";
        public static string _Description = "Test account";
        public static string _Broker = "AJBell";

        public static DateTime _previousValutionDate = DateTime.Parse("23/10/2015");
        public static DateTime _currentValuationDate = DateTime.Parse("25/11/2015");
        public static DateTime _previousRecordValutionDate = DateTime.Parse("23/10/2015 10:32:56");

        public static string _TestCompany = "Acme PLC";
        public static int _TestQuantity = 72;
        public static double _TestTotalCost = 1082.73;
        public static double _TestDividend = 95.24;

        public static string _TestSymbol = "ACME.L";
        public static string _TestCurrency = "GBP";
        public static double _TestScalingFactor = 100;
        public static double _TestSharePrice = 25.98;
    }
    internal class CurrentInvestmentsUserAccountData : UserAccountInterfaceTest
    {
        public override UserAccountData GetUserAccountData(UserAccountToken userToken)
        {
            return new UserAccountData
            {
                Name = TestDataCache._Name,
                Currency = TestDataCache._Currency,
                Description = TestDataCache._Description,
                Broker = TestDataCache._Broker
            };
       }
    }

    internal class CurrentInvestmentsClientData : ClientDataInterfaceTest
    {
        public override DateTime? GetPreviousAccountValuationDate(UserAccountToken userToken, DateTime dtValuation)
        {
            return TestDataCache._previousValutionDate;
        }
    }

    internal class CurrentInvestmentsRecordData : InvestmentRecordInterfaceTest
    {
        public override DateTime? GetLatestRecordInvestmentValuationDate(UserAccountToken userToken)
        {
            return TestDataCache._currentValuationDate;
        }

        public override DateTime? GetPreviousRecordInvestmentValuationDate(UserAccountToken userToken, DateTime dtValuation)
        {
            return TestDataCache._previousRecordValutionDate;
        }

        public override IEnumerable<CompanyData> GetInvestmentRecordData(UserAccountToken userToken, DateTime dtValuation)
        {
            return new List<CompanyData>
            {
                new CompanyData
                {
                        Name = TestDataCache._TestCompany,
                        ValuationDate = dtValuation,
                        //LastBrought = (DateTime)reader["LastBoughtDate"],
                        Quantity = TestDataCache._TestQuantity,
                        TotalCost =  TestDataCache._TestTotalCost,
                        Dividend = TestDataCache._TestDividend,
                        SharePrice = TestDataCache._TestSharePrice
                }
            };
        }

        public override InvestmentInformation GetInvestmentDetails(string investment)
        {
            return new InvestmentInformation
            {
                Currency = TestDataCache._TestCurrency,
                Symbol = TestDataCache._TestSymbol,
                ScalingFactor = TestDataCache._TestScalingFactor
            };   
        }

        public override Trades GetHistoricalTransactions(DateTime dtFrom, DateTime dtTo, UserAccountToken userToken)
        {
            return null;
        }

        public override bool IsExistingRecordValuationDate(UserAccountToken userToken, DateTime dtValuation)
        {
            return false;
        }
    }

    [TestClass]
    public class CurrentInvestmentTests
    {
        private static string _TestAccount = "Guy SIPP";
        private static string _TestUser = "TestUser";

        private UserAccountToken _userToken = new UserAccountToken(
                                                    _TestUser,
                                                    _TestAccount,
                                                    AuthorizationLevel.ADMINISTRATOR);


        private InvestmentBuilder.InvestmentBuilder _investmentBuilder;
        
        [TestInitialize]
        public void Setup()
        {
            var datalayer = new DataLayerTest(new CurrentInvestmentsClientData(),
                                              new CurrentInvestmentsRecordData(),
                                              null,
                                              new CurrentInvestmentsUserAccountData(),
                                              null);

            _investmentBuilder = new InvestmentBuilder.InvestmentBuilder(
                new ConfigurationSettingsTest(),
                datalayer,
                new MarketDataServices.MarketDataService(new TestMarketDataSource()),
                new BrokerManager(),
                new CashAccountTransactionManager(datalayer),
                new InvestmentReportEmptyWriter());
            //public InvestmentBuilder(IConfigurationSettings settings, IDataLayer dataLayer, IMarketDataService marketDataService, BrokerManager brokerManager,
            //                         CashAccountTransactionManager cashAccountManager)

        }

        [TestMethod]
        public void When_getting_current_investments()
        {
            Console.WriteLine("getting current investments...");

            var results = _investmentBuilder.GetCurrentInvestments(_userToken, null).ToList();
            Assert.AreEqual(1, results.Count);
            var companyData = results[0];
            Assert.AreEqual(TestDataCache._TestCompany, companyData.Name);
            Assert.AreEqual(TestDataCache._TestDividend, companyData.Dividend);
            Assert.AreEqual("172.3190", companyData.MonthChange.ToString("#0.0000"));
            Assert.AreEqual("9.2565", companyData.MonthChangeRatio.ToString("#0.0000"));
            Assert.AreEqual("2033.9290", companyData.NetSellingValue.ToString("#0.0000"));
            Assert.AreEqual("951.1990", companyData.ProfitLoss.ToString("#0.0000"));
            Assert.AreEqual("28.3733", companyData.SharePrice.ToString("#0.0000"));
        }

        [TestMethod]
        public void When_getting_current_investments_with_price_override()
        {
            Console.WriteLine("getting current investments with price override");

            var prices = new ManualPrices()
                {
                    {TestDataCache._TestCompany, 27.50 }
                };

            var results = _investmentBuilder.GetCurrentInvestments(_userToken, prices).ToList();
            Assert.AreEqual(1, results.Count);
            var companyData = results[0];
            Assert.AreEqual(TestDataCache._TestCompany, companyData.Name);
            Assert.AreEqual(TestDataCache._TestDividend, companyData.Dividend);
            Assert.AreEqual("109.4400", companyData.MonthChange.ToString("#0.0000"));
            Assert.AreEqual("5.8788", companyData.MonthChangeRatio.ToString("#0.0000"));
            Assert.AreEqual("1971.0500", companyData.NetSellingValue.ToString("#0.0000"));
            Assert.AreEqual("888.3200", companyData.ProfitLoss.ToString("#0.0000"));
            Assert.AreEqual("27.5000", companyData.SharePrice.ToString("#0.0000"));
        }
    }
}
