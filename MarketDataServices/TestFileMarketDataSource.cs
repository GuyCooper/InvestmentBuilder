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
   
        private Dictionary<string, MarketDataPrice> _marketDataLookup = new Dictionary<string, MarketDataPrice>();
        private Dictionary<string, double> _fxDataLookup = new Dictionary<string, double>();

        //private const string _testDataPath = @"C:\Projects\TestData\InvestmentBuilder";
        //private const string _testDataFile = "testMarketData.txt";

        private void _addMarketDataToLookup(string name, string strPrice, string strCurrency, Dictionary<string, MarketDataPrice> lookup)
        {
            double dPrice;
            if (double.TryParse(strPrice, out dPrice))
            {
                lookup.Add(name, new MarketDataPrice
                {
                    Name = name,
                    Price = dPrice,
                    Currency = strCurrency
                });
            }
        }

        private void _addDataToLookup(string name, string strPrice, Dictionary<string, double> lookup)
        {
            double dRate;
            if (double.TryParse(strPrice, out dRate))
            {
                lookup.Add(name, dRate);
            }
        }

        public TestFileMarketDataSource(string fileName)
        {
            //var fileName = Path.Combine(_testDataPath, _testDataFile);//Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "testMarketData.txt";
            using (var reader = new StreamReader(fileName))
            {
                string line;
                while((line = reader.ReadLine()) != null)
                {
                    var elems = line.Split(',');
                    if(elems.Length > 2)
                    {
                        if (string.Compare(elems[0], "M", true) == 0)
                        {
                            if (elems.Length > 3)
                            {
                                _addMarketDataToLookup(elems[1], elems[2], elems[3], _marketDataLookup);
                            }
                        }
                        else if (string.Compare(elems[0], "F", true) == 0)
                        {
                            _addDataToLookup(elems[1], elems[2], _fxDataLookup);
                        }
                    }
                }
            }
        }

        public bool TryGetMarketData(string symbol, string exchange, out MarketDataPrice marketData)
        {
            if(_marketDataLookup.TryGetValue(symbol, out marketData))
            {
                if (marketData.Currency[marketData.Currency.Length - 1] == 'p')
                {
                    marketData.Price = marketData.Price / 100d;
                    marketData.Currency = marketData.Currency.ToUpper();
                }
                return true;
            }
            return false;
        }

        public bool TryGetFxRate(string baseCurrency, string contraCurrency, out double dFxRate)
        {
            return _fxDataLookup.TryGetValue(baseCurrency + contraCurrency, out dFxRate);
        }

        private IEnumerable<HistoricalData> _GenerateHistoricalData(DateTime dtFrom, double dIncrement)
        {
            //always take basedate from first day of month
            DateTime dtDate = new DateTime(dtFrom.Year,dtFrom.Month, 1);
           
            double dPrice = 1.0;
            while(dtDate <= DateTime.Today)
            {
                yield return new HistoricalData
                {
                    Date = dtDate,
                    Price = dPrice
                };

                dtDate = dtDate.AddMonths(1);
                dPrice += dIncrement;
            }
        }

        public IEnumerable<HistoricalData> GetHistoricalData(string instrument, DateTime dtFrom)
        {
            if(instrument.Contains("FTSE"))
            {
                return _GenerateHistoricalData(dtFrom, 0.008); //ftse
            }
            else if(instrument.Contains("GSPC"))
            {
                return _GenerateHistoricalData(dtFrom, 0.009); //s&p
            }
            
            return _GenerateHistoricalData(dtFrom, 0.006);
        }

        public string Name
        {
            get { return "TestFileMarketDataSource"; }
        }
    }
}
