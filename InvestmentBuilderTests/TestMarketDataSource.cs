using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketDataServices;
using InvestmentBuilderCore;

namespace InvestmentBuilderTests
{
    internal class TestMarketDataSource : IMarketDataSource
    {
        public static double TestPrice = 21.43;
        public static double TestFxRate = 1.324;
        public static DateTime TestDate = new DateTime(2015, 02, 14);
        public static double HistoricalPrice = 45.87;

        public string Name { get { return "TestDatasource"; } }

        public bool TryGetMarketData(string symbol, string exchange, out double dData)
        {
            dData = TestPrice;
            return true;
        }

        public bool TryGetFxRate(string baseCurrency, string contraCurrency, out double dFxRate)
        {
            dFxRate = TestFxRate;
            return true;
        }

        public IEnumerable<HistoricalData> GetHistoricalData(string instrument, DateTime dtFrom)
        {
            return new List<HistoricalData>
            {
                new HistoricalData
                {
                     Date = TestDate,
                     Price = HistoricalPrice
                }
            };
        }
    }
}
