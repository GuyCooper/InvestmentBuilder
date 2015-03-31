using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketDataServices
{
    interface IMarketDataSource
    {
        double GetMarketData(string symbol);
        double GetFxRate(string baseCurrency, string contraCurrency);
        IEnumerable<HistoricalData> GetHistoricalData(string instrument, DateTime dtFrom);
    }
}
