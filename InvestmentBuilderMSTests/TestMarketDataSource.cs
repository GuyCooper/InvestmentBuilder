using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketDataServices;
using InvestmentBuilderCore;

namespace InvestmentBuilderMSTests
{
    internal class TestMarketDataSource : IMarketDataSource
    {
        public int Priority { get { return 0; } }
         
        public static double TestPrice = 21.43;
        public static double TestFxRate = 1.324;
        public static DateTime TestDate = new DateTime(2015, 02, 14);
        public static double HistoricalPrice = 45.87;
        public static string TestCurrency = "USD";

        public string Name { get { return "TestDatasource"; } }

        public IList<string> GetSources()
        {
            return new List<string> { Name };
        }

        public bool TryGetMarketData(string symbol, string exchange, string source, out MarketDataPrice marketData)
        {
            marketData = new MarketDataPrice(
                            symbol,
                            symbol,
                            TestPrice,
                            TestCurrency,
                            exchange);
            return true;
        }

        public bool TryGetFxRate(string baseCurrency, string contraCurrency, string exchange, string source, out double dFxRate)
        {
            dFxRate = TestFxRate;
            return true;
        }

        public IEnumerable<HistoricalData> GetHistoricalData(string instrument, string exchange, string source, DateTime dtFrom)
        {
            return new List<HistoricalData>
            {
                new HistoricalData
                (
                     date: TestDate,
                     price: HistoricalPrice
                )
            };
        }

        public IMarketDataReader DataReader { get; set; }
    }
}
