using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderCore;
using Microsoft.Office.Interop.Excel;
using System.IO;
using NLog;

namespace InvestmentReportGenerator
{
    /// <summary>
    /// excel implementation of the report writer. has 2 methods one to write the
    /// asset statement report and one to write the performance report. Has different
    /// file locations for both the reports
    /// </summary>
    public class ExcelInvestmentReportWriter : IInvestmentReportWriter, IDisposable
    {
        _Application _app;
        //private _Workbook _assetBook;
        //private _Workbook _templateBook;
        private string _templateFileName;

        public const string MonthlyAssetName = "Monthly Assets Statement";
        public const string TemplateBookName = "template.xls";
        private const string PerformanceChartName = "Performance Chart";

        private static Logger logger = LogManager.GetCurrentClassLogger();

        private string _assetSheetLocation;

        public ExcelInvestmentReportWriter(string templateBookLocation)
        {
            _app = new Microsoft.Office.Interop.Excel.Application();
            //open the template book
            _templateFileName = Path.Combine(templateBookLocation, TemplateBookName);
        }

        public string GetReportFileName(DateTime ValuationDate)
        {
            return $"{MonthlyAssetName}-{ValuationDate.ToString("yyyy")}.xls";
        }

        /// <summary>
        /// write the asset report to an excel spreadsheet. the output location
        /// is defined for the account for whihc the report is being written
        /// </summary>
        /// <param name="report"></param>
        /// <param name="startOfYear"></param>
        /// <param name="outputPath"></param>
        public void WriteAssetReport(AssetReport report, double startOfYear, string outputPath, ProgressCounter progress)
        {
            logger.Log(LogLevel.Info, "persisting asset report to excel spreadsheet in location {0}", outputPath);

            var lstCompanyData = report.Assets.ToList();

            progress.Initialise("writing excel asset report", lstCompanyData.Count + 6);
            _assetSheetLocation = Path.Combine(outputPath,GetReportFileName(report.ValuationDate));
            //if the asset sheet already exists,just open it,otherwise create a new one
            _Workbook assetBook = null;
            _Workbook templateBook = _app.Workbooks.Open(_templateFileName);

            progress.Increment();
            try
            {
                if (File.Exists(_assetSheetLocation))
                {
                    assetBook = _app.Workbooks.Open(_assetSheetLocation);
                }
                else
                {
                    assetBook = _app.Workbooks.Add();
                    assetBook.SaveAs(_assetSheetLocation, XlFileFormat.xlWorkbookNormal);
                }

                progress.Increment();

                var newSheetName = report.ValuationDate.ToString("MMMM");
                //if this sheet already exists then delete it
                //_DeleteExistingSheet(newSheetName);
                if (_SheetExists(newSheetName, assetBook))
                {
                    logger.Log(LogLevel.Warn, "sheet {0} already exists in asset book", newSheetName);
                    return;
                }

                _Worksheet templateSheet = templateBook.Worksheets["Assets"];
                templateSheet.Copy(assetBook.Worksheets[1]);

                _Worksheet newSheet = assetBook.Worksheets[1];

                newSheet.EnableCalculation = true;
                newSheet.Name = newSheetName;

                newSheet.get_Range("A1").Value = report.AccountName;
                newSheet.get_Range("C4").Value = report.ValuationDate;

                newSheet.get_Range("B16").Value = report.ReportingCurrency;

                progress.Increment();

                //add in redemptions
                if (report.Redemptions != null)
                {
                    newSheet.get_Range("B18").Font.Bold = true;
                    newSheet.get_Range("B18").Value = "Redemptions";
                    newSheet.get_Range("B19").Value = "Date";
                    newSheet.get_Range("C19").Value = "User";
                    newSheet.get_Range("D19").Value = "Amount";

                    int redemptionRow = 20;
                    foreach (var redemption in report.Redemptions)
                    {
                        newSheet.get_Range("B" + redemptionRow).Value = redemption.TransactionDate;
                        newSheet.get_Range("C" + redemptionRow).Value = redemption.User;
                        newSheet.get_Range("D" + redemptionRow).Value = redemption.Amount;
                        redemptionRow++;
                    }
                }

                //add in the new rows
                progress.Increment();

                for (int row = 1; row < lstCompanyData.Count; ++row)
                {
                    Range rowToCopy = newSheet.get_Range("A7", "A7").EntireRow;
                    rowToCopy.Copy(); //put row onto clipboard
                    rowToCopy.Insert(XlInsertShiftDirection.xlShiftDown);
                    newSheet.get_Range("A7", "A7").EntireRow.PasteSpecial(); //paste format into new row
                }

                progress.Increment();

                int count = 7;
                //now add the company data
                foreach (var company in lstCompanyData)
                {
                    logger.Log(LogLevel.Info, string.Format("Adding {0} to asset sheet", company.Name));
                    //Console.WriteLine("Adding {0} to asset sheet", company.sName);

                    newSheet.get_Range("B" + count).Value = company.Name;
                    newSheet.get_Range("C" + count).Value = company.LastBrought;
                    newSheet.get_Range("D" + count).Value = company.Quantity;
                    newSheet.get_Range("E" + count).Value = company.AveragePricePaid;
                    newSheet.get_Range("F" + count).Value = company.TotalCost;
                    newSheet.get_Range("G" + count).Value = company.SharePrice;
                    newSheet.get_Range("H" + count).Value = company.NetSellingValue;
                    newSheet.get_Range("I" + count).Value = company.ProfitLoss;
                    newSheet.get_Range("J" + count).Value = company.MonthChange;
                    newSheet.get_Range("K" + count).Value = company.MonthChangeRatio;
                    newSheet.get_Range("L" + count).Value = company.Dividend;
                    count++;
                    progress.Increment();
                }

                newSheet.get_Range("H" + count).Value = report.TotalAssetValue;
                newSheet.get_Range("J" + count).Value = lstCompanyData.Sum(a => a.MonthChange);
                newSheet.get_Range("L" + count).Value = lstCompanyData.Sum(a => a.Dividend);
                count++;
                newSheet.get_Range("H" + count++).Value = report.BankBalance;
                count += 2;
                newSheet.get_Range("C" + count).Value = startOfYear;
                newSheet.get_Range("H" + count++).Value = report.TotalAssets;

                newSheet.get_Range("H" + count++).Value = report.TotalLiabilities;
                count++;
                newSheet.get_Range("H" + count++).Value = report.NetAssets;
                newSheet.get_Range("H" + count++).Value = report.IssuedUnits;
                newSheet.get_Range("H" + count++).Value = report.ValuePerUnit;

                logger.Log(LogLevel.Info, "saving asset sheet: {0}", assetBook.FullName);
                assetBook.Save();
            }
            finally
            {
                assetBook.Close();
                templateBook.Close();
                progress.Increment();
            }
        }

