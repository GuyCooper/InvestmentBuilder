using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InvestmentBuilder;
using InvestmentBuilderCore;
using MarketDataServices;

namespace InvestmentBuilderMSTests
{
    internal static class InvestmentRecordStaticTestData
    {
        public static readonly string TestAccount = "Guy SIPP";
        public static readonly double TestPrice = 25.54;
        public static readonly string TestUser = "TestUser";
        public static readonly DateTime dtValuation = DateTime.Parse("15/10/2015");
        public static readonly DateTime dtPreviousValuation = DateTime.Parse("14/09/2015");
        public static readonly string Company = "Acme PLC";
        public static readonly double Dividends = 75.98;
        public static readonly string TestCurrency = "EUR";
        public static readonly string TestSymbol = "ACME.DE";
        public static readonly double TestScaling = 1.0d;
        public static readonly int TestQuantity = 132;
        public static readonly double TestSharePrice = 26.43;
        public static readonly double TestTotalCost = 2676.43;
        public static readonly UserAccountToken UserToken = new UserAccountToken(
                                                            TestUser,
                                                            TestAccount,
                                                            AuthorizationLevel.ADMINISTRATOR);
    }

    internal class InvestmentRecordTestData : InvestmentRecordInterfaceTest
    {
        private Dictionary<string, double> _investmentPriceList;

        public InvestmentRecordTestData()
        {
            _investmentPriceList = new Dictionary<string, double>();
        }

        public override DateTime? GetLatestRecordInvestmentValuationDate(UserAccountToken userToken)
        {
            return InvestmentRecordStaticTestData.dtPreviousValuation;
        }

        public override DateTime? GetPreviousRecordInvestmentValuationDate(UserAccountToken userToken, DateTime dtValuation)
        {
            return InvestmentRecordStaticTestData.dtPreviousValuation;
        } 

        public override IEnumerable<KeyValuePair<string, double>> GetInvestments(UserAccountToken userToken, DateTime dtValuation)
        {
            return new List<KeyValuePair<string, double>>
            {
                new KeyValuePair<string, double>(InvestmentRecordStaticTestData.Company,
                                                 InvestmentRecordStaticTestData.TestPrice)
            };
        }

        public override InvestmentInformation GetInvestmentDetails(string investment)
        {
            return new InvestmentInformation
            {
                Currency = InvestmentRecordStaticTestData.TestCurrency,
                ScalingFactor = InvestmentRecordStaticTestData.TestScaling,
                Symbol = InvestmentRecordStaticTestData.TestSymbol
            };
        }

        public override void RollInvestment(UserAccountToken userToken, string investment, DateTime dtValuation, DateTime dtPreviousValaution)
        {
        }

        public override void UpdateDividend(UserAccountToken userToken, string investment, DateTime dtValuation, double dividend)
        {
        }

        public override void UpdateClosingPrice(UserAccountToken userToken, string investment, DateTime dtValuation, double price)
        {
            _investmentPriceList.Add(investment, price);
        }

        public double GetLatestPrice(string investment)
        {
            return _investmentPriceList[investment];
        }

        public override void AddTradeTransactions(IEnumerable<Stock> trades, TradeType action, UserAccountToken userToken, DateTime dtValuation)
        {
        }

        public override IEnumerable<CompanyData> GetInvestmentRecordData(UserAccountToken userToken, DateTime dtValuation)
        {
            return new List<CompanyData>
            {
                new CompanyData
                {
                    Name = InvestmentRecordStaticTestData.Company,
                    AveragePricePaid = InvestmentRecordStaticTestData.TestPrice,
                    Dividend = InvestmentRecordStaticTestData.Dividends,
                    Quantity = InvestmentRecordStaticTestData.TestQuantity,
                    SharePrice = InvestmentRecordStaticTestData.TestSharePrice,
                    TotalCost = InvestmentRecordStaticTestData.TestTotalCost
                }
            };
        }

        public override Trades GetHistoricalTransactions(DateTime dtFrom, DateTime dtTo, UserAccountToken userToken)
        {
            return new Trades
            {
                Buys = new List<Stock>
                {
                    new Stock
                    {
                         Name = InvestmentRecordStaticTestData.Company,
                          Quantity = 24,
                          TotalCost = 658.65
                    }
                }.ToArray(),
                Sells = Enumerable.Empty<Stock>().ToArray(),
                Changed = Enumerable.Empty<Stock>().ToArray()
            };
        }
    }

