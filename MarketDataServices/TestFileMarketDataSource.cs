﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
//using System.Reflection;

namespace MarketDataServices
{
    /// <summary>
    /// market data source based on a test file.Just looks forthe symbol ina test file
    /// and returns value specified. returns true if symbol found otherwise returns
    /// false. does the same for FxRates
    ///  file has format M/F,Symbol,Value
    ///  where M = stock dataand F= FX data
    /// This class is useful for full application testing when there are no datasources available
    /// 
    /// </summary>
    public class TestFileMarketDataSource : IMarketDataSource
    {
   
        private Dictionary<string, double> _marketDataLookup = new Dictionary<string, double>();
        private Dictionary<string, double> _fxDataLookup = new Dictionary<string, double>();

        private const string _testDataPath = @"C:\Projects\TestData\InvestmentBuilder";
        private const string _testDataFile = "testMarketData.txt";

        private void _addDataToLookup(string name, string strValue, Dictionary<string, double> lookup)
        {
            double dRate;
            if (double.TryParse(strValue, out dRate))
            {
                lookup.Add(name, dRate);
            }
        }

        public TestFileMarketDataSource()
        {
            var fileName = Path.Combine(_testDataPath, _testDataFile);//Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "testMarketData.txt";
            using (var reader = new StreamReader(fileName))
            {
                string line;
                while((line = reader.ReadLine()) != null)
                {
                    var elems = line.Split(',');
                    if(elems.Length == 3)
                    {
                        if (string.Compare(elems[0], "M", true) == 0)
                        {
                            _addDataToLookup(elems[1], elems[2], _marketDataLookup);
                        }
                        if (string.Compare(elems[0], "F", true) == 0)
                        {
                            _addDataToLookup(elems[1], elems[2], _fxDataLookup);
                        }
                    }
                }
            }
        }

        public bool TryGetMarketData(string symbol, string exchange, out double dData)
        {
            return _marketDataLookup.TryGetValue(symbol, out dData);
        }

        public bool TryGetFxRate(string baseCurrency, string contraCurrency, out double dFxRate)
        {
            return _fxDataLookup.TryGetValue(baseCurrency + contraCurrency, out dFxRate);
        }

        public IEnumerable<HistoricalData> GetHistoricalData(string instrument, DateTime dtFrom)
        {
            throw new NotImplementedException();
        }

        public string Name
        {
            get { return "TestFileMarketDataSource"; }
        }
    }
}
