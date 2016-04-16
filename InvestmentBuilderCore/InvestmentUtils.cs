using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestmentBuilderCore
{
    public static class InvestmentUtils
    {
        /// <summary>
        /// method agggregates a list of stocks into distinct aggregated stocks (i.e. if there are
        /// 2 stocks in the listwith a name of BP with amounts 5 and 3 the resulting stock list would
        /// have a single BP stock with an amount of 8 assumes all other stock members are the same
        /// </summary>
        /// <param name="stocks"></param>
        /// <returns></returns>
        public static IEnumerable<Stock> AggregateStocks(this IEnumerable<Stock> stocks)
        {
            if (stocks != null)
            {
                return stocks.Aggregate(new List<Stock>(), (a, s) =>
                {
                    var existing = a.FirstOrDefault(x => string.Compare(x.Name, s.Name) == 0);
                    if (existing == null)
                    {
                        a.Add(new Stock(s));
                    }
                    else
                    {
                        existing.Quantity += s.Quantity;
                        existing.TotalCost += s.TotalCost;
                    }
                    return a;
                });
            }
            return Enumerable.Empty<Stock>();
        }

        public static bool IsZero(this double lhs)
        {
            return AreSame(lhs, 0d);
        }

        public static bool AreSame(this double lhs, double rhs)
        {
            return Math.Abs(lhs - rhs) < double.Epsilon;
        }
    }
}
