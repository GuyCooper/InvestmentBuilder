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
            //string path = @"C:\Users\Guy\Documents\Guy\Investments\Investment Club\accounts\";
            string path = @"C:\Users\Guy\Documents\Guy\Investment Club\Accounts\";
            string dbConn = @"Data Source=TRAVELPC\SQLEXPRESS;Initial Catalog=InvestmentBuilderTest;Integrated Security=True";

            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
            try
            {
                DateTime dtValuation = DateTime.Today;
                if (args.Length > 0)
                {
                    dtValuation = DateTime.Parse(args[0]);
                }
                //string assetSheet = string.Format("{0}Monthly Assets Statement-2014.xls", path);
                var bookHolder = new ExcelBookHolder(app, path, dtValuation);
                var performanceBuilder = new PerformanceBuilder(bookHolder, dbConn);
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
