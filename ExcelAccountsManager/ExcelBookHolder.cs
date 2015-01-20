using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;
using System.IO;

namespace ExcelAccountsManager
{
    public class ExcelBookHolder : IDisposable
    {
        private _Workbook _investmentRecordBook;
        private _Workbook _assetBook;
        private _Workbook _templateBook;
        private _Workbook _cashBook;
        private _Workbook _performanceBook;
        private IEnumerable<_Workbook> _historicalAssetBooks;

        public const string CashAccountName = "Cash Account";
        public const string InvestmentRecordName = "Investment Record";
        public const string MonthlyAssetName = "Monthly Assets Statement";
        public const string PerformanceChartName = "Performance Chart";

        public ExcelBookHolder(_Application app,
                                string investmentRecordSheetLocation,
                                string assetSheetLocation,
                                string templateSheetLocation,
                                string cashAccountSheetLocation,
                                string path)
        {
            _investmentRecordBook = !string.IsNullOrEmpty(investmentRecordSheetLocation) ? app.Workbooks.Open(investmentRecordSheetLocation) : null;
            _assetBook = !string.IsNullOrEmpty(assetSheetLocation) ? app.Workbooks.Open(assetSheetLocation) : null;
            _templateBook = !string.IsNullOrEmpty(templateSheetLocation) ? app.Workbooks.Open(templateSheetLocation) : null;
            _cashBook = !string.IsNullOrEmpty(cashAccountSheetLocation) ?  app.Workbooks.Open(cashAccountSheetLocation) : null;
            //_historicalAssetBooks = LoadAllBooks(path, MonthlyAssetName, app);
        }

        public ExcelBookHolder(_Application app,
                                string investmentRecordSheetLocation,
                                string cashAccountSheetLocation,
                                string path,
                                DateTime ValuationDate)
        {
            _investmentRecordBook = !string.IsNullOrEmpty(investmentRecordSheetLocation) ? app.Workbooks.Open(investmentRecordSheetLocation) : null;
            _cashBook = !string.IsNullOrEmpty(cashAccountSheetLocation) ? app.Workbooks.Open(cashAccountSheetLocation) : null;
            _historicalAssetBooks = LoadAllBooks(path, MonthlyAssetName, app, ValuationDate);
        }

        public ExcelBookHolder(_Application app,
                                string path,
                                DateTime ValuationDate)
        {
            var performanceBookName = string.Format(@"{0}{1}-{2}.xlsx", path, PerformanceChartName, ValuationDate.ToString("MMM-yyyy"));
            File.Delete(performanceBookName);
            _performanceBook = app.Workbooks.Add();
            _performanceBook.SaveAs(performanceBookName);
            _historicalAssetBooks = LoadAllBooks(path, MonthlyAssetName, app, ValuationDate);
        }

        private void _SaveBook(_Workbook book)
        {
            if(book != null)
            {
                book.Save();
            }
        }
        public void SaveBooks()
        {
            _SaveBook(_investmentRecordBook);
            _SaveBook(_assetBook);
            _SaveBook(_cashBook);
            _SaveBook(_performanceBook);
        }

        public _Workbook GetInvestmentRecordBook()
        {
            return _investmentRecordBook;
        }

        public _Workbook GetAssetSheetBook()
        {
            return _assetBook;
        }

        public _Workbook GetCashBook()
        {
            return _cashBook;
        }

        public _Workbook GetTemplateBook()
        {
            return _templateBook;
        }

        public IEnumerable<_Workbook> GetHistoricalAssetBooks()
        {
            return _historicalAssetBooks;
        }

        public _Workbook GetPerformanceBook()
        {
            return _performanceBook;
        }

        public static IList<_Workbook> LoadAllBooks(string path, string bookName, _Application app, DateTime dtValuation)
        {
            //from 2009 to today
            var bookList = new List<_Workbook>();
            int currentYear = dtValuation.Year;
            for (int year = 2009; year < currentYear; ++year)
            {
                string fileName = string.Format(@"{0}\{1}\{2}-{3}.xls", path, year, bookName, year);
                bookList.Add(app.Workbooks.Open(fileName));
            }

            var currentBook = app.Workbooks.Open(string.Format(@"{0}\{1}-{2}.xls", path, bookName, currentYear));
            bookList.Add(currentBook);
            return bookList;
        }

        public void Dispose()
        {
            if (_investmentRecordBook != null)
                _investmentRecordBook.Close();
            if (_assetBook != null)
                _assetBook.Close();
            if (_templateBook != null)
                _templateBook.Close();
            if (_cashBook != null)
                _cashBook.Close();
            if (_performanceBook != null)
                _performanceBook.Close();
            if (_historicalAssetBooks != null)
            {
                foreach (var book in _historicalAssetBooks)
                {
                    book.Close();
                }
            }
        }
    }
}
