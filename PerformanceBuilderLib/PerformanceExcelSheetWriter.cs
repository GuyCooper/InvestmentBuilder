using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using NLog;
using System.IO;
using InvestmentBuilderCore;

namespace PerformanceBuilderLib
{
    class PerformanceExcelSheetWriter : IPerformanceDataWriter, IDisposable
    {
        private Application _app;
        private _Workbook _performanceBook;

        private const string PerformanceChartName = "Performance Chart";
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public PerformanceExcelSheetWriter(string path, DateTime dtValuation)
        {
            _app = new Microsoft.Office.Interop.Excel.Application();
            var performanceBookName = string.Format(@"{0}\{1}-{2}.xlsx", path, PerformanceChartName, dtValuation.ToString("MMM-yyyy"));
            File.Delete(performanceBookName);
            _performanceBook = _app.Workbooks.Add();
            _performanceBook.SaveAs(performanceBookName);
        }

        private string _GetCurrentColumnString(char col1, char? col2)
        {
            return col2.HasValue ? string.Format("{0}{1}", col1, col2) : col1.ToString();
        }

        private string _GetNextColumnString(ref char col1, ref char? col2)
        {
            var strNextCol = _GetCurrentColumnString(col1, col2);
            if (col1 == 'Z')
            {
                if(col2.HasValue == false)
                {
                    col1 = 'A';
                    col2 = 'A';
                }
            }
            else if(col2.HasValue)
            {
                if (col2 == 'Z')
                {
                    col1++;
                    col2 = 'A';
                }
                else
                {
                    col2++;
                }
            }
            else
            {
                col1++;
            }
            return strNextCol;
        }

        public void WritePerformanceData(IList<IndexedRangeData> data)
        {
            _Worksheet perfsheet = _performanceBook.Worksheets[1];

            char startColA = 'B';
            char? startColB = null;
            
            int startRow = 1;
            foreach (var rangeIndex in data)
            {
                var currentCol1 = startColA;
                char? currentCol2 = startColB;

                int currentRow = startRow;

                string keyName = rangeIndex.IsHistorical ? "date" : rangeIndex.KeyName;
                perfsheet.get_Range(_GetNextColumnString(ref currentCol1, ref currentCol2) + currentRow).Value = keyName;

                foreach (var index in rangeIndex.Data)
                {
                    perfsheet.get_Range(_GetNextColumnString(ref currentCol1, ref currentCol2) + currentRow).Value = index.Name;
                }

                //now add the data
                //take the total numberof nodesin this index range from the first index
                //this should be the clubindex
                var clubIndex = rangeIndex.Data.First();
                int totalNodeCount = clubIndex.Data.Count();
                currentRow++;
                char previousCol1 = startColA;
                char? previousCol2 = startColB;

                for (int i = 0; i < totalNodeCount; ++i)
                {
                    currentCol1 = startColA;
                    currentCol2 = startColB;
        
                    //first add the key column (date if historical data)
                    if (rangeIndex.IsHistorical == true)
                    {
                        perfsheet.get_Range(_GetNextColumnString(ref currentCol1, ref currentCol2) + currentRow).Value = clubIndex.Data[i].Date.Value;
                    }
                    else
                    {
                        perfsheet.get_Range(_GetNextColumnString(ref currentCol1, ref currentCol2) + currentRow).Value = clubIndex.Data[i].Key;
                    }

                    //now add the data for each index (including  cub index)
                    foreach (var index in rangeIndex.Data)
                    {
                        previousCol1 = currentCol1;
                        previousCol2 = currentCol2;
                        var nextCell = perfsheet.get_Range(_GetNextColumnString(ref currentCol1, ref currentCol2) + currentRow);
                        nextCell.Value = index.Data.Count > i ? index.Data[i].Price : 0d;
                    }
                    currentRow++;
                }

                //now generate the chart and pivot table...
                var lstIndexes = rangeIndex.Data.Select(x => x.Name).ToList();
                //FirstRow = startRow, LastRow = currentRow - 1, FirstCol = chFirstCol, LastCol = (char)(chCol - 1)
                BuildPivotTableAndChart(lstIndexes, rangeIndex.Name, keyName, _GetCurrentColumnString(startColA, startColB), _GetCurrentColumnString(previousCol1, previousCol2),
                                        startRow, currentRow - 1, perfsheet, rangeIndex.IsHistorical, rangeIndex.MinValue);
                startColA = currentCol1;
                startColB = currentCol2;
                _GetNextColumnString(ref startColA, ref startColB);
            }

            logger.Log(LogLevel.Info, "saving performance chart to sheet {0}",
                                _performanceBook.FullName);

            _performanceBook.Save();
        }

