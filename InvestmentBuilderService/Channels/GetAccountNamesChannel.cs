using System.Collections.Generic;
using System.Linq;
using InvestmentBuilderCore;

namespace InvestmentBuilderService.Channels
{
    internal class AccountNamesDto : Dto
    {
        public IEnumerable<AccountIdentifier> AccountNames { get; set; }
        public bool IsTest { get; set; }
    }

    /// <summary>
    /// handler class for getting list of account names for current user
    /// </summary>
    class GetAccountNamesChannel : EndpointChannel<Dto, ChannelUpdater>
    {
        private readonly IConfigurationSettings _configuration;

        public GetAccountNamesChannel(ServiceAggregator aggregator, IConfigurationSettings configuration) :
            base("GET_ACCOUNT_NAMES_REQUEST", "GET_ACCOUNT_NAMES_RESPONSE", aggregator)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Method handles the request to get list of account names.
        /// </summary>
        protected override Dto HandleEndpointRequest(UserSession userSession, Dto payload, ChannelUpdater update)
        {
            return new AccountNamesDto
            {
                AccountNames = GetAccountService().GetAccountsForUser(userSession).ToList(),
                IsTest = _configuration.IsTest
            };
        }
    }
}
