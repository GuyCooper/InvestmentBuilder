namespace PerformanceBuilderLib
{
    #region References

    using InvestmentBuilderCore;
    using NLog;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    #endregion

    public sealed class AnalyticDataBuilder
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private IConfigurationSettings _configurationSettings;
        private IClientDataInterface _clientData;
        private ICashAccountInterface _cashAccountData;
        private readonly IClock _clock;
        private const double _inflation = 0.02d;

        public AnalyticDataBuilder(IConfigurationSettings configurationSettings,
                                   IDataLayer dataLayer,
                                   IClock clock)
        {
            _configurationSettings = configurationSettings;
            _clientData = dataLayer.ClientData;
            _cashAccountData = dataLayer.CashAccountData;
            _clock = clock;
        }
        public AnalyticData BuildAnalyticData(UserAccountToken userToken, 
                                              AssetReport assetReport)
        {
            //YTD
            //Avg. Yield
            //Projections
            var result = new AnalyticData
            {
                Title = "Analytic Data (inflation adjusted)",
                ReportingCurrency = assetReport.ReportingCurrency,
                ValuationDate = assetReport.ValuationDate,
                PerformanceYTD = assetReport.YearToDatePerformance,
            };

            var valuationData = _clientData.GetAllValuations(userToken)
                                .OrderBy(v => v.ValuationDate)
                                .ToList();

            var subscriptions = _cashAccountData.GetCashTransactions(
                                                    userToken,
                                                    TransactionTypes.SUBSCRIPTION)
                                                    .OrderByDescending(t => t.Item1)
                                                    .ToList();


            var monthlyAmount = subscriptions.FirstOrDefault()?.Item2;

            if (valuationData.Any())
            {
                var firstValuation = valuationData.First();
                var lastValuation = valuationData.Last();

                var duration = (lastValuation.ValuationDate - firstValuation.ValuationDate).Duration();
                var durationYears = (int)Math.Ceiling(duration.TotalDays / 365);              
                
                result.AverageYield = AnalyticsCalculator.CalculateAverageYield(firstValuation.UnitValue,
                                                                                lastValuation.UnitValue,
                                                                                durationYears);

                var ranges = new List<int>
                {
                    5,
                    10,
                    15,
                    20
                };

                var strRanges = ranges.Select(r => $"{r} years").ToList();

                var rates = new List<Tuple<string, double>>
                {
                    Tuple.Create($"{result.AverageYield:N2}%", result.AverageYield / 100),
                    Tuple.Create("3%", 0.03),
                    Tuple.Create("5%", 0.05),
                    Tuple.Create("8%", 0.08),
                    Tuple.Create("10%", 0.10),
                    Tuple.Create("12%", 0.12)
                };

                result.ProjectedNAVs = new RateProjections
                {
                    Ranges = strRanges,
                    Projections = rates.Select(r =>
                    {
                        var projections = ranges.Select(t =>
                        AnalyticsCalculator.CalculateProjection(
                                                              assetReport.NetAssets,
                                                              monthlyAmount ?? 0,
                                                              t,
                                                              r.Item2,
                                                              _inflation))
                        .ToList();

                        return new KeyValuePair<string, List<double>>
                        (
                           r.Item1,
                           projections
                        );
                   }).ToList()
                };
            }

            return result;
        }
    }
}
