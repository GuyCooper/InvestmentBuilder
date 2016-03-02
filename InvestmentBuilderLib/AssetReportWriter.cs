using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using System.IO;
using NLog;

namespace InvestmentBuilder
{
    //class persists the asset report to a file
    interface IAssetReportWriter
    {
        void WriteAssetReport(AssetReport report, double startOfYear);
    }

    //persist asset report to excel 
    class AssetReportWriterExcel : IAssetReportWriter, IDisposable
    {
        _Application _app;
        private _Workbook _assetBook;
        private _Workbook _templateBook;

        public const string MonthlyAssetName = "Monthly Assets Statement";
        public const string TemplateBookName = "template.xls";

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public AssetReportWriterExcel(string outputPath, string templateBookLocation)
        {
            _app = new Microsoft.Office.Interop.Excel.Application();
            var assetSheetLocation = string.Format(@"{0}\{1}-{2}.xls", outputPath, MonthlyAssetName, DateTime.Today.ToString("yyyy"));
            //if the asset sheet already exists,just open it,otherwise create a new one
            if (File.Exists(assetSheetLocation))
            {
                _assetBook = _app.Workbooks.Open(assetSheetLocation);
            }
            else
            {
                _assetBook = _app.Workbooks.Add();
                _assetBook.SaveAs(assetSheetLocation, XlFileFormat.xlWorkbookNormal);
            }
            //open the template book
            var templateFileName = Path.Combine(templateBookLocation, TemplateBookName);
            _templateBook = _app.Workbooks.Open(templateFileName);
        }

        private bool _SheetExists(string newSheetName)
        {
            foreach (_Worksheet existingSheet in _assetBook.Worksheets)
            {
                if (string.Compare(existingSheet.Name, newSheetName, true) == 0)
                {
                    //existingSheet.Delete();
                    return true;
                }
            }
            return false;
        }

        public void WriteAssetReport(AssetReport report, double startOfYear)
        {
            logger.Log(LogLevel.Info, "persisting asset report to excel spreadsheet");

            var newSheetName = report.ValuationDate.ToString("MMMM");
            //if this sheet already exists then delete it
            //_DeleteExistingSheet(newSheetName);
            if(_SheetExists(newSheetName))
            {
                logger.Log(LogLevel.Warn, "sheet {0} already exists in asset book", newSheetName);
                return;
            }

            _Worksheet templateSheet = _templateBook.Worksheets["Assets"];
            templateSheet.Copy(_assetBook.Worksheets[1]);

            _Worksheet newSheet = _assetBook.Worksheets[1];

            newSheet.EnableCalculation = true;
            newSheet.Name = newSheetName; 

            newSheet.get_Range("A1").Value = report.AccountName;
            newSheet.get_Range("C4").Value = report.ValuationDate;

            newSheet.get_Range("B16").Value = report.ReportingCurrency;

            //add in redemptions
            if(report.Redemptions != null)
            {
                newSheet.get_Range("B18").Font.Bold = true;
                newSheet.get_Range("B18").Value = "Redemptions";
                newSheet.get_Range("B19").Value = "Date";
                newSheet.get_Range("C19").Value = "User";
                newSheet.get_Range("D19").Value = "Amount";

                int redemptionRow = 20;
                foreach(var redemption in report.Redemptions)
                {
                    newSheet.get_Range("B" + redemptionRow).Value = redemption.TransactionDate;
                    newSheet.get_Range("C" + redemptionRow).Value = redemption.User;
                    newSheet.get_Range("D" + redemptionRow).Value = redemption.Amount;
                    redemptionRow++;
                }
            }

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
