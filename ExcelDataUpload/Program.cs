using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ExcelDataUpload
{
    class Program
    {
        /// <summary>
        /// program uploads all investment club data from excel spreadsheets into database
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //read in cash account data from excel - add to cash account table
            string accountsPath = null;
            string valuedate = null;
            var db = @"Data Source=TRAVELPC\SQLEXPRESS;Initial Catalog=InvestmentBuilderTest;Integrated Security=True";
            for(int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                if(arg[0] == '-')
                {
                    if(arg[1] == 'p')
                    {
                        accountsPath = arg.Substring(3);
                    }
                    else if(arg[1] == 'd')
                    {
                        valuedate = arg.Substring(3);
                    }
                }
            }

            if(accountsPath != null && valuedate != null)
            {
                Console.WriteLine("path: {0}", accountsPath);
                Console.WriteLine("valuation date: {0}", valuedate);
                using(var dataLoader = new DataLoader(accountsPath, db, DateTime.Parse(valuedate)))
                {
                    dataLoader.LoadData();
                }
            }
            
        }
    }
}
