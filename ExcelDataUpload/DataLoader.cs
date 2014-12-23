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
            //ExcelBookHolder bookHolder = new ExcelBookHolder(_app, path);

            //transfer over the cash account data
            var cashBooks = ExcelBookHolder.LoadAllBooks(path, "Cash Account", _app);
            foreach(var book in cashBooks)
            {
                var cashSheet = book.Worksheets["Cash Account"];
            }

        }
    }
}