        /// <summary>
        /// helper method for building pivot table andchart
        /// </summary>
        /// <param name="key"></param>
        /// <param name="firstCol"></param>
        /// <param name="lastCol"></param>
        /// <param name="firstRow"></param>
        /// <param name="lastRow"></param>
        /// <param name="sourceSheet"></param>
        private void BuildPivotTableAndChart(IEnumerable<string> enIndexes, string indexName, string key, string firstCol, string lastCol, 
                                            int firstRow, int lastRow, _Worksheet sourceSheet, bool IsHistorical, double minValue)
        {
            logger.Log(LogLevel.Info, "building chart for {0}", indexName);

            //add a pivot sheet for this date range 
            _performanceBook.Worksheets.Add();
            var pivotSheet = _performanceBook.Worksheets[1];
            pivotSheet.Name = indexName;

            var firstCell = firstCol + firstRow;
            var lastCell = lastCol + lastRow;
            var sourceRange = sourceSheet.Range[firstCell, lastCell];

            var destinationFirstCell = pivotSheet.Range["B1"];
            var destinationLastCell = lastCol + (1 + (lastRow - firstRow));
            var destinationRange = pivotSheet.Range["B1", destinationLastCell];

            //Macro for building pivot table and chart
            PivotCache pivotCache = _performanceBook.PivotCaches().Create(XlPivotTableSourceType.xlDatabase, sourceRange, XlPivotTableVersionList.xlPivotTableVersion14);
            PivotTable pivotTable = pivotCache.CreatePivotTable(destinationFirstCell, "Club Performance");// true, XlPivotTableVersionList.xlPivotTableVersion14);

            var newShape = pivotSheet.Shapes.AddChart(240, XlChartType.xlColumnClustered);
            var newChart = newShape.Chart;
            newChart.SetSourceData(destinationRange);

            if (IsHistorical == true)
            {
                pivotTable.PivotFields("Date").Orientation = XlPivotFieldOrientation.xlRowField;
                pivotTable.PivotFields("Date").Position = 1;
            }
            else
            {
                pivotTable.PivotFields(key).Orientation = XlPivotFieldOrientation.xlRowField;
                pivotTable.PivotFields(key).Position = 1;
            }

            foreach (var index in enIndexes)
            {
                pivotTable.AddDataField(pivotTable.PivotFields(index), string.Format("Sum Of {0}", index));
            }

            if (IsHistorical == true)
            {
                newChart.ChartType = XlChartType.xlLineMarkers;
                newShape.ScaleWidth(1.2075f, Microsoft.Office.Core.MsoTriState.msoFalse);
                newShape.ScaleHeight(1.2208333333f, Microsoft.Office.Core.MsoTriState.msoFalse);
            }
            else
            {
                newShape.ScaleWidth(1.8979166667f, Microsoft.Office.Core.MsoTriState.msoFalse);
                newShape.ScaleHeight(1.5902777778f, Microsoft.Office.Core.MsoTriState.msoFalse);
            }

            //newChart.ChartTitle.Text = key;
            newChart.Axes(XlAxisType.xlValue).MinimumScale = minValue;
        }

        public void Dispose()
        {
            _performanceBook.Close();
        }
    }
}
