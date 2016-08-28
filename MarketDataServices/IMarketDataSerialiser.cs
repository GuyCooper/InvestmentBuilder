using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketDataServices
{
    /// <summary>
    /// abstracted interface for serialising market data
    /// </summary>
    internal interface IMarketDataSerialiser
    {
        void StartSerialiser();
        void EndSerialiser();
        void SerialiseData(string data, params object[] prm);
    }
}
