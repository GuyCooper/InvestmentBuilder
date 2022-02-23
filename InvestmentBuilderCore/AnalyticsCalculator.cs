namespace InvestmentBuilderCore
{
    #region References

    using System;

    #endregion 

    public static class AnalyticsCalculator
    {
        public static double CalculateProjection(double initialAmount,
                                                 double monthlyPayment,
                                                 int projectionYears,
                                                 double yield,
                                                 double inflation)
        {
            var amount = initialAmount;
            double realYield = yield - inflation;
            double monthYield = realYield / 12d;
            int projectionMonths = projectionYears * 12;
            for (int month = 0; month < projectionMonths; month++)
            {
                var totalMonthAmount = amount + monthlyPayment;
                amount = totalMonthAmount + (totalMonthAmount * monthYield);
            }
            return amount;
        }

        public static double CalculateAverageYield(double startAmount, double endAmount, int years)
        {
            if(startAmount.AreSame(endAmount))
            {
                return 0d;
            }

            var isNegativeYield = endAmount < startAmount;
            var lowestAmount = isNegativeYield ? endAmount : startAmount;
            var highestAmount = isNegativeYield ? startAmount : endAmount;
            var result = CalculateAverageYieldLowToHigh(lowestAmount, highestAmount, years);
            return isNegativeYield ? (-1 * result) : result;
        }

        private static double CalculateAverageYieldLowToHigh(double lowestAmount, double highestAmount, int years)
        {
            (double yield, double nextYield) result = (0,0);
            double increment = 1;
            for (int depth = 0; depth < 4 ; depth++)
            {
                result = CalculateAverageYield(lowestAmount, highestAmount, years, result.yield, increment);
                increment = increment / 10;
            }

                var res1 = Math.Abs(highestAmount - CalculateFinalAmount(lowestAmount, result.yield, years));
                var res2 = Math.Abs(highestAmount - CalculateFinalAmount(lowestAmount, result.nextYield, years));
                return res1 <= res2 ? result.yield : result.nextYield;            
        }

        private static (double yield, double yieldNext) CalculateAverageYield(double startAmount, double endAmount, int years, double startYield, double increment)
        {
            double yield = startYield;
            var previousYield = yield;
            var next = startAmount;
            while (next < endAmount)
            {
                next = CalculateFinalAmount(startAmount, yield, years);
                if(next < endAmount)
                {
                    previousYield = yield;
                    yield += increment; 
                }
            }

            return (previousYield, yield);
        }

        private static double CalculateFinalAmount(double startAmount, double yield, int years)
        {
            var amount = startAmount;
            var yieldPercent = yield / 100;
            for (int year = 0; year < years; year++)
            {
                amount += (amount * yieldPercent);
            }
            return amount;

        }
    }
}
