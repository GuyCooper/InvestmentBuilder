using System.Collections.Generic;

namespace InvestmentBuilderServiceTestRunner.Requests
{
    /// <summary>
    /// UpdateAccountDetailsRequest tests.
    /// </summary>
    class UpdateAccountDetailsRequest : RequestEndpoint<UpdateAccountDetailsRequest.Request, UpdateAccountDetailsRequest.Result>
    {
        #region Internal classes

        /// <summary>
        /// Request Dto.
        /// </summary>
        public class Request : Dto
        {
            public Dtos.AccountIdentifier AccountName { get; set; }
            public string Description { get; set; }
            public string ReportingCurrency { get; set; }
            public string AccountType { get; set; }
            public bool Enabled { get; set; }
            public string Broker { get; set; }
            public IList<Dtos.AccountMemberDto> Members { get; set; }
        }

        /// <summary>
        /// Result Dto.
        /// </summary>
        public class Result : Dto
        {
            public bool Status { get; set; }
            public IEnumerable<Dtos.AccountIdentifier> AccountNames { get; set; }
        }

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public UpdateAccountDetailsRequest()
            : base("UPDATE_ACCOUNT_DETAILS_REQUEST", "UPDATE_ACCOUNT_DETAILS_RESPONSE")
        {
        }
    }
}
