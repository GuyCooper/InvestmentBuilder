using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketDataServices
{
    public enum SourceDataFormat
    {
        CSV,
        JSON,
        XML
    }

    /// <summary>
    /// abstract interface for reading raw data from a data source
    /// </summary>
    public interface IMarketDataReader
    {
        IEnumerable<string> GetData(string url, SourceDataFormat format);
    }
}
