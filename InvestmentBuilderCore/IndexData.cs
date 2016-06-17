using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestmentBuilderCore
{
    public class IndexData
    {
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public IList<HistoricalData> Data { get; set; }
    }

    //class defines a range of indexes to be displayed on a single
    //chart. 
    public class IndexedRangeData
    {
        public double MinValue { get; set; } //min value for all indexes in range
        public bool IsHistorical { get; set; }  //true if range contains only historical (date based) data. cannot mix historical and non historical. x-axis of chart will be date
        public string Name { get; set; }  //name for range of indexes. this will appear as sheet name
        public string KeyName { get; set; }   //if non historical data then all indexes in range must have a common key name. this forms x-axis of chart
        public string Title { get; set; } //this will appear as the title of the chart
        public IList<IndexData> Data { get; set; } //the list of indexes in range
    }
}
