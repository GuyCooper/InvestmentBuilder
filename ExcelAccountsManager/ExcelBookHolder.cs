﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;
using System.IO;

namespace ExcelAccountsManager
{
    public class ExcelBookHolder : IDisposable
    {
        _Application _app;
        //private _Workbook _investmentRecordBook;
        private _Workbook _assetBook;
        private _Workbook _templateBook;
        //private _Workbook _cashBook;
        private _Workbook _performanceBook;
        //private IEnumerable<_Workbook> _historicalAssetBooks;

        //public const string CashAccountName = "Cash Account";
        //public const string InvestmentRecordName = "Investment Record";
        public const string MonthlyAssetName = "Monthly Assets Statement";
        public const string PerformanceChartName = "Performance Chart";

        public ExcelBookHolder(string assetSheetLocation,
                                string templateSheetLocation,
                                string path)
                                
        {
            _app = new Microsoft.Office.Interop.Excel.Application();
            if (!string.IsNullOrEmpty(assetSheetLocation))
            {
                //if the asset sheet already exists,just open it,otherwise create a new one
                if(File.Exists(assetSheetLocation))
                {
                    _assetBook = _app.Workbooks.Open(assetSheetLocation);
                }
                else
                {
                    _assetBook = _app.Workbooks.Add();
                    _assetBook.SaveAs(assetSheetLocation);
                }
            }
            else
            {
                _assetBook = null;
            }

            _templateBook = !string.IsNullOrEmpty(templateSheetLocation) ? _app.Workbooks.Open(templateSheetLocation) : null;
        }

        public ExcelBookHolder(string performanceBookLocation)
        {
            _app = new Microsoft.Office.Interop.Excel.Application();
            File.Delete(performanceBookLocation);
            _performanceBook = _app.Workbooks.Add();
            _performanceBook.SaveAs(performanceBookLocation);
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
            _SaveBook(_assetBook);
            _SaveBook(_performanceBook);
        }

        public _Workbook GetAssetSheetBook()
        {
            return _assetBook;
        }

        public _Workbook GetTemplateBook()
        {
            return _templateBook;
        }

        public _Workbook GetPerformanceBook()
        {
            return _performanceBook;
        }

        //public static IList<_Workbook> LoadAllBooks(string path, string bookName, _Application app, DateTime dtValuation)
        //{
        //    //from 2009 to today
        //    var bookList = new List<_Workbook>();
        //    int currentYear = dtValuation.Year;
        //    for (int year = 2009; year < currentYear; ++year)
        //    {
        //        string fileName = string.Format(@"{0}\{1}\{2}-{3}.xls", path, year, bookName, year);
        //        bookList.Add(app.Workbooks.Open(fileName));
        //    }

        //    var currentBook = app.Workbooks.Open(string.Format(@"{0}\{1}-{2}.xls", path, bookName, currentYear));
        //    bookList.Add(currentBook);
        //    return bookList;
        //}

        public void Dispose()
        {
            if (_assetBook != null)
                _assetBook.Close();
            if (_templateBook != null)
                _templateBook.Close();
            if (_performanceBook != null)
                _performanceBook.Close();
        }
    }
}
