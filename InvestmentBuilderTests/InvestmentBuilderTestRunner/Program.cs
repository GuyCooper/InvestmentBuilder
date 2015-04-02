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
        }
    }
}