        /// <summary>
        /// method writes the performane report to an excel spreadsheet
        /// </summary>
        /// <param name="data"></param>
        /// <param name="path"></param>
        /// <param name="dtValuation"></param>
        public void WritePerformanceData(IList<IndexedRangeData> data, string outputPath, DateTime dtValuation, ProgressCounter progress)
        {
            //_Workbook performanceBook = null;
            //try
            //{
            //    var performanceBookName = string.Format(@"{0}\{1}-{2}.xlsx", outputPath, PerformanceChartName, dtValuation.ToString("MMM-yyyy"));
            //    File.Delete(performanceBookName);
            //    performanceBook = _app.Workbooks.Add();
            //    performanceBook.SaveAs(performanceBookName);

            //    _Worksheet perfsheet = performanceBook.Worksheets[1];

            //    char startColA = 'B';
            //    char? startColB = null;

            //    int startRow = 1;
            //    foreach (var rangeIndex in data)
            //    {
            //        var currentCol1 = startColA;
            //        char? currentCol2 = startColB;

            //        int currentRow = startRow;

            //        string keyName = rangeIndex.IsHistorical ? "date" : rangeIndex.KeyName;
            //        perfsheet.get_Range(_GetNextColumnString(ref currentCol1, ref currentCol2) + currentRow).Value = keyName;

            //        foreach (var index in rangeIndex.Data)
            //        {
            //            perfsheet.get_Range(_GetNextColumnString(ref currentCol1, ref currentCol2) + currentRow).Value = index.Name;
            //        }

            //        //now add the data
            //        //take the total numberof nodesin this index range from the first index
            //        //this should be the clubindex
            //        var clubIndex = rangeIndex.Data.First();
            //        int totalNodeCount = clubIndex.Data.Count();
            //        currentRow++;
            //        char previousCol1 = startColA;
            //        char? previousCol2 = startColB;

            //        progress.Initialise("writing performance data to excel report", totalNodeCount + 1);
            //        for (int i = 0; i < totalNodeCount; ++i)
            //        {
            //            currentCol1 = startColA;
            //            currentCol2 = startColB;

            //            //first add the key column (date if historical data)
            //            if (rangeIndex.IsHistorical == true)
            //            {
            //                perfsheet.get_Range(_GetNextColumnString(ref currentCol1, ref currentCol2) + currentRow).Value = clubIndex.Data[i].Date.Value;
            //            }
            //            else
            //            {
            //                perfsheet.get_Range(_GetNextColumnString(ref currentCol1, ref currentCol2) + currentRow).Value = clubIndex.Data[i].Key;
            //            }

            //            //now add the data for each index (including  cub index)
            //            foreach (var index in rangeIndex.Data)
            //            {
            //                if (index.Data.Count > 0)
            //                {
            //                    previousCol1 = currentCol1;
            //                    previousCol2 = currentCol2;
            //                    var nextCell = perfsheet.get_Range(_GetNextColumnString(ref currentCol1, ref currentCol2) + currentRow);
            //                    nextCell.Value = index.Data.Count > i ? index.Data[i].Price : index.Data[index.Data.Count - 1].Price;
            //                }
            //            }
            //            currentRow++;

            //            progress.Increment();
            //        }

            //        //now generate the chart and pivot table...
            //        var lstIndexes = rangeIndex.Data.Select(x => x.Name).ToList();
            //        //FirstRow = startRow, LastRow = currentRow - 1, FirstCol = chFirstCol, LastCol = (char)(chCol - 1)
            //        _buildPivotTableAndChart(performanceBook, lstIndexes, rangeIndex, keyName, _GetCurrentColumnString(startColA, startColB), _GetCurrentColumnString(previousCol1, previousCol2),
            //                                startRow, currentRow - 1, perfsheet, rangeIndex.IsHistorical, rangeIndex.MinValue);
            //        startColA = currentCol1;
            //        startColB = currentCol2;
            //        _GetNextColumnString(ref startColA, ref startColB);
            //    }

            //    logger.Log(LogLevel.Info, "saving performance chart to sheet {0}",
            //                        performanceBook.FullName);

            //    performanceBook.Save();

            //    progress.Increment();
            //}
            //finally
            //{
            //    performanceBook.Close();
            //}
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
        private void _buildPivotTableAndChart(_Workbook performanceBook, IEnumerable<string> enIndexes, IndexedRangeData index, string key, string firstCol, string lastCol,
                                            int firstRow, int lastRow, _Worksheet sourceSheet, bool IsHistorical, double minValue)
        {
            logger.Log(LogLevel.Info, "building chart for {0}", index.Name);

            //add a pivot sheet for this date range 
            performanceBook.Worksheets.Add();
            var pivotSheet = performanceBook.Worksheets[1];
            pivotSheet.Name = index.Name;

            var firstCell = firstCol + firstRow;
            var lastCell = lastCol + lastRow;
            var sourceRange = sourceSheet.Range[firstCell, lastCell];

            var destinationFirstCell = pivotSheet.Range["B1"];
            var destinationLastCell = lastCol + (1 + (lastRow - firstRow));
            var destinationRange = pivotSheet.Range["B1", destinationLastCell];

            //Macro for building pivot table and chart
            PivotCache pivotCache = performanceBook.PivotCaches().Create(XlPivotTableSourceType.xlDatabase, sourceRange, XlPivotTableVersionList.xlPivotTableVersion14);
            PivotTable pivotTable = pivotCache.CreatePivotTable(destinationFirstCell, "Club Performance");// true, XlPivotTableVersionList.xlPivotTableVersion14);

            var newShape = pivotSheet.Shapes.AddChart(240, XlChartType.xlColumnClustered);
            Microsoft.Office.Interop.Excel.Chart newChart = newShape.Chart;
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

            foreach (var subIndex in enIndexes)
            {
                pivotTable.AddDataField(pivotTable.PivotFields(subIndex), string.Format("Sum Of {0}", subIndex));
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

            newChart.HasTitle = true;
            newChart.ChartTitle.Text = index.Title;
            //newChart.ChartTitle.Text = key;
            newChart.Axes(XlAxisType.xlValue).MinimumScale = minValue;
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
                if (col2.HasValue == false)
                {
                    col1 = 'A';
                    col2 = 'A';
                }
            }
            else if (col2.HasValue)
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

        private bool _SheetExists(string newSheetName, _Workbook book)
        {
            foreach (_Worksheet existingSheet in book.Worksheets)
            {
                if (string.Compare(existingSheet.Name, newSheetName, true) == 0)
                {
                    //existingSheet.Delete();
                    return true;
                }
            }
            return false;
        }

        public void Dispose()
        {
        }
    }
}
