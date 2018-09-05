using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using InvestmentBuilderCore;

namespace MarketDataServices
{
    /// <summary>
    /// market data file serialiser class. saves data to a file
    /// </summary>
    internal class MarketDataFileSerialiser : IMarketDataSerialiser
    {
        private string _fileName;
        private StreamWriter _writer;

        public MarketDataFileSerialiser(IConfigurationSettings settings)
        {
            _fileName = settings.OutputCachedMarketData;
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
            if (_fileName != null &&_writer == null)
            {
                _writer = new StreamWriter(_fileName);
            }
        }

        public void LoadData(Action<string> processRecord)
        {
            if(_fileName == null)
            {
                return;
            }

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
