﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestmentBuilderService.Channels
{
    internal class UpdateCurrentAccountRequestDto : Dto
    {
        public string AccountName { get; set; }
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
