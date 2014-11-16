using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelAccountsManager;

namespace PerformanceBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"C:\Users\Guy\Documents\Guy\Investments\Investment Club\accounts\";
            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
            try
            {
                string assetSheet = string.Format("{0}Monthly Assets Statement-2014.xls", path);
                var bookHolder = new ExcelBookHolder(app, null, assetSheet, null, null, path);
                var performanceBuilder = new PerformanceBuilder(bookHolder);
                performanceBuilder.Run(app);
                bookHolder.SaveBooks();
            }
            finally
            {
                app.Workbooks.Close();
            }

        }
    }
}
