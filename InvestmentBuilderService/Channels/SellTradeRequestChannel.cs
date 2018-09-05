using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderCore;
using InvestmentBuilderService.Dtos;
using InvestmentBuilderService.Translators;

namespace InvestmentBuilderService.Channels
{
    internal class SellTradeRequestDto : Dto
    {
        public string TradeName { get; set; }
    }

    internal class SellTradeRequestChannel : EndpointChannel<SellTradeRequestDto, ChannelUpdater>
    {
        private IClientDataInterface _clientData;
        private InvestmentBuilder.InvestmentBuilder _builder;

        public SellTradeRequestChannel(AccountService accountService, 
                                       IDataLayer dataLayer,
                                       InvestmentBuilder.InvestmentBuilder builder) : 
            base("SELL_TRADE_REQUEST", "SELL_TRADE_RESPONSE", accountService)
        {
            _clientData = dataLayer.ClientData;
            _builder = builder;
        }

        protected override Dto HandleEndpointRequest(UserSession userSession, SellTradeRequestDto payload, ChannelUpdater update)
        {
            var token = GetCurrentUserToken(userSession);
            var tradeItem = _clientData.GetTradeItem(token, payload.TradeName);
            var result = false;
            if (tradeItem != null)
            {
                result = _builder.UpdateTrades(token, tradeItem.ToInternalTrade(TransactionType.Sell), null, null);
            }

            return new ResponseDto
            {
                Status = result
            };
        }
    }
}
