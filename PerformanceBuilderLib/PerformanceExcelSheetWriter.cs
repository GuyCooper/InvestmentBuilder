using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using ExcelAccountsManager;
using NLog;

namespace PerformanceBuilderLib
{
    class PerformanceExcelSheetWriter : IPerformanceDataWriter, IDisposable
    {
        private ExcelBookHolder _bookHolder;
        private Application _app;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public PerformanceExcelSheetWriter(string path, DateTime dtValuation)
        {
            _app = new Microsoft.Office.Interop.Excel.Application();
            var performanceBookName = string.Format(@"{0}\{1}-{2}.xlsx", path, ExcelBookHolder.PerformanceChartName, dtValuation.ToString("MMM-yyyy"));
            _bookHolder = new ExcelBookHolder(_app, performanceBookName);

        }

        public void WritePerformanceData(IList<IndexedRangeData> data)
        {
            Microsoft.Office.Interop.Excel._Worksheet perfsheet = _bookHolder.GetPerformanceBook().Worksheets[1];

            char startCol = 'B';
            int startRow = 1;
            foreach (var rangeIndex in data)
            {
                var currentCol = startCol;
                int currentRow = startRow;
 
                perfsheet.get_Range(currentCol++.ToString() + currentRow).Value = "date";
                foreach (var index in rangeIndex.Data)
                {
                    perfsheet.get_Range(currentCol++.ToString() + currentRow).Value = index.Name;
                }

                //now add the data
                //take the total numberof nodesin this index range from the first index
                //this should be the clubindex
                var clubIndex = rangeIndex.Data.First();
                int totalNodeCount = clubIndex.Data.Count();
                currentRow++;
                for (int i = 0; i < totalNodeCount; ++i)
                {
                    currentCol = startCol;
                    //first add the date
                    perfsheet.get_Range(currentCol++.ToString() + currentRow).Value = clubIndex.Data[i].Date;
                    //now add the data for each index (including  cub index)
                    foreach (var index in rangeIndex.Data)
                    {
                        var nextCell = perfsheet.get_Range(currentCol++.ToString() + currentRow);
                        nextCell.Value = index.Data.Count > i ? index.Data[i].Price : 0d;
                    }
                    currentRow++;
                }

                //now generate the chart and pivot table...
                var lstIndexes = rangeIndex.Data.Select(x => x.Name).ToList();
                //FirstRow = startRow, LastRow = currentRow - 1, FirstCol = chFirstCol, LastCol = (char)(chCol - 1)
                BuildPivotTableAndChart(lstIndexes, rangeIndex.Name, startCol, (char)(currentCol - 1),
                                        startRow, currentRow - 1, perfsheet);
                startCol = currentCol;
                startCol++;
            }

            _bookHolder.SaveBooks();
        }

        /// <summary>
        /// helper method for building pivot table andchart
        /// </summary>
        /// <param name="dateRange"></param>
        /// <param name="firstCol"></param>
        /// <param name="lastCol"></param>
        /// <param name="firstRow"></param>
        /// <param name="lastRow"></param>
        /// <param name="sourceSheet"></param>
        private void BuildPivotTableAndChart(IEnumerable<string> enIndexes, string dateRange, char firstCol, char lastCol, 
                                            int firstRow, int lastRow, _Worksheet sourceSheet)
        {
            logger.Log(LogLevel.Info, "building chart for {0}", dateRange);

            //add a pivot sheet for this date range 
            _bookHolder.GetPerformanceBook().Worksheets.Add();
            var pivotSheet = _bookHolder.GetPerformanceBook().Worksheets[1];
            pivotSheet.Name = dateRange;

            var firstCell = firstCol.ToString() + firstRow;
            var lastCell = lastCol.ToString() + lastRow;
            var sourceRange = sourceSheet.Range[firstCell, lastCell];

            var destinationFirstCell = pivotSheet.Range["B1"];
            var destinationLastCell = lastCol.ToString() + (1 + (lastRow - firstRow));
            var destinationRange = pivotSheet.Range["B1", destinationLastCell];

            //Macro for building pivot table and chart
            PivotCache pivotCache = _bookHolder.GetPerformanceBook().PivotCaches().Create(XlPivotTableSourceType.xlDatabase, sourceRange, XlPivotTableVersionList.xlPivotTableVersion14);
            PivotTable pivotTable = pivotCache.CreatePivotTable(destinationFirstCell, "Club Performance");// true, XlPivotTableVersionList.xlPivotTableVersion14);

            var newShape = pivotSheet.Shapes.AddChart(240, XlChartType.xlColumnClustered);
            var newChart = newShape.Chart;
            newChart.SetSourceData(destinationRange);

            pivotTable.PivotFields("Date").Orientation = XlPivotFieldOrientation.xlRowField;
            pivotTable.PivotFields("Date").Position = 1;

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
