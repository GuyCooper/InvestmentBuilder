using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestmentBuilderServiceTestRunner
{
    /// <summary>
    /// GetAccountNames request.
    /// </summary>
    class GetAccountNamesRequest : RequestEndpoint<Dto, GetAccountNamesRequest.Result>
    {
        #region Internal classes

        public class AccountIdentifier
        {
            public string Name { get; set; }
            public int AccountId { get; set; }
        }

        public class Result : Dto
        {
            public IEnumerable<AccountIdentifier> AccountNames { get; set; }
        }

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public GetAccountNamesRequest() : base("GET_ACCOUNT_NAMES_REQUEST", "GET_ACCOUNT_NAMES_RESPONSE")
        {
        }
    }
}
