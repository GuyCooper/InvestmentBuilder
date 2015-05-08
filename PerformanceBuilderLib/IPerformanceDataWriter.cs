using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketDataServices;
using InvestmentBuilderCore;

namespace PerformanceBuilderLib
{
    internal interface IPerformanceDataWriter
    {
        void WritePerformanceData(IList<IndexedRangeData> data);
    }

    public class IndexData
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public IList<HistoricalData> Data { get; set; }
    }

}