    [TestClass]
    public class InvestmentRecordTests
    {

        private readonly UserAccountData _userData = new UserAccountData
        {
            Broker = "ShareCentre",
            Currency = "GBP",
            Description = "Test Account",
            Name = InvestmentRecordStaticTestData.TestAccount
        };

        private CashAccountData _cashData;
        private InvestmentRecordBuilder _builder;

        private InvestmentRecordTestData _testDataInterface;
        [TestInitialize]
        public void Setup()
        {
            var mdSource = new TestMarketDataSource();
            var mdService = new MarketDataService(mdSource);
             _testDataInterface = new InvestmentRecordTestData();

            _builder = new InvestmentRecordBuilder(mdService
                                                   , _testDataInterface
                                                   , new BrokerManager());
            _cashData = new CashAccountData();
            _cashData.Dividends.Add(InvestmentRecordStaticTestData.Company, InvestmentRecordStaticTestData.Dividends);
        }

        [TestMethod]
        public void When_updating_investment_records()
        {
           var result =  _builder.UpdateInvestmentRecords(InvestmentRecordStaticTestData.UserToken
                                             , _userData
                                             , null
                                             , _cashData
                                             , InvestmentRecordStaticTestData.dtValuation
                                             , null);

            Assert.IsTrue(result);

            var price = _testDataInterface.GetLatestPrice(InvestmentRecordStaticTestData.Company).ToString("#.####");

            var comparePrice = (TestMarketDataSource.TestPrice * TestMarketDataSource.TestFxRate).ToString("#.####");
            Assert.AreEqual(comparePrice, price);
        }

        [TestMethod]
        public void When_getting_investment_records()
        {
            var result = _builder.GetInvestmentRecords(InvestmentRecordStaticTestData.UserToken,
                                          _userData,
                                          InvestmentRecordStaticTestData.dtValuation,
                                          InvestmentRecordStaticTestData.dtPreviousValuation,
                                          null,
                                          false).ToList();

            Assert.AreEqual(1, result.Count);

            var data = result[0];
            Assert.AreEqual(data.AveragePricePaid, 25.54);
            Assert.AreEqual(data.Dividend, 75.98);
            var strChange = data.MonthChange.ToString("#.###");
            Assert.AreEqual(strChange, "-658.65");
            var strMonthChange = data.MonthChangeRatio.ToString("#.###");
            Assert.AreEqual(strMonthChange, "-19.07");
            Assert.AreEqual(data.Name, InvestmentRecordStaticTestData.Company);
            Assert.AreEqual(data.NetSellingValue.ToString("#.###"), "3453.872");
            Assert.AreEqual(data.ProfitLoss.ToString("#.###"),  "777.442");
            Assert.AreEqual(data.Quantity, 132);
            Assert.AreEqual(data.SharePrice, 26.43);
            Assert.AreEqual(data.TotalCost, 2676.43);
        }

        [TestMethod]
        public void When_getting_investment_record_snapshot()
        {
            var result = _builder.GetInvestmentRecords(InvestmentRecordStaticTestData.UserToken,
                                                  _userData,
                                                  InvestmentRecordStaticTestData.dtValuation,
                                                  null,
                                                  null,
                                                  true).ToList();
            Assert.AreEqual(1, result.Count);
            var data = result[0];
            Assert.AreEqual(data.AveragePricePaid, 25.54);
            Assert.AreEqual(data.Dividend, 75.98);
            Assert.AreEqual(0, data.MonthChange);
            Assert.AreEqual(0, data.MonthChangeRatio);
            Assert.AreEqual(data.Name, InvestmentRecordStaticTestData.Company);
            Assert.AreEqual(data.NetSellingValue.ToString("#.###"), "3707.825");
            Assert.AreEqual(data.ProfitLoss.ToString("#.###"), "1031.395");
            Assert.AreEqual(data.Quantity, 132);
            Assert.AreEqual(data.SharePrice.ToString("#.###"), "28.373");
            Assert.AreEqual(data.TotalCost, 2676.43);
        }
    }
}
