using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilder;
using InvestmentBuilderCore;

namespace InvestmentBuilderService.Channels
{
    /// <summary>
    /// Dto for returning the list of redemptions to a client.
    /// </summary>
    internal class RedemptionsDto : Dto
    {
        public List<Redemption> Redemptions { get; set; }
    }

    /// <summary>
    /// Handles the GetRedemptions request.
    /// </summary>
    class GetRedemptionsChannel : EndpointChannel<Dto, ChannelUpdater>
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public GetRedemptionsChannel(AccountService accountService, InvestmentBuilder.InvestmentBuilder builder) : 
            base("GET_REDEMPTIONS_REQUEST", "GET_REDEMPTIONS_RESPONSE", accountService)
        {
            _builder = builder;
        }

        #endregion

        #region Protected Overrides

        /// <summary>
        /// Handle the GetRedemptions request and return the list of redemptions for this users account.
        /// </summary>
        protected override Dto HandleEndpointRequest(UserSession userSession, Dto payload, ChannelUpdater updater)
        {
            var userToken = GetCurrentUserToken(userSession);
            return new RedemptionsDto
            {
                Redemptions = _builder.GetRedemptions(userToken, userSession.ValuationDate).ToList()
            };
        }

        #endregion

        #region Private Data

        private readonly InvestmentBuilder.InvestmentBuilder _builder;

        #endregion
    }
}
