﻿using System.Collections.Generic;
using System.Linq;
using InvestmentBuilderCore;

namespace InvestmentBuilderService.Channels
{
    internal class AccountNamesDto : Dto
    {
        public IEnumerable<AccountIdentifier> AccountNames { get; set; }
    }

    /// <summary>
    /// handler class for getting list of account names for current user
    /// </summary>
    class GetAccountNamesChannel : EndpointChannel<Dto, ChannelUpdater>
    {
        public GetAccountNamesChannel(AccountService accountService) :
            base("GET_ACCOUNT_NAMES_REQUEST", "GET_ACCOUNT_NAMES_RESPONSE", accountService)
        { }

        /// <summary>
        /// Method handles the request to get list of account names.
        /// </summary>
        protected override Dto HandleEndpointRequest(UserSession userSession, Dto payload, ChannelUpdater update)
        {
            return new AccountNamesDto
            {
                AccountNames = GetAccountService().GetAccountsForUser(userSession).ToList()
            };
        }
    }
}
