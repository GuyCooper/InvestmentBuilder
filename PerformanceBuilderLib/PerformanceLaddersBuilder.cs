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
    public class IndexedRangeData
    {
        public string Name { get; set; }
        public IList<IndexData> Data { get; set; }
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
            if (firstRecord != null)
            {
                //add all time range
                result.Add(new Tuple<DateTime?, string>(null, "All Time"));
                int previousYear = -1;
                string description = "1 year";
                while (_GetIndexRangeForYear(firstRecord.Date, previousYear, description, result))
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
        public IndexedRangeData BuildCompanyPerformanceLadders(UserAccountToken userToken, DateTime dtValuation)
        {
            logger.Log(LogLevel.Info, "building company performance data ladders");
            return new IndexedRangeData
            {
                Name = "Companies",
                Data = _BuildCompanyIndexData(userToken, dtValuation)
            };
        }

        public IndexedRangeData BuildAccountDividendPerformanceLadder(UserAccountToken userToken, DateTime dtValuation)
        {
            return new IndexedRangeData
            {
                Name = "Dividends",
                Data = _BuildAccountDividendIndexData(userToken, dtValuation)
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

            DateTime dtFirstDate = rebasedClubData.First().Date;

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

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="company"></param>
        ///// <returns></returns>
 
        ///// <summary>
        ///// returns a breakdown of performance data for each active company in the account. this allows
        ///// the user to compare performance on a per company data
        ///// </summary>
        ///// <param name="account"></param>
        ///// <returns></returns>
        //private IList<IndexData> _BuildCompanyIndexData(UserAccountToken userToken, DateTime valuationDate)
        //{
        //    logger.Log(LogLevel.Info, "building performance ladders for individual companies");
        //    var indexDataList = new List<IndexData>();
        //    var companies = _ClientRecordData.GetActiveCompanies(userToken, valuationDate);
        //    foreach (var company in companies)
        //    {
        //        var index = new IndexData { Name = company };
        //        var dataList = new List<HistoricalData> { };
        //        var investmentRecords = _investmentRecordData.GetCompanyRecordData(userToken, company).OrderBy(x => x.ValuationDate);
        //        //get the latest investment record for each month
        //        DateTime dtPrevious = new DateTime();
        //        foreach (var record in investmentRecords)
        //        {
        //            if((record.ValuationDate.Year == dtPrevious.Year)&&(record.ValuationDate.Month == dtPrevious.Month))
        //            {
        //                //multiple records for same month, use the latest   
        //                var previousRecord = dataList.FirstOrDefault(x => x.Date == dtPrevious);
        //                if(previousRecord != null)
        //                {
        //                    dataList.Remove(previousRecord);
        //                }
        //            }

        //            dataList.Add(new HistoricalData
        //            {
        //                Date = record.ValuationDate,
        //                Price = _GetCompanyPerformanceAmount(record)
        //            });
        //            dtPrevious = record.ValuationDate;
        //        }

        //        if (dataList.Count > 0)
        //        {
        //            indexDataList.Add(new IndexData
        //            {
        //                Data = dataList,
        //                StartDate = dataList.First().Date,
        //                Name = company
        //            });
        //        }
        //    }

        //    //each company chart must contain the same number of date points, so normalise
        //    //them here
        //    var oldest = indexDataList.OrderBy(x => x.StartDate).First();
        //    foreach (var item in oldest.Data)
        //    {
        //        foreach (var other in indexDataList.Where(x => x.Name != oldest.Name))
        //        {
        //            if (other.Data.Count < oldest.Data.Count)
        //            {
        //                if (other.StartDate > item.Date)
        //                {
        //                    other.Data.Insert(0, new HistoricalData
        //                    {
        //                        Date = item.Date,
        //                        Price = 1d
        //                    });
        //                    other.StartDate = item.Date;
        //                }
        //            }
        //        }
        //    }

        //    foreach(var index in indexDataList)
        //    {
        //        index.Data = RebaseDataList(index.Data, index.StartDate).ToList();
        //    }

        //    return indexDataList;
        //}

        private double _GetPerformanceAmount(CompanyData company)
        {
            company.NetSellingValue = company.Quantity * company.SharePrice;
            return (company.NetSellingValue + company.Dividend) / company.TotalCost;
        }

        private IList<IndexData> _BuildCompanyIndexData(UserAccountToken userToken, DateTime valuationDate)
        {
            var companies = _ClientRecordData.GetActiveCompanies(userToken, valuationDate);
            var investmentRecords = _investmentRecordData.GetFullInvestmentRecordData(userToken).OrderBy(x => x.ValuationDate);
            var indexes = new List<IndexData>();
            foreach (var company in companies)
            {
                DateTime dtPrevious = new DateTime();
                var dataList = new List<HistoricalData>();
                var index = new IndexData { Name = company };
                foreach (var investment in investmentRecords.Where(x => x.Name == company))
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
                        Date = investment.ValuationDate,
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
            var oldest = indexes.OrderBy(x => x.StartDate).First();
            foreach(var point in oldest.Data)
            {
                foreach(var index in indexes)
                {
                    if(index.StartDate.Year >= point.Date.Year &&
                                           index.StartDate.Month > point.Date.Month)
                    {
                        index.Data.Insert(0, new HistoricalData
                        {
                            Date = point.Date,
                            Price = 1d
                        });
                        index.StartDate = point.Date;
                    }
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
            //var investmentRecords = _investmentRecordData.GetFullInvestmentRecordData(userToken).GroupBy(x => x.ValuationDate);
            //foreach(var record in investmentRecords)
            //{
            //   record.Sum( x=> (x.SharePrice * x.Quantity))
            //}
            return null;
        }
    }
}
