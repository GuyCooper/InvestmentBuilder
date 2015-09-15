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
            //msTests.When_getting_a_closing_price_from_yahoo();
            //msTests.When_getting_a_closing_price_from_yahoo_with_fx();
            //msTests.When_getting_a_closing_price_from_aggregate();

            var utilityTests = new UtilityTests();
            utilityTests.When_AggregatingTradeList(@"TestFiles\testAggregateTrades.xml");

            var tradeLoaderTests = new TradeLoaderTests();
            tradeLoaderTests.When_loading_Trade_File(@"TestFiles\Trades.xml");
            tradeLoaderTests.When_Saving_Trades();
            tradeLoaderTests.When_Saving_Trades1();
            tradeLoaderTests.When_Saving_Trades2();

            var fullTest = new FullInvestmentBuilderTests();
            fullTest.Setup();

            fullTest.When_getting_current_investments();

            fullTest.When_getting_current_investments_with_price_overrides();

            fullTest.RunFullTests();

            Console.WriteLine("All Tests Passed!!");

        }
    }
}
