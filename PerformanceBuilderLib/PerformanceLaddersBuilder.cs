using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketDataServices;
using NLog;
using InvestmentBuilderCore;

namespace PerformanceBuilderLib
{
    //class defines a range of indexes to be displayed on a single
    //chart. 
    public class IndexedRangeData
    {
        public double MinValue { get; set; } //min value for all indexes in range
        public bool IsHistorical { get; set; }  //true if range contains only historical (date based) data. cannot mix historical and non historical. x-axis of chart will be date
        public string Name { get; set; }  //name for range of indexes. this will appear as sheet name
        public string KeyName { get; set; }   //if non historical data then all indexes in range must have a common key name. this forms x-axis of chart
        public IList<IndexData> Data { get; set; } //the list of indexes in range
    }

    /// <summary>
    /// this class builds all the different perfomance ladders that will be included in the
    /// report.
    /// </summary>
    internal class PerformanceLaddersBuilder
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private IConfigurationSettings _settings;
        private IHistoricalDataReader _historicalDataReader;
        private IInvestmentRecordInterface _investmentRecordData;
        private IClientDataInterface _ClientRecordData;
        private IMarketDataSource _marketDataSource;

        public PerformanceLaddersBuilder(IConfigurationSettings settings, IDataLayer dataLayer, IMarketDataSource marketDataSource)
        {
            _historicalDataReader = dataLayer.HistoricalData;
            _investmentRecordData = dataLayer.InvestmentRecordData;
            _ClientRecordData = dataLayer.ClientData;
            _marketDataSource = marketDataSource;
            _settings = settings;
        }

