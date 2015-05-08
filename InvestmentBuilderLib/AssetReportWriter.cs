using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelAccountsManager;
using Microsoft.Office.Interop.Excel;
using System.IO;
using NLog;

namespace InvestmentBuilder
{
    //class persists the asset report to a file
    interface IAssetReportWriter
    {
        void WriteAssetReport(AssetReport report);
    }

    //persist asset report to excel 
    class AssetReportWriterExcel : IAssetReportWriter, IDisposable
    {
        _Application _app;
        private _Workbook _assetBook;
        private _Workbook _templateBook;

        public const string MonthlyAssetName = "Monthly Assets Statement";

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public AssetReportWriterExcel(string outputPath, string templateBookLocation)
        {
            _app = new Microsoft.Office.Interop.Excel.Application();
            var assetSheetLocation = string.Format(@"{0}\{1}-{2}.xls", outputPath, MonthlyAssetName, DateTime.Today.ToString("YYYY"));
            //if the asset sheet already exists,just open it,otherwise create a new one
            if (File.Exists(assetSheetLocation))
            {
                _assetBook = _app.Workbooks.Open(assetSheetLocation);
            }
            else
            {
                _assetBook = _app.Workbooks.Add();
                _assetBook.SaveAs(assetSheetLocation);
            }
            //open the template book
            _templateBook = !string.IsNullOrEmpty(templateBookLocation) ? _app.Workbooks.Open(templateBookLocation) : null;
        }

        private void _DeleteExistingSheet(string newSheetName)
        {
            foreach (_Worksheet existingSheet in _assetBook.Worksheets)
            {
                if (string.Compare(existingSheet.Name, newSheetName) == 0)
                {
                    existingSheet.Delete();
                    return;
                }
            }
        }

        public void WriteAssetReport(AssetReport report)
        {
            logger.Log(LogLevel.Info, "persisting asset report to excel spreadsheet");

            var newSheetName = report.ValuationDate.ToString("MMMM");
            //if this sheet already exists then delete it
            //_DeleteExistingSheet(newSheetName);

            _Worksheet templateSheet = _templateBook.Worksheets["Assets"];
            templateSheet.Copy(_assetBook.Worksheets[1]);

            _Worksheet newSheet = _assetBook.Worksheets[1];

            newSheet.EnableCalculation = true;
            newSheet.Name = newSheetName; 

            newSheet.get_Range("A1").Value = report.AccountName;
            newSheet.get_Range("C4").Value = report.ValuationDate;

            newSheet.get_Range("B16").Value = report.ReportingCurrency;

            //add in the new rows
            var lstCompanyData = report.Assets.ToList();
            for (int row = 1; row < lstCompanyData.Count; ++row)
            {
                Range rowToCopy = newSheet.get_Range("A7", "A7").EntireRow;
                rowToCopy.Copy(); //put row onto clipboard
                rowToCopy.Insert(XlInsertShiftDirection.xlShiftDown);
                newSheet.get_Range("A7", "A7").EntireRow.PasteSpecial(); //paste format into new row
            }

            int count = 7;
            //now add the company data
            foreach (var company in lstCompanyData)
            {
                logger.Log(LogLevel.Info, string.Format("Adding {0} to asset sheet", company.sName));
                //Console.WriteLine("Adding {0} to asset sheet", company.sName);

                newSheet.get_Range("B" + count).Value = company.sName;
                newSheet.get_Range("C" + count).Value = company.dtLastBrought.Value;
                newSheet.get_Range("D" + count).Value = company.iNumberOfShares;
                newSheet.get_Range("E" + count).Value = company.dAveragePricePaid;
                newSheet.get_Range("F" + count).Value = company.dTotalCost;
                newSheet.get_Range("G" + count).Value = company.dSharePrice;
                newSheet.get_Range("H" + count).Value = company.dNetSellingValue;
                newSheet.get_Range("I" + count).Value = company.dProfitLoss;
                newSheet.get_Range("J" + count).Value = company.dMonthChange;
                newSheet.get_Range("K" + count).Value = company.dMonthChangeRatio;
                newSheet.get_Range("L" + count).Value = company.dDividend;
                count++;
            }

            newSheet.get_Range("H" + count).Value = report.TotalAssetValue;
            newSheet.get_Range("J" + count).Value = lstCompanyData.Sum(a => a.dMonthChange);
            newSheet.get_Range("L" + count).Value = lstCompanyData.Sum(a => a.dDividend);
            count++;
            newSheet.get_Range("H" + count++).Value = report.BankBalance;
            count += 2;
            newSheet.get_Range("H" + count++).Value = report.TotalAssets;
            newSheet.get_Range("H" + count++).Value = report.TotalLiabilities;
            count++;
            newSheet.get_Range("H" + count++).Value = report.NetAssets;
            newSheet.get_Range("H" + count++).Value = report.IssuedUnits;
            newSheet.get_Range("H" + count++).Value = report.ValuePerUnit;

            logger.Log(LogLevel.Info, "saving asset sheet: {0}", _assetBook.FullName);
            _assetBook.Save();
        }

        public void Dispose()
        {
            if(_assetBook != null)
            {
                _assetBook.Close();
            }
            if(_templateBook != null)
            {
                _templateBook.Close();
            }
        }
    }
}
