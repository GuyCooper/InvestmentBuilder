using InvestmentBuilder;
using System.Collections.Generic;
using System.Linq;

namespace InvestmentBuilderService
{
    internal class BrokerManagerResponseDto : Dto
    {
        public IList<string> Brokers { get; set; }
    }

    /// <summary>
    /// Endpoint Channel class. returns all brokers
    /// </summary>
    internal class GetBrokersChannel : EndpointChannel<Dto, ChannelUpdater>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GetBrokersChannel(AccountService accountService, BrokerManager manager) : 
            base("GET_BROKERS_REQUEST", "GET_BROKERS_RESPONSE", accountService)
        {
            _manager = manager;
        }

        /// <summary>
        /// Method handles the get brokers request. returns a list of all supported
        /// brokers in the system.
        /// </summary>
        protected override Dto HandleEndpointRequest(UserSession userSession, Dto payload, ChannelUpdater updater)
        {
            return new BrokerManagerResponseDto
            {
                Brokers = _manager.GetBrokers().ToList()
            };            
        }

        #region Private Data Members

        private readonly BrokerManager _manager;

        #endregion
    }
}