        private DateTime MonthlyDate(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1);
        }

        private void DumpData(string title, IEnumerable<HistoricalData> enData)
        {
            //Console.WriteLine(title);
            logger.Log(LogLevel.Info, title);
            foreach (var elem in enData)
            {
                logger.Log(LogLevel.Info, "{0} : {1}", elem.Date, elem.Price);
                //Console.WriteLine("{0} : {1}", elem.Date, elem.Price);
            }
        }

        private bool _GetIndexRangeForYear(DateTime dtStartDate, int years, string description, IList<Tuple<DateTime?, string>> listIndexes)
        {
            var dtYear = DateTime.Today.AddYears(years);
            if (dtStartDate < dtYear)
            {
                //add a one year chart
                listIndexes.Add(new Tuple<DateTime?, string>(dtYear, description));
                return true;
            }
            return false;
        }
        /// <summary>
        /// using the historical data for the account,determine index ranges we
        /// want to include in the report.for example. if the account has data
        /// for the last 3 1/2 years, we would want to have 1, 3 and all time
        /// charts
        /// </summary>
        /// <returns></returns>
        private IList<Tuple<DateTime?, string>> _DetermineIndexRanges(IEnumerable<HistoricalData> historicalData)
        {
            //historical data must be in ascending chronological order (oldest data first)
            var result = new List<Tuple<DateTime?, string>>();
            var firstRecord = historicalData.FirstOrDefault();
            if (firstRecord != null && firstRecord.Date.HasValue)
            {
                //add all time range
                result.Add(new Tuple<DateTime?, string>(null, "All Time"));
                int previousYear = -1;
                string description = "1 year";
                while (_GetIndexRangeForYear(firstRecord.Date.Value, previousYear, description, result))
                {
                    if (previousYear == -1)
                        previousYear = -3;
                    else if (previousYear == -3)
                        previousYear = -5;
                    else
                        previousYear -= 5;

                    description = string.Format("{0} year", Math.Abs(previousYear));
                }
            }
            return result;
        }

        public IList<IndexedRangeData> BuildPerformanceLadders(UserAccountToken userToken, DateTime dtValuation)
        {
            logger.Log(LogLevel.Info, "building performance ladders");

            var historicalData = _historicalDataReader.GetHistoricalAccountData(userToken);

            var performanceRangeList = _DetermineIndexRanges(historicalData);
            //now retrieve all historical data ladders from the market data source 
            var allLadders = new List<IndexedRangeData>();
            foreach (var point in performanceRangeList)
            {
                logger.Log(LogLevel.Info, "building data ladder for {0}", point.Item2);

                //Console.WriteLine("building data ladder for {0}", perfPoint.Item2);
                var indexladder = _BuildIndexLadders(point.Item1, historicalData, userToken.Account);
                allLadders.Add(new IndexedRangeData
                {
                    MinValue = 0.8,
                    IsHistorical = true,
                    Name = point.Item2,
                    Data = indexladder
                });
            }

            logger.Log(LogLevel.Info, "performance data ladders complete...");

            return allLadders;
        }

        /// <summary>
        /// build company performance data ladder
        /// </summary>
        /// <param name="userToken"></param>
        /// <param name="dtValuation"></param>
        /// <returns></returns>
        public IndexedRangeData BuildCompanyPerformanceLadders(UserAccountToken userToken)
        {
            var dtValuation = _investmentRecordData.GetLatestRecordInvestmentValuationDate(userToken);
            if (dtValuation.HasValue == false)
                return null;

            logger.Log(LogLevel.Info, "building company performance data ladders");
            return new IndexedRangeData
            {
                MinValue = 0.0,
                IsHistorical = true,
                Name = "Companies",
                Data = _BuildCompanyIndexData(userToken, dtValuation.Value)
            };
        }

        public IndexedRangeData BuildAccountDividendPerformanceLadder(UserAccountToken userToken)
        {
            var dtValuation = _investmentRecordData.GetLatestRecordInvestmentValuationDate(userToken);
            if (dtValuation.HasValue == false)
                return null;

            logger.Log(LogLevel.Info, "building company dividend data ladders");

            return new IndexedRangeData
            {
                MinValue = 0,
                IsHistorical = false,
                KeyName = "Company",
                Name = "Dividends",
                Data = _BuildAccountDividendIndexData(userToken, dtValuation.Value)
            };
        }

        public IndexedRangeData BuildAccountDividendYieldPerformanceLadder(UserAccountToken userToken)
        {
            var dtValuation = _investmentRecordData.GetLatestRecordInvestmentValuationDate(userToken);
            if (dtValuation.HasValue == false)
                return null;

            logger.Log(LogLevel.Info, "building company dividend yield data ladders");

            return new IndexedRangeData
            {
                MinValue = 0,
                IsHistorical = false,
                KeyName = "Company",
                Name = "Average Yield",
                Data = _BuildAccountDividendYieldIndexData(userToken, dtValuation.Value)
            };
        }

        /// <summary>
        /// method filters out any olderr prices than start date,and rebases on first date
        /// </summary>
        /// <param name="dataList"></param>
        /// <param name="dtStartDate"></param>
        /// <returns></returns>
        private IEnumerable<HistoricalData> RebaseDataList(IEnumerable<HistoricalData> dataList, DateTime? dtStartDate)
        {
            var tmpList = dtStartDate.HasValue ? dataList.Where(x => x.Date >= dtStartDate).OrderBy(x => x.Date) :
                dataList.OrderBy(x => x.Date);
            var dFirstPrice = tmpList.First().Price;
            return tmpList.Select(x =>
            {
                return new HistoricalData
                {
                    Date = x.Date,
                    Price = 1 + ((x.Price - dFirstPrice) / dFirstPrice)
                };
            });

        }

        /// <summary>
        /// method retrieves the historical price information for all the configured indexes for all required
        /// date ranges. Data is then rebased for each date range to allow easy determination of relative performance
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="indexes"></param>
        /// <returns></returns>
        private IList<IndexData> _BuildIndexLadders(DateTime? startDate, IEnumerable<HistoricalData> historicalData, string account)
        {
            //first get club history
            //var clubData = _GetClubData().OrderBy(x => x.Date).ToList();
            var result = new List<IndexData>();

            var clubData = historicalData.OrderBy(x => x.Date).ToList();
            DumpData("club data", clubData);
            var rebasedClubData = RebaseDataList(clubData, startDate).ToList();

            DateTime dtFirstDate = rebasedClubData.First().Date.Value;

            result.Add(new IndexData
            {
                Name = account,
                StartDate = dtFirstDate,
                Data = rebasedClubData
            });

            _settings.ComparisonIndexes.ToList().ForEach(index =>
            {
                var indexedData = _marketDataSource.GetHistoricalData(index.Symbol, dtFirstDate).ToList();
                var rebasedIndexedData = RebaseDataList(indexedData, null).ToList();
                result.Add(new IndexData
                {
                    Name = index.Name,
                    StartDate = dtFirstDate,
                    Data = rebasedIndexedData
                });
            });

            return result;
        }

        private double _GetPerformanceAmount(CompanyData company)
        {
            company.NetSellingValue = company.Quantity * company.SharePrice;
            return (company.NetSellingValue + company.Dividend) / company.TotalCost;
        }

        private bool _IsPointLaterThan(IndexData index, DateTime dtDate)
        {
            if ((index.StartDate.Value.Year > dtDate.Year) ||
                ((index.StartDate.Value.Year == dtDate.Year) && (index.StartDate.Value.Month > dtDate.Month)))
            {
                return true;
            }
            return false;
        }

        private IList<IndexData> _BuildCompanyIndexData(UserAccountToken userToken, DateTime valuationDate)
        {
            var companies = _ClientRecordData.GetActiveCompanies(userToken, valuationDate).ToList();
            var investmentRecords = _investmentRecordData.GetFullInvestmentRecordData(userToken).ToList();
            var indexes = new List<IndexData>();
            foreach (var company in companies)
            {
                DateTime dtPrevious = new DateTime();
                var dataList = new List<HistoricalData>();
                var index = new IndexData { Name = company };
                var companyRecords = investmentRecords.Where(x => x.Name == company).OrderBy(x => x.ValuationDate).ToList();
                foreach (var investment in companyRecords)
                {
                    //where multiple records exist for a single month, just the the last one
                    if(dtPrevious.Year == investment.ValuationDate.Year && 
                        dtPrevious.Month == investment.ValuationDate.Month)
                    {
                        dataList.RemoveAt(dataList.Count - 1);
                    }

                    dtPrevious = investment.ValuationDate;
                    dataList.Add(new HistoricalData
                    {
                        Date = investment.ValuationDate.Date,
                        Price = _GetPerformanceAmount(investment)
                    });
                }
                if(dataList.Count > 0)
                {
                    index.StartDate = dataList.First().Date;
                    index.Data = dataList;
                    indexes.Add(index);
                }
            }
            //now get the oldest company index
            //and pad all other indexes with unit data points
            if (indexes.Count == 0)
                return indexes;

            var oldest = indexes.OrderBy(x => x.StartDate).First();
            foreach(var index in indexes)
            {
                if(_IsPointLaterThan(index, oldest.StartDate.Value) == true)
                {
                    foreach (var point in oldest.Data)
                    {
                        if (_IsPointLaterThan(index, point.Date.Value) == true)
                        {
                            index.Data.Insert(0, new HistoricalData
                            {
                                Date = point.Date,
                                Price = 1d
                            });
                        }
                        else
                            break;
                    }
                    index.StartDate = oldest.StartDate;
                }
            }

            foreach (var index in indexes)
            {
                index.Data = RebaseDataList(index.Data, index.StartDate).ToList();
            }

            return indexes;
        }

        private IList<IndexData> _BuildAccountDividendIndexData(UserAccountToken userToken, DateTime valuationDate)
        {
            var investmentRecords = _investmentRecordData.GetInvestmentRecordData(userToken, valuationDate);
            var indexes = new List<IndexData>();
            var index = new List<HistoricalData>();
            foreach (var record in investmentRecords)
            {
                index.Add(new HistoricalData
                {
                    Key = record.Name,
                    Price = record.Dividend
                });
            }

            indexes.Add(new IndexData
            {
                Name = "Dividends",
                Data = index
            });

            return indexes;
        }

        private IList<IndexData> _BuildAccountDividendYieldIndexData(UserAccountToken userToken, DateTime valuationDate)
        {
            var allRecords = _investmentRecordData.GetFullInvestmentRecordData(userToken).OrderBy(x => x.ValuationDate).ToList();
            var investmentRecords = _investmentRecordData.GetInvestmentRecordData(userToken, valuationDate);
            var indexes = new List<IndexData>();
            var index = new List<HistoricalData>();
            foreach (var record in investmentRecords)
            {
                var firstRecord = allRecords.FirstOrDefault(x => x.Name == record.Name);
                var span = record.ValuationDate - firstRecord.LastBrought;
                double dYears = span.TotalDays / 365d;
                double yield = (record.Dividend / record.TotalCost / dYears) * 100;
                index.Add(new HistoricalData
                {
                    Key = record.Name,
                    Price = yield
                });
            }

            indexes.Add(new IndexData
            {
                Name = "Average Yield",
                Data = index
            });

            return indexes;
        }
    }
}
