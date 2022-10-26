
using InvestmentBuilderCore;
using System;

namespace InvestmentBuilderService.Channels
{
    /// <summary>
    /// Dto for RemoveRedemption request.
    /// </summary>
    internal class RemoveRedemptionRequestDto : Dto
    {
        public int RedemptionId { get; set; }
    }


    /// <summary>
    /// Handle the RemoveRedemption request
    /// </summary>
    internal class RemoveRedemptionChannel : EndpointChannel<RemoveRedemptionRequestDto, ChannelUpdater>
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public RemoveRedemptionChannel(ServiceAggregator aggregator) :
            base("REMOVE_REDEMPTION_REQUEST", "REMOVE_REDEMPTION_RESPONSE", aggregator)
        {
            _userAccountData = aggregator.DataLayer.UserAccountData;
        }

        #endregion

        protected override Dto HandleEndpointRequest(UserSession userSession, RemoveRedemptionRequestDto payload, ChannelUpdater updater)
        {
            var userToken = GetCurrentUserToken(userSession);
            var result = _userAccountData.RemoveRedemption(userToken,
                                                            payload.RedemptionId);

            return new Dto
            {
                IsError = !result,
                Error = "Unable to remove redemption"
            };
        }

        #region Private Data

        private readonly IUserAccountInterface _userAccountData;

        #endregion
    }
}
