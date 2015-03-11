using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelAccountsManager;
using Microsoft.Office.Interop.Excel;
using NLog;

namespace InvestmentBuilder
{
    //class persists the asset report to a file
    interface IAssetReportWriter
    {
        void WriteAssetReport(AssetReport report);
    }

    //persist asset report to excel 
    class AssetReportWriterExcel : IAssetReportWriter
    {
        private ExcelBookHolder _bookHolder;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public AssetReportWriterExcel(ExcelBookHolder bookHolder)
        {
            _bookHolder = bookHolder;
        }

        public void WriteAssetReport(AssetReport report)
        {
            logger.Log(LogLevel.Info, "persisting asset report to excel spreadsheet");
            _Worksheet templateSheet = _bookHolder.GetTemplateBook().Worksheets["Assets"];
            templateSheet.Copy(_bookHolder.GetAssetSheetBook().Worksheets[1]);

            _Worksheet newSheet = _bookHolder.GetAssetSheetBook().Worksheets[1];
            newSheet.EnableCalculation = true;
            newSheet.Name = report.ValuationDate.ToString("MMMM");

            newSheet.get_Range("A1").Value = report.ClubName;
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
        }
    }
}
