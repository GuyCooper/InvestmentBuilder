using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestmentBuilderService.Channels
{
    internal class AccountNamesDto : Dto
    {
        public IEnumerable<string> AccountNames { get; set; }
    }

    class GetAccountNamesChannel : EndpointChannel<Dto>
    {
        public GetAccountNamesChannel(AccountService accountService) :
            base("GET_ACCOUNT_NAMES_REQUEST", "GET_ACCOUNT_NAMES_RESPONSE", accountService)
        { }

        public override Dto HandleEndpointRequest(UserSession userSession, Dto payload)
        {
            return new AccountNamesDto
            {
                AccountNames = GetAccountService().GetAccountsForUser(userSession).ToList()
            };
        }
    }
}
