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

    public class PerformanceBuilder 
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private IConfigurationSettings _settings;
        private IHistoricalDataReader _historicalDataReader;
        private IMarketDataSource _marketDataSource;

        public PerformanceBuilder(IConfigurationSettings settings, IDataLayer dataLayer, IMarketDataSource marketDataSource)
        {
            _historicalDataReader = dataLayer.HistoricalData;
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

        private bool _GetIndexRangeForYear(DateTime dtStartDate, int years , string description, IList<Tuple<DateTime?, string>> listIndexes)
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
            if(firstRecord != null)
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

        public IList<IndexedRangeData> Run(UserAccountToken userToken, DateTime dtValuation)
        {
            logger.Log(LogLevel.Info, "starting performance builder...");
            logger.Log(LogLevel.Info, "output path: {0}", _settings.GetOutputPath(userToken.Account));
            logger.Log(LogLevel.Info, "valuation date {0}", dtValuation);

            var historicalData = _historicalDataReader.GetHistoricalAccountData(userToken);

            //Console.WriteLine("starting performance builder...");
            //compre performance to FTSE100 and S&p 500
            //var listIndexes = new List<Tuple<string, string>> { new Tuple<string, string>("^FTSE", "FTSE 100"),
            //                                                    new Tuple<string, string>("^GSPC","S&P 500") };

            var performanceRangeList = _DetermineIndexRanges(historicalData);
            //now retrieve all historical data ladders from the market data source 
            var allLadders = new List<IndexedRangeData>();
            foreach(var point in performanceRangeList)
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

            logger.Log(LogLevel.Info, "data ladders complete...");

            //now persist it to the spreadsheet
            using(var dataWriter = new PerformanceExcelSheetWriter(_settings.GetOutputPath(userToken.Account), dtValuation))
            {
                dataWriter.WritePerformanceData(allLadders);
            }

            logger.Log(LogLevel.Info, "performance chartbuilder complete");
            return allLadders;
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
    }
}

    //Macro for building pivor table and chart
    // Sheets.Add
    //ActiveWorkbook.PivotCaches.Create(SourceType:=xlDatabase, SourceData:= _
    //    "Sheet1!R1C2:R67C4", Version:=xlPivotTableVersion14).CreatePivotTable _
    //    TableDestination:="Sheet4!R1C1", TableName:="PivotTable1", DefaultVersion _
    //    :=xlPivotTableVersion14
    //Sheets("Sheet4").Select
    //Cells(1, 1).Select
    //ActiveSheet.Shapes.AddChart.Select
    //ActiveChart.ChartType = xlColumnClustered
    //ActiveChart.SetSourceData Source:=Range("Sheet4!$A$1:$C$18")
    //ActiveSheet.Shapes("Chart 1").IncrementLeft 192
    //ActiveSheet.Shapes("Chart 1").IncrementTop 15
    //With ActiveSheet.PivotTables("PivotTable1").PivotFields("Date")
    //    .Orientation = xlRowField
    //    .Position = 1
    //End With
    //ActiveSheet.PivotTables("PivotTable1").AddDataField ActiveSheet.PivotTables( _
    //    "PivotTable1").PivotFields("FTSE Price"), "Sum of FTSE Price", xlSum
    //ActiveSheet.PivotTables("PivotTable1").AddDataField ActiveSheet.PivotTables( _
    //    "PivotTable1").PivotFields("Club Price"), "Sum of Club Price", xlSum
    //ActiveChart.ChartType = xlLine
    //ActiveSheet.Shapes("Chart 1").IncrementLeft -271.5
    //ActiveSheet.Shapes("Chart 1").IncrementTop 24
    //ActiveSheet.Shapes("Chart 1").ScaleWidth 1.8979166667, msoFalse, _
    //    msoScaleFromTopLeft
    //ActiveSheet.Shapes("Chart 1").ScaleHeight 1.5902777778, msoFalse, _
    //    msoScaleFromTopLeft
    //ActiveChart.Axes(xlValue).Select
    //ActiveChart.Axes(xlValue).MinimumScale = 0
    //ActiveChart.Axes(xlValue).MinimumScale = 0.8

/*
 *     Range("B1").Select
    Sheets.Add
    ActiveWorkbook.PivotCaches.Create(SourceType:=xlDatabase, SourceData:= _
        "Sheet1!R1C2:R73C5", Version:=xlPivotTableVersion14).CreatePivotTable _
        TableDestination:="Sheet4!R3C1", TableName:="PivotTable1", DefaultVersion _
        :=xlPivotTableVersion14
    Sheets("Sheet4").Select
    Cells(3, 1).Select
    ActiveSheet.PivotTables("PivotTable1").AddDataField ActiveSheet.PivotTables( _
        "PivotTable1").PivotFields("S&P 500"), "Sum of S&P 500", xlSum
    ActiveSheet.PivotTables("PivotTable1").AddDataField ActiveSheet.PivotTables( _
        "PivotTable1").PivotFields("FTSE 100"), "Sum of FTSE 100", xlSum
    ActiveSheet.PivotTables("PivotTable1").AddDataField ActiveSheet.PivotTables( _
        "PivotTable1").PivotFields("Club Price"), "Sum of Club Price", xlSum
    With ActiveSheet.PivotTables("PivotTable1").PivotFields("Date")
        .Orientation = xlRowField
        .Position = 1
    End With
    ActiveSheet.Shapes.AddChart.Select
    ActiveChart.ChartType = xlLineMarkers
    ActiveChart.SetSourceData Source:=Range("Sheet4!$A$3:$D$76")
    ActiveSheet.Shapes("Chart 1").ScaleWidth 1.5708333333, msoFalse, _
        msoScaleFromTopLeft
    ActiveSheet.Shapes("Chart 1").ScaleHeight 0.88715296, msoFalse, _
        msoScaleFromTopLeft

*/