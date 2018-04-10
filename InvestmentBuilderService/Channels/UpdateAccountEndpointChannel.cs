using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestmentBuilderService.Channels
{
    internal class UpdateAccountRequestDto : Dto
    {
        public string AccountName { get; set; }
    }

    class UpdateAccountEndpointChannel : EndpointChannel<UpdateAccountRequestDto>
    {
        public UpdateAccountEndpointChannel(AccountService accountService) :
            base("UPDATE_ACCOUNT_REQUEST", "UPDATE_ACCOUNT_RESPONSE", accountService)
        { }

        public override Dto HandleEndpointRequest(UserSession userSession, UpdateAccountRequestDto payload)
        {
            GetCurrentUserToken(userSession, payload.AccountName);
            userSession.AccountName = payload.AccountName;
            return new ResponseDto { Status = true };
        }
    }
}
