using System;

namespace InvestmentBuilderServiceTestRunner.Requests
{
    /// <summary>
    /// Get Investment Summary tests.
    /// </summary>
    class GetInvestmentSummaryRequest : RequestEndpoint<GetInvestmentSummaryRequest.Request, GetInvestmentSummaryRequest.Result>
    {
        #region Internal classes

        /// <summary>
        /// Request Dto
        /// </summary>
        public class Request : Dto
        {
            public string DateFrom { get; set; }
        }

        /// <summary>
        /// Result Dto
        /// </summary>
        public class Result : Dto
        {
            public Dtos.AccountIdentifier AccountName { get; set; }
            public string ReportingCurrency { get; set; }
            public DateTime ValuationDate { get; set; }
            public string TotalAssetValue { get; set; }
            public string BankBalance { get; set; }
            public string TotalAssets { get; set; }
            public string NetAssets { get; set; }
            public string ValuePerUnit { get; set; }
            public string MonthlyPnL { get; set; }
        }

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public GetInvestmentSummaryRequest()
            : base("GET_INVESTMENT_SUMMARY_REQUEST", "GET_INVESTMENT_SUMMARY_RESPONSE")
        {
        }

        #region Test Methods

        /// <summary>
        /// GetInvestmentSummary
        /// </summary>
        public void GetInvestmentSummaryEmptyUser(ConnectionService connectionService)
        {
            LogMessage("GetInvestmentSummary");
            var result = SendRequest(new GetInvestmentSummaryRequest.Request(), connectionService);
            Assert.IsTrue(result.Success, "GetInvestmentSummary", result.Error);
        }

        #endregion
    }
}
