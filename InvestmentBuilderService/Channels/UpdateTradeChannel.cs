using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderService.Dtos;
using InvestmentBuilderService.Translators;

namespace InvestmentBuilderService.Channels
{
    class UpdateTradeChannel : EndpointChannel<TradeItemDto, ChannelUpdater>
    {
        private InvestmentBuilder.InvestmentBuilder _builder;
        public UpdateTradeChannel(InvestmentBuilder.InvestmentBuilder builder, AccountService accountService) :
            base("UPDATE_TRADE_REQUEST", "UPDATE_TRADE_RESPONSE", accountService)
        {
            _builder = builder;
        }

        protected override Dto HandleEndpointRequest(UserSession userSession, TradeItemDto payload, ChannelUpdater update)
        {
            var token = GetCurrentUserToken(userSession);
            var result = _builder.UpdateTrades(token, payload.ToInternalTrade(), userSession.UserPrices, null, null);
            //this command creates a new valuation snapshot. reset the valuation date to allow
            //any subsequent updates.
            userSession.ValuationDate = DateTime.Now;
            return new ResponseDto
            {
                Status = result
            };
        }
    }
}
