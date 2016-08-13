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
    internal class EmptyTestInvestmentRecordBuilder : IInvestmentRecordDataManager
    {
        public virtual IEnumerable<CompanyData> GetInvestmentRecords(UserAccountToken userToken, UserAccountData account, DateTime dtValuationDate, DateTime? dtPreviousValuationDate, ManualPrices manualPrices, bool bSnapshot)
        {
            return Enumerable.Empty<CompanyData>();
        }

        public virtual IEnumerable<CompanyData> GetInvestmentRecordSnapshot(UserAccountToken userToken, UserAccountData account, ManualPrices manualPrices)
        {
            return Enumerable.Empty<CompanyData>();
        }

        public virtual DateTime? GetLatestRecordValuationDate(UserAccountToken userToken)
        {
            return null;
        }

        public virtual bool UpdateInvestmentRecords(UserAccountToken userToken, UserAccountData account, Trades trades, CashAccountData cashData, DateTime valuationDate, ManualPrices manualPrices)
        {
            return true;
        }
    }

    internal class SimpleTestInvestmentRecordBuilder : EmptyTestInvestmentRecordBuilder
    {
        public override IEnumerable<CompanyData> GetInvestmentRecords(UserAccountToken userToken, UserAccountData account, DateTime dtValuationDate, DateTime? dtPreviousValuationDate, ManualPrices manualPrices, bool bSnapshot)
        {
            return new List<CompanyData>
            {
                TestDataCache.TestCompanyData
            };
        }

        public override IEnumerable<CompanyData> GetInvestmentRecordSnapshot(UserAccountToken userToken, UserAccountData account, ManualPrices manualPrices)
        {
            return new List<CompanyData>
            {
                TestDataCache.TestCompanyData
            };
        }

        public override DateTime? GetLatestRecordValuationDate(UserAccountToken userToken)
        {
            return TestDataCache._previousRecordValutionDate;
        }
    }

    /// <summary>
    /// this class provides unit tests for the InvestmentBuilder class
    /// </summary>
    [TestClass]
    public class InvestmentBuilderTests
    {
        private InvestmentBuilder.InvestmentBuilder CreateEmptyBuilder()
        {
            var dataLayer = new DataLayerTest(new ClientDataEmptyInterfaceTest(),
                                               new InvestmentRecordEmptyInterfaceTest(),
                                               new CashAccountEmptyInterfaceTest(),
                                               new CurrentInvestmentsUserAccountData(),
                                               new EmptyHistoricalDataReaderTest());



            var marketDataServices = new MarketDataServices.MarketDataService(new TestMarketDataSource());
            var builder = new InvestmentBuilder.InvestmentBuilder(new ConfigurationSettingsTest(),
                                                                  dataLayer,
                                                                  new InvestmentBuilder.CashAccountTransactionManager(dataLayer),
                                                                  new InvestmentReportEmptyWriter(),
                                                                  new EmptyTestInvestmentRecordBuilder());
            return builder;
        }

        [TestMethod]
        public void When_getting_report_file_name()
        {
            var builder = CreateEmptyBuilder();
            var reportFile = builder.GetInvestmentReport(TestDataCache._userToken, TestDataCache._currentValuationDate);
            Assert.AreEqual(InvestmentReportEmptyWriter.FileName, reportFile);
        }

        [TestMethod]
        public void When_building_empty_asset_report()
        {

            var builder = CreateEmptyBuilder();
            var report = builder.BuildAssetReport(TestDataCache._userToken, TestDataCache._currentValuationDate, true, null);

            Assert.IsNotNull(report);
            Assert.AreEqual(TestDataCache._TestAccount, report.AccountName);
            Assert.AreEqual(TestDataCache._Currency, report.ReportingCurrency);
            Assert.AreEqual(0d, report.BankBalance);
            Assert.AreEqual(0, report.Assets.Count());
            Assert.AreEqual(0d, report.IssuedUnits);
            Assert.AreEqual(0d, report.NetAssets);
            Assert.AreEqual(0d, report.TotalAssets);
            Assert.AreEqual(1d, report.ValuePerUnit);
        }

        [TestMethod]
        public void When_building_simple_asset_report()
        {
            var dataLayer = new DataLayerTest(new CurrentInvestmentsClientData(),
                                              new CurrentInvestmentsRecordData(),
                                              new CashAccountEmptyInterfaceTest(),
                                              new CurrentInvestmentsUserAccountData(),
                                              new EmptyHistoricalDataReaderTest());



            var marketDataServices = new MarketDataServices.MarketDataService(new TestMarketDataSource());
            var builder = new InvestmentBuilder.InvestmentBuilder(new ConfigurationSettingsTest(),
                                                                  dataLayer,
                                                                  new InvestmentBuilder.CashAccountTransactionManager(dataLayer),
                                                                  new InvestmentReportEmptyWriter(),
                                                                  new SimpleTestInvestmentRecordBuilder());

            var report = builder.BuildAssetReport(TestDataCache._userToken, TestDataCache._currentValuationDate, true, null);

            Assert.IsNotNull(report);

            Assert.AreEqual(TestDataCache._TestAccount, report.AccountName);
            Assert.AreEqual(TestDataCache._testUserSubscription + TestDataCache._testUserValution, report.IssuedUnits);
            Assert.AreEqual(TestDataCache._TestNetSellingValue, report.NetAssets);
            Assert.AreEqual(0d, report.BankBalance);
            var calculatedUnitPrice = TestDataCache._TestNetSellingValue / (TestDataCache._testUserSubscription + TestDataCache._testUserValution);
            Assert.AreEqual(calculatedUnitPrice, report.ValuePerUnit);

        }
    }
}
