using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using ExcelAccountsManager;
using MarketDataServices;
using NLog;

namespace PerformanceBuilderLib
{
    struct RangeDimensions
    {
        public int FirstRow;
        public int LastRow;
        public char FirstCol;
        public char LastCol;
    }

    public static class PerformanceBuilderExternal
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static void RunBuilder(string account, string path, string dataSource, DateTime dtValuation )
        {
            logger.Log(LogLevel.Info, "running performance chartbuilder...");
            logger.Log(LogLevel.Info, string.Format("output folder {0}", path));
            logger.Log(LogLevel.Info, string.Format("datasource: {0}", dataSource));
            logger.Log(LogLevel.Info, string.Format("valuation date: {0}", dtValuation));
            //Console.WriteLine("running performance chartbuilder...");
            //Console.WriteLine("output folder {0}", path);
            //Console.WriteLine("datasource: {0}", dataSource);
            //Console.WriteLine("valuation date: {0}", dtValuation);

            using(var builder = new PerformanceBuilder(account, path, dataSource, dtValuation))
            {
                builder.Run();
            }
        }
    }

    internal class PerformanceBuilder : IDisposable
    {
        private ExcelBookHolder _bookHolder;
        private IEnumerable<HistoricalData> _historicalData;
        private Application _app;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        private string performanceBookName;

        public PerformanceBuilder(string account, string path, string datasource, DateTime dtValuation)
        {
             _app = new Microsoft.Office.Interop.Excel.Application();

             performanceBookName = string.Format(@"{0}\{1}-{2}.xlsx", path, ExcelBookHolder.PerformanceChartName, dtValuation.ToString("MMM-yyyy"));
             _bookHolder = new ExcelBookHolder(_app, performanceBookName);
            using (var reader = new HistoricalDataReader(datasource))
            {
                _historicalData = reader.GetClubData(account).ToList();
            }
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

        public void Run()
        {
            logger.Log(LogLevel.Info, "starting performance builder...");
            //Console.WriteLine("starting performance builder...");
            //compre performance to FTSE100 and S&p 500
            var listIndexes = new List<Tuple<string, string>> { new Tuple<string, string>("^FTSE", "FTSE 100"),
                                                                new Tuple<string, string>("^GSPC","S&P 500") };

            //benchmark for 1 year, 3 year and all time
            var performanceRangeList = new List<Tuple<DateTime?, string>>{ new Tuple<DateTime?, string>(null, "All Time"),
                                                                           new Tuple<DateTime?,string>(DateTime.Today.AddYears(-5), "5 Year"),
                                                                           new Tuple<DateTime?,string>(DateTime.Today.AddYears(-3), "3 Year"),
                                                                           new Tuple<DateTime?, string>( DateTime.Today.AddYears(-1), "1 Year")};

            char startCol = 'B';
            var lstRanges = new List<Tuple<string, RangeDimensions>>();
            foreach(var perfPoint in performanceRangeList)
            {
                logger.Log(LogLevel.Info, "building data ladder for {0}", perfPoint.Item2);
                //Console.WriteLine("building data ladder for {0}", perfPoint.Item2);
                var dimensions = BuildDataLadder(perfPoint.Item1, listIndexes, 1, startCol);
                lstRanges.Add(new Tuple<string, RangeDimensions>(perfPoint.Item2, dimensions));
                startCol = dimensions.LastCol;
                startCol++;
                startCol++;
            }

            _Worksheet sourceSheet = _bookHolder.GetPerformanceBook().Worksheets[1];
            //each performance rabge will be added to a new sheet
            foreach(var perfRange in lstRanges )
            {
                logger.Log(LogLevel.Info, "building chart for {0}", perfRange.Item1);
                //Console.WriteLine("building chart for {0}", perfRange.Item1);
                _bookHolder.GetPerformanceBook().Worksheets.Add();
                var targetSheet = _bookHolder.GetPerformanceBook().Worksheets[1];
                targetSheet.Name = perfRange.Item1;
                //app.ActiveSheet.Name = perfRange.Item1;
                //BuildPivotTableAndChart(listIndexes.Select(x => x.Item2), perfRange.Item2, app,sourceSheet, app.ActiveSheet);
                BuildPivotTableAndChart(listIndexes.Select(x => x.Item2), perfRange.Item2, sourceSheet, targetSheet);
            }

            _bookHolder.SaveBooks();

            logger.Log(LogLevel.Info, "performance chartbuilder complete, performance chart location: {0}", performanceBookName);
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

        private RangeDimensions BuildDataLadder(DateTime? startDate, IEnumerable<Tuple<string, string>> indexes, int startRow, char startCol)
        {
            //first get club history
            //var clubData = _GetClubData().OrderBy(x => x.Date).ToList();
            var clubData = _historicalData.OrderBy(x => x.Date).ToList();
            DumpData("club data", clubData);

            var rebasedClubData = RebaseDataList(clubData, startDate).ToList();

            int currentRow = startRow;
            DateTime dtFirstDate = rebasedClubData.First().Date;
            var indexResults = indexes.Select( index =>
                {
                    var indexedData = MarketDataService.GetHistoricalData(index.Item1, dtFirstDate);
                    var rebasedIndexedData = RebaseDataList(indexedData, null).ToList();
                    return new { Name = index.Item2, Index = rebasedIndexedData};
                }).ToList();

            
            //now add the data to a new performance sheet
            _Worksheet perfSheet = _bookHolder.GetPerformanceBook().Worksheets[1];

            var chFirstCol = startCol;
            var chCol = chFirstCol;
            //add the headers in the result sheet
            perfSheet.get_Range(chCol++.ToString() + currentRow).Value = "Date";
            var clubPriceCell = perfSheet.get_Range(chCol++.ToString() + currentRow);
            clubPriceCell.Value = "Club Price";
            foreach (var indexPoint in indexResults)
            {
                var nextCell = perfSheet.get_Range(chCol++.ToString() + currentRow);
                nextCell.Value = indexPoint.Name;
            }

            //now add the data
            currentRow++;
            for (int i = 0; i < rebasedClubData.Count; ++i )
            {
                chCol = chFirstCol;
                perfSheet.get_Range(chCol++.ToString() + currentRow).Value = rebasedClubData[i].Date;
                clubPriceCell = perfSheet.get_Range(chCol++.ToString() + currentRow);
                clubPriceCell.Value = rebasedClubData[i].Price;
                foreach (var indexPoint in indexResults)
                {
                    var nextCell = perfSheet.get_Range(chCol++.ToString() + currentRow);
                    nextCell.Value = indexPoint.Index.Count > i ? indexPoint.Index[i].Price : 0d;
                }
                currentRow++;
            }

            return new RangeDimensions { FirstRow = startRow, LastRow = currentRow - 1, FirstCol = chFirstCol, LastCol = (char)(chCol - 1) };
        }

        private void BuildPivotTableAndChart(IEnumerable<string> enIndexes, RangeDimensions dimensions, _Worksheet sourceSheet, _Worksheet pivotSheet)
        {
            var perfBook = _bookHolder.GetPerformanceBook();
            
            var firstCell = dimensions.FirstCol.ToString() + dimensions.FirstRow;
            var lastCell = dimensions.LastCol.ToString() + dimensions.LastRow;
            var sourceRange = sourceSheet.Range[firstCell, lastCell];

            var destinationFirstCell = pivotSheet.Range["B1"];
            var destinationLastCell = dimensions.LastCol.ToString() + (1 + (dimensions.LastRow - dimensions.FirstRow));
            var destinationRange = pivotSheet.Range["B1", destinationLastCell];

            //var destinationRange = pivotSheet.Range["B1", "D" + (count - 1)];
            //Macro for building pivor table and chart
            PivotCache pivotCache = perfBook.PivotCaches().Create(XlPivotTableSourceType.xlDatabase, sourceRange, XlPivotTableVersionList.xlPivotTableVersion14);
            PivotTable pivotTable = pivotCache.CreatePivotTable(destinationFirstCell, "Club Performance");// true, XlPivotTableVersionList.xlPivotTableVersion14);

            //pivotSheet.Select();
            //app.ActiveSheet.Shapes.AddChart(240, XlChartType.xlColumnClustered).Select();
            //app.ActiveChart.SetSourceData(destinationRange);

            //pivotSheet.Shapes.AddChart(240, XlChartType.xlColumnClustered).Select();
            var newShape = pivotSheet.Shapes.AddChart(240, XlChartType.xlColumnClustered);
            var newChart = newShape.Chart;
            newChart.SetSourceData(destinationRange);

            pivotTable.PivotFields("Date").Orientation = XlPivotFieldOrientation.xlRowField;
            pivotTable.PivotFields("Date").Position = 1;

            pivotTable.AddDataField(pivotTable.PivotFields("Club Price"), "Sum Of Club Price");
            foreach (var index in enIndexes)
            {
                pivotTable.AddDataField(pivotTable.PivotFields(index), string.Format("Sum Of {0}", index));
            }

            newChart.ChartType = XlChartType.xlLineMarkers;
            newShape.ScaleWidth(1.8979166667f, Microsoft.Office.Core.MsoTriState.msoFalse);
            newShape.ScaleHeight(1.5902777778f, Microsoft.Office.Core.MsoTriState.msoFalse);

            newChart.Axes(XlAxisType.xlValue).MinimumScale = 0.8;
        }

        public void Dispose()
        {
            _bookHolder.Dispose();
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