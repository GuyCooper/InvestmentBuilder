using InvestmentBuilderCore;

namespace InvestmentBuilderService.Channels
{
    /// <summary>
    /// UpdateCurrentAccount request dto.
    /// </summary>
    internal class UpdateCurrentAccountRequestDto : Dto
    {
        public AccountIdentifier AccountName { get; set; }
    }

    /// <summary>
    /// Class handles Updatecurrentaccount request.
    /// </summary>
    class UpdateCurrentAccountChannel : EndpointChannel<UpdateCurrentAccountRequestDto, ChannelUpdater>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public UpdateCurrentAccountChannel(ServiceAggregator aggregator) :
            base("UPDATE_CURRENT_ACCOUNT_REQUEST", "UPDATE_CURRENT_ACCOUNT_RESPONSE", aggregator)
        {
        }

        /// <summary>
        /// Handle update current account request.
        /// </summary>
        protected override Dto HandleEndpointRequest(UserSession userSession, UpdateCurrentAccountRequestDto payload, ChannelUpdater update)
        {
            GetCurrentUserToken(userSession, payload.AccountName);
            userSession.AccountName = payload.AccountName;
            return new ResponseDto { Status = true };
        }
    }
}
