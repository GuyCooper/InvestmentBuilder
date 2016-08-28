﻿using System;
using System.Collections.Generic;
using System.Linq;
using PerformanceBuilderLib;
using InvestmentBuilderCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InvestmentBuilderMSTests
{
    internal static class PerfBuilderConstants
    {
        public static readonly  DateTime TestDate1 = DateTime.Parse("01/02/2016");
        public static readonly  DateTime TestDate2 = DateTime.Parse("02/03/2016");
        public static readonly  double TestPrice1 = 1.34d;
        public static readonly  double TestPrice2 = 1.36d;
        public static readonly  double TestPrice3 = 1.31d;
        public static readonly  double TestPrice4 = 1.32d;
        public static readonly  string TestCompany1 = "Acme";
        public static readonly  string TestCompany2 = "TheSun";

        public static readonly  double company1TotalCost = 998.34;
        public static readonly  double company2TotalCost = 734.86;

        public static readonly  double dividend1 = 56.21;
        public static readonly  double dividend2 = 34.21;

        public static readonly  IEnumerable<CompanyData> FullRecordInfo = new List<CompanyData>
        {
            new CompanyData
            {
                Name = TestCompany1,
                ValuationDate = TestDate1,
                TotalCost = company1TotalCost,
                SharePrice = 132.34,
                Quantity = 12,
                Dividend = 43.21
            },
           new CompanyData
            {
                Name = TestCompany1,
                ValuationDate = TestDate2,
                TotalCost = company1TotalCost,
                SharePrice = 136.34,
                Quantity = 12,
                Dividend = dividend1
            },
            new CompanyData
            {
                Name = TestCompany2,
                ValuationDate = TestDate2,
                TotalCost = company2TotalCost,
                SharePrice = 88.62,
                Quantity = 9,
                Dividend = dividend2
            }
        };

        public static readonly  string TestAccount = "TestAcc";
        public static readonly  string TestUser = "TestUser";
        public static readonly  DateTime ValuationDate = DateTime.Parse("12/04/2016");

        public static readonly UserAccountToken UserToken = new UserAccountToken(
                                               TestUser,
                                               TestAccount,
                                               AuthorizationLevel.UPDATE);

    }

    internal class PerfLibMarketDataSourceTest : MarketDataSourceTest
    {
        public override IEnumerable<HistoricalData> GetHistoricalData(string instrument, string exchange, string source, DateTime dtFrom)
        {
            return new List<HistoricalData>
            {
                new HistoricalData { Date = PerfBuilderConstants.TestDate1, Price = PerfBuilderConstants.TestPrice3 },
                new HistoricalData { Date = PerfBuilderConstants.TestDate2, Price = PerfBuilderConstants.TestPrice4 }
            };
        }
    }

    internal class PerfLibHistoricalData : HistoricalDataReaderTest
    {
        public override IEnumerable<HistoricalData> GetHistoricalAccountData(UserAccountToken userToken)
        {
            return new List<HistoricalData>
            {
                new HistoricalData { Date = PerfBuilderConstants.TestDate1 , Price = PerfBuilderConstants.TestPrice1 },
                new HistoricalData { Date = PerfBuilderConstants.TestDate2, Price = PerfBuilderConstants.TestPrice2 }
            };
        }
    }

    internal class PerfLibConfigurationTest : ConfigurationSettingsTest
    {
        public override IEnumerable<Index> ComparisonIndexes
        {
            get
            {
                return new List<Index> { new Index
                {Name = "TestIndex",
                 Symbol = "TEST" }};
            }
        }
    }

    internal class PerfLibClientDataInterfaceTest : ClientDataInterfaceTest
    {
        public override IEnumerable<string> GetActiveCompanies(UserAccountToken userToken, DateTime valuationDate)
        {
            return new List<string>
            {
                PerfBuilderConstants.TestCompany1,
                PerfBuilderConstants.TestCompany2
            };
        }
    }

    internal class PerfLibInvestmentRecordInterfaceTest : InvestmentRecordInterfaceTest
    {
        public override IEnumerable<CompanyData> GetFullInvestmentRecordData(UserAccountToken userToken) 
        {
            return PerfBuilderConstants.FullRecordInfo;
        }

        public override DateTime? GetLatestRecordInvestmentValuationDate(UserAccountToken userToken)
        {
            return PerfBuilderConstants.TestDate1;
        }

        public override IEnumerable<CompanyData> GetInvestmentRecordData(UserAccountToken userToken, DateTime dtValuation)
        {
            return PerfBuilderConstants.FullRecordInfo.Skip(1);
        }
    }

    [TestClass]
    public class PerformanceLibraryTests
    {
        private IDataLayer _dataLayer;
        [TestInitialize]
        public void Setup()
        {
            _dataLayer = new DataLayerTest(new PerfLibClientDataInterfaceTest(),
                         new PerfLibInvestmentRecordInterfaceTest(),
                         null,
                         null,
                         new PerfLibHistoricalData());
        }

        [TestMethod]
        public void When_Creating_Total_Performance_Ladders()
        {
            var ladderBuilder = new PerformanceLaddersBuilder(new PerfLibConfigurationTest(),
                                                              _dataLayer,
                                                              new PerfLibMarketDataSourceTest());

            var result = ladderBuilder.BuildPerformanceLadders(PerfBuilderConstants.UserToken, PerfBuilderConstants.ValuationDate);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);

            var rangeData = result.First();
            Assert.AreEqual(2, rangeData.Data.Count);
            Assert.AreEqual("All Time", rangeData.Name);

            var testAccData = rangeData.Data[0];
            var testIndexData = rangeData.Data[1];
            Assert.AreEqual("TestAcc", testAccData.Name);
            Assert.AreEqual("TestIndex", testIndexData.Name);

            Assert.AreEqual(2, testAccData.Data.Count);
            Assert.AreEqual(2, testIndexData.Data.Count);

            Assert.AreEqual(PerfBuilderConstants.TestDate1, testAccData.Data[0].Date);
            Assert.AreEqual(1d, testAccData.Data[0].Price);
            Assert.AreEqual(PerfBuilderConstants.TestDate2, testAccData.Data[1].Date);
            Assert.AreEqual("1.0149", testAccData.Data[1].Price.ToString("#0.0000"));
            Assert.AreEqual(PerfBuilderConstants.TestDate1, testIndexData.Data[0].Date);
            Assert.AreEqual(1d, testIndexData.Data[0].Price);
            Assert.AreEqual(PerfBuilderConstants.TestDate2, testIndexData.Data[1].Date);
            Assert.AreEqual("1.0076", testIndexData.Data[1].Price.ToString("#0.0000"));
        }

        [TestMethod]
        public void When_Creating_Company_Performance_Ladders()
        {
            var ladderBuilder = new PerformanceLaddersBuilder(new PerfLibConfigurationTest(),
                                                               _dataLayer,
                                                               new PerfLibMarketDataSourceTest());
            var result = ladderBuilder.BuildCompanyPerformanceLadders(PerfBuilderConstants.UserToken);

            Assert.AreEqual("Companies", result.Name);
            Assert.AreEqual(2, result.Data.Count);

            var indexData1 = result.Data[0];
            var indexData2 = result.Data[1];

            Assert.AreEqual(PerfBuilderConstants.TestCompany1, indexData1.Name);
            Assert.AreEqual(PerfBuilderConstants.TestCompany2, indexData2.Name);

            Assert.AreEqual(1d, indexData1.Data[0].Price);
            Assert.AreEqual("1.0374", indexData1.Data[1].Price.ToString("#0.0000"));

            Assert.AreEqual(2, indexData2.Data.Count);

            Assert.AreEqual(1d, indexData2.Data[0].Price);
            Assert.AreEqual("1.1319", indexData2.Data[1].Price.ToString("#0.0000"));
        }

        [TestMethod]
        public void when_creating_account_dividend_performance()
        {
            var ladderBuilder = new PerformanceLaddersBuilder(new PerfLibConfigurationTest(),
                                                                           _dataLayer,
                                                                           new PerfLibMarketDataSourceTest());
            var result = ladderBuilder.BuildAccountDividendPerformanceLadder(PerfBuilderConstants.UserToken);

            Assert.IsNotNull(result);

            Assert.AreEqual("Dividends", result.Name);

            var lstResults = result.Data.First().Data.ToList();
            Assert.AreEqual(2, lstResults.Count);

            Assert.AreEqual(PerfBuilderConstants.TestCompany1, lstResults[0].Key);
            Assert.AreEqual(PerfBuilderConstants.TestCompany2, lstResults[1].Key);
            Assert.AreEqual(PerfBuilderConstants.dividend1, lstResults[0].Price);
            Assert.AreEqual(PerfBuilderConstants.dividend2, lstResults[1].Price);

        }
    }

    /// <summary>
    /// same tests as PerformanceLibraryTests but with empty data
    /// </summary>
    [TestClass]
    public class PerformanceLibraryNoDataTests
    {
        private IDataLayer _dataLayer;

        [TestInitialize]
        public void Setup()
        {
            _dataLayer = new EmptyDataLayerTest();
        }

        [TestMethod]
        public void When_Creating_Empty_Total_Performance_Ladders()
        {
            var ladderBuilder = new PerformanceLaddersBuilder(new PerfLibConfigurationTest(),
                                                             _dataLayer,
                                                             new PerfLibMarketDataSourceTest());

            var result = ladderBuilder.BuildPerformanceLadders(PerfBuilderConstants.UserToken, PerfBuilderConstants.ValuationDate).ToList();

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void When_Creating_Empty_Company_Performance_Ladders()
        {
            var ladderBuilder = new PerformanceLaddersBuilder(new PerfLibConfigurationTest(),
                                                                   _dataLayer,
                                                                   new PerfLibMarketDataSourceTest());
            var result = ladderBuilder.BuildCompanyPerformanceLadders(PerfBuilderConstants.UserToken);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void when_creating_empty_account_dividend_performance()
        {
            var ladderBuilder = new PerformanceLaddersBuilder(new PerfLibConfigurationTest(),
                                                                               _dataLayer,
                                                                               new PerfLibMarketDataSourceTest());
            var result = ladderBuilder.BuildAccountDividendPerformanceLadder(PerfBuilderConstants.UserToken);

            Assert.IsNull(result);
        }
    }
}