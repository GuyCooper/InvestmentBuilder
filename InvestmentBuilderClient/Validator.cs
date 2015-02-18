using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderClient.View;

namespace InvestmentBuilderClient
{
    internal static class Validator
    {
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
            return invalid == false;
        }
    }
}
