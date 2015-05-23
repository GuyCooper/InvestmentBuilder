using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderTests;

namespace InvestmentBuilderTestRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            MarketDataServiceTests msTests = new MarketDataServiceTests();
            msTests.Setup();
            msTests.When_getting_a_closing_price();
            msTests.When_getting_a_closing_price_from_yahoo();
            msTests.When_getting_a_closing_price_from_yahoo_with_fx();
            msTests.When_getting_a_closing_price_from_aggregate();

            var utilityTests = new UtilityTests();
            utilityTests.When_AggregatingTradeList();
        }
    }
}
