using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestmentBuilderConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine(@"syntax: InvestmentBuilder <-p:path> <-t> <-v:Valuation Date> <-d:database string>. where -t = test -p = path and -d specifies valuation date");
                return;
            }

            string path = @"C:\Users\Guy\Documents\Guy\Investments\Investment Club\accounts\";
            string connectionsstr = string.Empty;
            bool bTest = false;
            DateTime? dtValuationDate = null;

            foreach (var arg in args)
            {
                if (arg[0] == '-')
                {
                    switch (arg[1])
                    {
                        case 'p':
                            if (arg[2] == ':')
                                path = arg.Substring(3);
                            break;
                        case 't':
                            bTest = true;
                            break;
                        case 'v':
                            if (arg[2] == ':')
                                dtValuationDate = DateTime.Parse(arg.Substring(3));
                            break;
                        case 'd':
                            if (arg[2] == ':')
                                connectionsstr = arg.Substring(3);
                            break;
                    }
                }
            }

            if (path.Last() != '\\')
            {
                path = path + "\\";
            }

            //load in any trades from the trades file
            string tradeFile = string.Format("{0}Trades.xml", path);

            var format = string.IsNullOrEmpty(connectionsstr) ? InvestmentBuilder.DataFormat.EXCEL :
                InvestmentBuilder.DataFormat.DATABASE;

            InvestmentBuilder.AssetSheetBuilder.BuildAssetSheet(tradeFile, path, connectionsstr, bTest, dtValuationDate.Value,
                                                                   format);

        }
    }
}
