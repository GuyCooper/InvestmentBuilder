using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PerformanceBuilderLib;

//using ExcelAccountsManager;

namespace PerformanceBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            //string path = @"C:\Users\Guy\Documents\Guy\Investments\Investment Club\accounts\";
            string path = @"C:\Users\Guy\Documents\Guy\Investment Club\Accounts\";
            string dbConn = @"Data Source=TRAVELPC\SQLEXPRESS;Initial Catalog=InvestmentBuilderTest;Integrated Security=True";

            DateTime dtValuation = DateTime.Today;
            if (args.Length > 0)
            {
                dtValuation = DateTime.Parse(args[0]);
            }

            Console.WriteLine("performance builder: valuation date: {0}.type any key to continue...", dtValuation);
            Console.ReadKey();
            PerformanceBuilderExternal.RunBuilder(path, dbConn, dtValuation);
        }
    }
}
