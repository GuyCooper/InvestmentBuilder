using System.Collections.Generic;

namespace InvestmentBuilderServiceTestRunner.Requests
{
    /// <summary>
    /// GetPortfolio Request tests.
    /// </summary>
    class GetPortfolioRequest : RequestEndpoint<Dto, GetPortfolioRequest.Result>
    {
        #region Internal classes

        public class Result :  Dto
        {
            public IEnumerable<Dtos.CompanyData> Portfolio { get; private set; }
        }

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public GetPortfolioRequest() 
            : base("GET_PORTFOLIO_REQUEST", "GET_PORTFOLIO_RESPONSE")
        {
        }

        #region Test Methods

        /// <summary>
        /// Get the portfolio with no investments.
        /// </summary>
        public void GetPortfolioEmpty(ConnectionService connectionService)
        {
            LogMessage("GetPortfolioEmpty");
            var result = SendRequest(new Dto(), connectionService);
            Assert.IsTrue(result.Success, "GetPortfolioEmpty", result.Error);
            Assert.IsNull(result.Result.Portfolio, "Number of Investments");
        }

        #endregion
    }
}
