using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderCore;

namespace MarketDataServices
{
    /// <summary>
    /// this class registers all the required services for a production ready market data service source
    /// </summary>
    public static class MarketDataRegisterService
    {
        public static void RegisterServices()
        {
            ContainerManager.RegisterType(typeof(IMarketDataReader), typeof(WebMarketDataReader), true);
            ContainerManager.RegisterType(typeof(IMarketDataSerialiser), typeof(MarketDataFileSerialiser), true, "testMarketData.txt");
            ContainerManager.RegisterType(typeof(IMarketSourceLocator), typeof(MefMarketSourceLocator), true);
            ContainerManager.RegisterType(typeof(IMarketDataSource), typeof(CachedMarketDataSource), true);
        }
    }
}
