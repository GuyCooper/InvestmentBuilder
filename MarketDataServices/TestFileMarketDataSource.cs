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
    public class TestFileMarketDataSource : IMarketDataSource, IDisposable
    {
   
        private Dictionary<string, MarketDataPrice> _marketDataLookup = new Dictionary<string, MarketDataPrice>();
        private Dictionary<string, double> _fxDataLookup = new Dictionary<string, double>();
        private Dictionary<string, IList<HistoricalData>> _historicalDataLookup = new Dictionary<string, IList<HistoricalData>>();

        //private const string _testDataPath = @"C:\Projects\TestData\InvestmentBuilder";
        //private const string _testDataFile = "testMarketData.txt";

        private void _addMarketDataToLookup(string name, string strPrice, string strCurrency, Dictionary<string, MarketDataPrice> lookup)
        {
            if(lookup.ContainsKey(name) == true)
            {
                return;
            }

            double dPrice;
            if (double.TryParse(strPrice, out dPrice))
            {
                lookup.Add(name, new MarketDataPrice
                (
                    name,
                    name,
                    dPrice,
                    strCurrency
                ));
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

        //historical data has format dd/mm/yyyy=value:dd/mm/yyyy=value:etc...
        private void _AddDataToHistoricalLookup(string name, string data)
        {
            _historicalDataLookup.Add(name,
                data.Split(':').Select(x =>
                {
                    int split = x.IndexOf('=');
                    return new HistoricalData
                    (
                        date: DateTime.Parse(x.Substring(0, split)),
                        price: Double.Parse(x.Substring(split + 1))
                    );
                }).ToList());
        }

        public TestFileMarketDataSource(string fileName)
        {
            //var fileName = Path.Combine(_testDataPath, _testDataFile);//Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "testMarketData.txt";
            using (var reader = new StreamReader(fileName))
            {
                string line;
                while((line = reader.ReadLine()) != null)
                {
                    var elems = line.Split(';');
                    if(elems.Length > 2)
                    {
                        if (elems[0].Equals("M", StringComparison.CurrentCultureIgnoreCase) == true)
                        {
                            if (elems.Length > 3)
                            {
                                _addMarketDataToLookup(elems[1], elems[2], elems[3], _marketDataLookup);
                            }
                        }
                        else if (elems[0].Equals("F", StringComparison.CurrentCultureIgnoreCase) == true)
                        {
                            _addDataToLookup(elems[1], elems[2], _fxDataLookup);
                        }
                        else if(elems[0].Equals("H", StringComparison.CurrentCultureIgnoreCase) == true)
                        {
                            _AddDataToHistoricalLookup(elems[1], elems[2]);
                        }
                    }
                }
            }
        }

        public IList<string> GetSources()
        {
            return new List<string> { Name };
        }

        public bool TryGetMarketData(string symbol, string exchange,string source,  out MarketDataPrice marketData)
        {
            if(_marketDataLookup.TryGetValue(symbol, out marketData))
            {
                marketData.DecimalisePrice();
                return true;
            }
            return false;
        }

        public bool TryGetFxRate(string baseCurrency, string contraCurrency, string exchange, string source, out double dFxRate)
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
                (
                    date: dtDate,
                    price: dPrice
                );

                dtDate = dtDate.AddMonths(1);
                dPrice += dIncrement;
            }
        }

        public IEnumerable<HistoricalData> GetHistoricalData(string instrument, string exchange, string source, DateTime dtFrom)
        {
            //first check if instrument is in historical data cache
            IList<HistoricalData> cache;
            if(_historicalDataLookup.TryGetValue(instrument, out cache) == true)
            {
                var result = cache.Where(x => x.Date >= dtFrom).ToList();
                if(result.Count > 0)
                {
                    return result;
                }
            }

            //if not in cache then just generate some historical price data

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

        public void Dispose()
        {
            Console.WriteLine("disposing TestDataSource...");
        }

        public string Name
        {
            get { return "TestFileMarketDataSource"; }
        }

        public int Priority { get { return 1; } }

        public IMarketDataReader DataReader { get; set; }
    }
}
