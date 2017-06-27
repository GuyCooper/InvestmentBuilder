using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MarketDataServices
{
    /// <summary>
    /// market data file serialiser class. saves data to a file
    /// </summary>
    internal class MarketDataFileSerialiser : IMarketDataSerialiser
    {
        private string _fileName;
        private StreamWriter _writer;

        public MarketDataFileSerialiser(string fileName)
        {
            _fileName = fileName;
        }

        public void EndSerialiser()
        {
            if(_writer != null)
            {
                _writer.Dispose();
                _writer = null;
            }
        }

        public void SerialiseData(string data, params object[] prm)
        {
            if(_writer != null)
            {
                _writer.WriteLine(data, prm);
            }
        }

        public void StartSerialiser()
        {
            if (_writer == null)
            {
                _writer = new StreamWriter(_fileName);
            }
        }

        public void LoadData(Action<string> processRecord)
        {
            using (var reader = new StreamReader(_fileName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if(processRecord != null)
                    {
                        processRecord(line);
                    }
                }
            }
        }
    }
}
