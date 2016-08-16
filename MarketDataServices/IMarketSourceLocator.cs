using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketDataServices
{
    public interface IMarketSourceLocator
    {
        IEnumerable<IMarketDataSource> Sources { get; }
    }
}
