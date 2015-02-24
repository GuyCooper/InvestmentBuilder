using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using ExcelAccountsManager;
using MarketDataServices;
using NLog;

namespace InvestmentBuilder
{
    /// <summary>
    /// excel investment record
    /// </summary>
    class ExcelInvestment : IInvestment
    {
        private _Worksheet _sheet;
        private int _lastRow;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public ExcelInvestment(_Worksheet sheet)
        {
            _sheet = sheet;
            Name = _sheet.Name;
            _lastRow = _sheet.GetLastPopulatedRow("A", 9);
        }

        public string Name { get; private set; }

        public void DeactivateInvestment()
        {
            _sheet.get_Range("B7").Value = false;
        }

        public void UpdateRow(DateTime valuationDate, DateTime? previousDate)
        {            
            //check if the last row was written this month, if it was then just update the current last
            //row, otherwise add a new row
            var lastDate = _sheet.GetValueDateTime("A", _lastRow);
            if (lastDate.HasValue && lastDate.Value.Month == valuationDate.Month)
            {
                //delete the row
                Range rowToDelete = _sheet.get_Range("A" + _lastRow, "A" + _lastRow).EntireRow;
                rowToDelete.Delete(XlDeleteShiftDirection.xlShiftUp);
                _lastRow--;
            }
            Range rowToCopy = _sheet.get_Range("A" + _lastRow, "A" + _lastRow).EntireRow;
            rowToCopy.Copy(); //copy onto clipboard
            _lastRow++;
            Range newRow = _sheet.get_Range("A" + _lastRow, "A" + _lastRow).EntireRow;
            newRow.PasteSpecial();

            //add the date
            _sheet.get_Range("A" + _lastRow).Value = valuationDate;
            _sheet.get_Range("I" + _lastRow).Value = valuationDate;

        }

        public void ChangeShareHolding(int holding)
        {
            _sheet.get_Range("B" + _lastRow).Value = holding;
        }

        public void AddNewShares(Stock stock)
        {
            logger.Log(LogLevel.Info, string.Format("adding new shares to existing for company {0}", Name));
            //Console.WriteLine("adding new shares to existing for company {0}", Name);
            //this stock has been brought, update the bought
            var newBought = (int)_sheet.get_Range("B" + _lastRow).Value + stock.Number;
            var newTotalCost = (double)_sheet.get_Range("G" + _lastRow).Value + stock.TotalCost;
            _sheet.get_Range("B" + _lastRow).Value = newBought;
            _sheet.get_Range("G" + _lastRow).Value = newTotalCost;
        }

        public void UpdateDividend(double dDividend)
        {
            double dExistingDividend = 0;
            _sheet.GetValueDouble("P", _lastRow, ref dExistingDividend);
            _sheet.get_Range("P" + _lastRow).Value = dExistingDividend + dDividend;
        }

        public void UpdateClosingPrice()
        {
            //now download the previous close price for this stock and update the record
            string symbol = _sheet.get_Range("B4").Value as string;
            string currency = _sheet.get_Range("B5").Value as string;
            double dScaling = 1;
            _sheet.GetValueDouble("C", 4, ref dScaling);
            //_SetClosingPrice(Name, symbol, currency, dScaling, _lastRow);
            double dClosing;
            if (MarketDataService.TryGetClosingPrice(symbol, Name, currency, dScaling, out dClosing))
            {
                _sheet.get_Range("J" + _lastRow).Value = dClosing;
            }
 
        }

        //private void _SetClosingPrice(string company, string symbol, string currency, double scaling, int row)
        //{
        //    double dClosing;
        //    if (MarketDataService.TryGetClosingPrice(symbol, company, currency, scaling, out dClosing))
        //    {
        //        _sheet.get_Range("J" + row).Value = dClosing;
        //    }
        //}
    }

    class InvestmentRecordBuilderExcel : InvestmentRecordBuilder
    {
        private ExcelBookHolder _bookHolder;
        public InvestmentRecordBuilderExcel(ExcelBookHolder bookHolder)
        {
            _bookHolder = bookHolder;
        }

        override protected IEnumerable<IInvestment> GetInvestments(DateTime valuationDate)
        {
            for (int index = 1; index <= _bookHolder.GetInvestmentRecordBook().Worksheets.Count; ++index)
            {
                _Worksheet sheet = _bookHolder.GetInvestmentRecordBook().Worksheets[index];
                var title = sheet.get_Range("A3").Value as string;
                if (title != null && title.ToUpper() == "NAME OF COMPANY")
                {
                    yield return new ExcelInvestment(sheet);
                }
            }
        }

        override protected void CreateNewInvestment(Stock newTrade, DateTime valuationDate)
        {
            _Worksheet templateSheet = _bookHolder.GetTemplateBook().Worksheets["Investment"];
            templateSheet.Copy(Type.Missing, _bookHolder.GetInvestmentRecordBook().Worksheets
                [_bookHolder.GetInvestmentRecordBook().Worksheets.Count]);

            _Worksheet newRecordSheet = _bookHolder.GetInvestmentRecordBook().Worksheets
                [_bookHolder.GetInvestmentRecordBook().Worksheets.Count];
            newRecordSheet.EnableCalculation = true;
            newRecordSheet.Name = newTrade.Name;
            newRecordSheet.get_Range("B3").Value = newTrade.Name;
            newRecordSheet.get_Range("B4").Value = newTrade.Symbol;
            newRecordSheet.get_Range("B5").Value = newTrade.Currency;
            newRecordSheet.get_Range("B7").Value = true;
            newRecordSheet.get_Range("A10").Value = valuationDate;
            newRecordSheet.get_Range("I10").Value = valuationDate;
            newRecordSheet.get_Range("B10").Value = newTrade.Number;
            newRecordSheet.get_Range("G10").Value = newTrade.TotalCost;
            newRecordSheet.get_Range("C4").Value = newTrade.ScalingFactor;
            var newInvestment = new ExcelInvestment(newRecordSheet);
            newInvestment.UpdateClosingPrice();
        }
    }
}
