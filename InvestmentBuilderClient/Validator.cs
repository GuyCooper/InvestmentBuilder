using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderClient.View;
using NLog;

namespace InvestmentBuilderClient
{
    internal static class Validator
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static bool Compare(double d1, double d2)
        {
            return Math.Abs(d1 - d2) <= double.Epsilon;
        }

        public static bool Validate(IEnumerable<IInvestmentBuilderView> enViews)
        {
            bool invalid = false;
            enViews.OfType<CashAccountView>().Aggregate((p, c) =>
                {
                    if(p != null && !invalid)
                    {
                        invalid = Compare(p.GetTotal(), c.GetTotal()) == false;
                    }
                    return c;
                });

            if(invalid == true)
            {
                logger.Log(LogLevel.Error, "validation failed, non matching totals for cash account");
            }
            return invalid == false;
        }
    }
}
