using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using ExcelAccountsManager;

namespace ExcelDataUpload
{
    class DataLoader
    {
        private Application _app = new Microsoft.Office.Interop.Excel.Application();

        public void LoadData(string path)
        {
            //first load the cash account and investment record books for the current year
            string InvestmentRecordFile = string.Format(@"{0}\{1}-{2}", path, ExcelBookHolder.InvestmentRecordName, DateTime.Today.Year);
            string CashAccountFile = string.Format(@"{0}\{1}-{2}", path, ExcelBookHolder.CashAccountName, DateTime.Today.Year);
            ExcelBookHolder bookHolder = new ExcelBookHolder(_app, InvestmentRecordFile, null, null, CashAccountFile, path);

           //now upload the investment record data
   
        }

        private void _UploadInvestmentRecordData(ExcelBookHolder bookHolder)
        {
            for (int index = 1; index <= bookHolder.GetInvestmentRecordBook().Worksheets.Count; ++index)
            {
                _Worksheet recordSheet = bookHolder.GetInvestmentRecordBook().Worksheets[index];
                int row = 10;
                while (recordSheet.IsCellPopulated("A", row))
                {
                    var dtValuation = recordSheet.GetValueDateTime("A", row);
                    if (dtValuation.HasValue == false)
                    {
                        break;
                    }
                    int iSharesBought, iBonusShares, iSharesSold;
                    double dSharePrice, dTotalCost, dSellingPrice;
                    iSharesBought = iBonusShares = iSharesSold = 0;
                    dTotalCost = dSharePrice = dSellingPrice = 0;
                    recordSheet.GetValueInt("B", row, ref iSharesBought);
                    recordSheet.GetValueInt("C", row, ref iBonusShares);
                    recordSheet.GetValueInt("D", row, ref iSharesSold);
                    recordSheet.GetValueDouble("E", row, ref dTotalCost);
                    recordSheet.GetValueDouble("F", row, ref dSharePrice);
                    recordSheet.GetValueDouble("G", row, ref dSellingPrice);

                    row++;
                }
            }
        }
    }
}
