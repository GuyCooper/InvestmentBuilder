using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
//using System.Net;
using System.IO;
using NLog;
using InvestmentBuilderCore;

namespace MarketDataServices
{
    /// <summary>
    /// class gets market data from yahoo
    /// </summary>
    [Export(typeof(IMarketDataSource))]
    internal class YahooMarketDataSource : TestFileMarketDataSource
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public YahooMarketDataSource() 
        {
        }

        public override string Name { get { return "Yahoo"; } }

        public override int Priority { get { return 1; } }

    }
}