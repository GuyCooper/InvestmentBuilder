﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InvestmentBuilderCore;
using InvestmentBuilder;

namespace InvestmentBuilderMSTests
{
    internal static class TestDataCache
    {
        public static readonly string _TestAccount = "TestAccount";
        public static readonly string _Currency = "GBP";
        public static readonly string _Description = "Test account";
        public static readonly string _Broker = "AJBell";

        public static readonly string _testUser = "testUser";
        public static readonly double _testUserValution = 1.24;
        public static readonly double _testUserSubscription = 1065;
        public static readonly DateTime _previousValutionDate = DateTime.Parse("23/10/2015");
        public static readonly DateTime _currentValuationDate = DateTime.Parse("25/11/2015");
        public static readonly DateTime _previousRecordValutionDate = DateTime.Parse("23/10/2015 10:32:56");

        public static readonly string _TestCompany = "Acme PLC";
        public static readonly int _TestQuantity = 72;
        public static readonly double _TestTotalCost = 1082.73;
        public static readonly double _TestDividend = 95.24;
        public static readonly double _TestNetSellingValue = 1328.42;

        public static readonly string _TestSymbol = "ACME.L";
        public static readonly string _TestCurrency = "GBP";
        public static readonly double _TestScalingFactor = 100;
        public static readonly double _TestSharePrice = 25.98;

        public static readonly CompanyData TestCompanyData = new CompanyData
        {
            Name = _TestCompany,
            ValuationDate = _currentValuationDate,
            TotalCost = _TestTotalCost,
            SharePrice = _TestSharePrice,
            Quantity = _TestQuantity,
            Dividend = _TestDividend,
            NetSellingValue = _TestNetSellingValue
        };

        public static readonly UserAccountToken _userToken = new UserAccountToken(_testUser,
                                                                    _TestAccount,
                                                                    AuthorizationLevel.UPDATE);


    }
    internal class CurrentInvestmentsUserAccountData : UserAccountInterfaceTest
    {
        public override UserAccountData GetUserAccountData(UserAccountToken userToken)
        {
            return new UserAccountData
            {
                Name = TestDataCache._TestAccount,
                Currency = TestDataCache._Currency,
                Description = TestDataCache._Description,
                Broker = TestDataCache._Broker
            };
        }

        public override void SaveNewUnitValue(UserAccountToken userToken, DateTime dtValuation, double dUnitValue)
        {
        }

        public override double GetStartOfYearValuation(UserAccountToken userToken, DateTime valuationDate)
        {
            return 0d;
        }

        public override IEnumerable<Redemption> GetRedemptions(UserAccountToken userToken, DateTime valuationDate)
        {
            return Enumerable.Empty<Redemption>();
        }

        public override double GetPreviousUnitValuation(UserAccountToken userToken, DateTime? previousDate)
        {
            return 1d;
        }

        public override IEnumerable<KeyValuePair<string, double>> GetMemberAccountData(UserAccountToken userToken, DateTime dtValuation)
        {
            return new List<KeyValuePair<string, double>>
            {
                new KeyValuePair<string, double>(TestDataCache._testUser,
                                                TestDataCache._testUserValution)
            };
        }

        public override double GetMemberSubscription(UserAccountToken userToken, DateTime dtValuation, string member)
        {
            return TestDataCache._testUserSubscription;
        }

        public override void UpdateMemberAccount(UserAccountToken userToken, DateTime dtValuation, string member, double dAmount)
        {
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

    public class CurrentInvestmentTestsBase
    {
        protected static string _TestAccount = "Guy SIPP";
        protected static string _TestUser = "TestUser";

        protected UserAccountToken _userToken = new UserAccountToken(
                                                    _TestUser,
                                                    _TestAccount,
                                                    AuthorizationLevel.ADMINISTRATOR);

        protected InvestmentBuilder.InvestmentBuilder _investmentBuilder;
    }

    [TestClass]
    public class CurrentInvestmentTests : CurrentInvestmentTestsBase
    {
        [TestInitialize]
        public virtual void Setup()
        {
            var datalayer = new DataLayerTest(new CurrentInvestmentsClientData(),
                                              new CurrentInvestmentsRecordData(),
                                              null,
                                              new CurrentInvestmentsUserAccountData(),
                                              null);

            var marketDataServices = new MarketDataServices.MarketDataService(new TestMarketDataSource());
            var brokerManager = new BrokerManager();
            _investmentBuilder = new InvestmentBuilder.InvestmentBuilder(
                new ConfigurationSettingsTest(),
                datalayer,
                new CashAccountTransactionManager(datalayer),
                new InvestmentReportEmptyWriter(),
                new InvestmentRecordBuilder(marketDataServices, datalayer, brokerManager)
                );
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

            double dTestPrice = 27.50;
            var prices = new ManualPrices()
                {
                    {TestDataCache._TestCompany, dTestPrice }
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
            Assert.AreEqual(dTestPrice.ToString("#0.000"), companyData.ManualPrice);
        }
    }

    [TestClass]
    public class EmptyCurrentInvestmentTests : CurrentInvestmentTestsBase
    {
        [TestInitialize]
        public void Setup()
        {
            var datalayer = new DataLayerTest(new ClientDataEmptyInterfaceTest(),
                                              new InvestmentRecordEmptyInterfaceTest(),
                                              null,
                                              new CurrentInvestmentsUserAccountData(),
                                              null);

            var marketDataServices = new MarketDataServices.MarketDataService(new TestMarketDataSource());
            var brokerManager = new BrokerManager();
            _investmentBuilder = new InvestmentBuilder.InvestmentBuilder(
                new ConfigurationSettingsTest(),
                datalayer,
                new CashAccountTransactionManager(datalayer),
                new InvestmentReportEmptyWriter(),
                new InvestmentRecordBuilder(marketDataServices, datalayer, brokerManager)
                );
        }

        [TestMethod]
        public void When_getting_empty_current_investments()
        {
            Console.WriteLine("When_getting_empty_current_investments test");
            var results = _investmentBuilder.GetCurrentInvestments(_userToken, null).ToList();

            Assert.AreEqual(0, results.Count);
        }
    }
}