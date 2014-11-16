using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;
using System.IO;

namespace ExcelAccountsManager
{
    public class ExcelBookHolder
    {
        private _Workbook _investmentRecordBook;
        private _Workbook _assetBook;
        private _Workbook _templateBook;
        private _Workbook _cashBook;
        private _Workbook _performanceBook;
        private IEnumerable<_Workbook> _historicalAssetBooks;

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
            _historicalAssetBooks = _BuildHistoricalAssetSheets(path, app).ToList();

            var performanceBookName = string.Format(@"{0}PerformanceChart.xlsx",path);
            File.Delete(performanceBookName);
            _performanceBook = app.Workbooks.Add();
            _performanceBook.SaveAs(performanceBookName);
        }

        private IEnumerable<_Workbook> _BuildHistoricalAssetSheets(string path, _Application app)
        {
            //from 2009 to today
            int currentYear = DateTime.Today.Year;
            for(int year = 2009; year < currentYear; ++year )
            {
                string fileName = string.Format(@"{0}\{1}\Monthly Assets Statement-{2}.xls", path, year, year);
                yield return app.Workbooks.Open(fileName);
            }

            //don't forget the current asset book
            yield return _assetBook; 
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
    }
}
