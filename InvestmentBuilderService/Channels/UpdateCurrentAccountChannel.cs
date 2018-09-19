using InvestmentBuilderCore;

namespace InvestmentBuilderService.Channels
{
    internal class UpdateCurrentAccountRequestDto : Dto
    {
        public AccountIdentifier AccountName { get; set; }
    }

    class UpdateCurrentAccountChannel : EndpointChannel<UpdateCurrentAccountRequestDto, ChannelUpdater>
    {
        public UpdateCurrentAccountChannel(AccountService accountService) :
            base("UPDATE_CURRENT_ACCOUNT_REQUEST", "UPDATE_CURRENT_ACCOUNT_RESPONSE", accountService)
        { }

        protected override Dto HandleEndpointRequest(UserSession userSession, UpdateCurrentAccountRequestDto payload, ChannelUpdater update)
        {
            GetCurrentUserToken(userSession, payload.AccountName);
            userSession.AccountName = payload.AccountName;
            return new ResponseDto { Status = true };
        }
    }
}
