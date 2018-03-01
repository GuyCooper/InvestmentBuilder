﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestmentBuilderService.Channels
{
    internal class InvestmentPriceUpdateDto : Dto
    {
        public string Investment { get; set; }
        public double Price { get; set; }
    }

    internal class UpdateManualPriceChannel : EndpointChannel<InvestmentPriceUpdateDto>
    {
        public UpdateManualPriceChannel(AccountService accountService) : 
            base("UPDATE_INVESTMENT_PRICE_REQUEST", "UPDATE_INVESTMENT_PRICE_RESPONSE", accountService)
        {
        }

        public override Dto HandleEndpointRequest(UserSession userSession, InvestmentPriceUpdateDto payload)
        {
            if(userSession.UserPrices.ContainsKey(payload.Investment) == false)
            {
                userSession.UserPrices.Add(payload.Investment, payload.Price);
            }
            else
            {
                userSession.UserPrices[payload.Investment] = payload.Price;
            }
            return new ResponseDto { Status = true };
        }
    }
}