using System.Collections.Generic;
using System.Linq;

namespace InvestmentBuilderServiceTestRunner
{
    /// <summary>
    /// GetAccountNames Request tests
    /// </summary>
    class GetAccountNamesRequest : RequestEndpoint<Dto, GetAccountNamesRequest.Result>
    {
        #region Internal classes

        public class Result : Dto
        {
            public IEnumerable<Dtos.AccountIdentifier> AccountNames { get; set; }
        }

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public GetAccountNamesRequest() : base("GET_ACCOUNT_NAMES_REQUEST", "GET_ACCOUNT_NAMES_RESPONSE")
        {
        }

        #region Test Methods

        /// <summary>
        /// Get account names for empty user
        /// </summary>
        public void GetAccountNamesEmptyUser(ConnectionService connectionService)
        {
            // Get account names..
            LogMessage("GetAccountNames");
            var result = SendRequest(new Dto(), connectionService);
            Assert.IsTrue(result.Success, "GetAccountNames", result.Error);
            Assert.AreEqual(0, result.Result.AccountNames.Count(), "GetAccountNames Number of Accounts");
        }

        #endregion
    }
}
