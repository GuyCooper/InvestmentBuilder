namespace InvestmentBuilderCore
{
    #region References

    using System;
    using System.Collections.Generic;

    #endregion

    public class RateProjections
    {
        /// <summary>
        /// date range for projection (1 year, 5 year etc...)
        /// </summary>
        public List<string> Ranges { get; set; }

        /// <summary>
        /// list of yield to projections ( 5% - 1 year, 5 year...)
        /// </summary>
        public List<KeyValuePair<string, List<double>>> Projections { get; set; }
    }

    public class AnalyticData
    {
        public string Title { get; set; }

        public DateTime ValuationDate { get; set; }

        public string Description { get; set; }

        public string ReportingCurrency { get; set; }

        public double PerformanceYTD { get; set; }
        
        public double AverageYield { get; set; }

        public RateProjections ProjectedNAVs { get; set; }

    }
}
