﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderCore;
using InvestmentBuilderService.Dtos;
using InvestmentBuilderService.Translators;

namespace InvestmentBuilderService.Channels
{
    /// <summary>
    /// SellTrade request dto.
    /// </summary>
    internal class SellTradeRequestDto : Dto
    {
        public string TradeName { get; set; }
    }

    /// <summary>
    /// Handler class for selling a trade.
    /// </summary>
    internal class SellTradeRequestChannel : EndpointChannel<SellTradeRequestDto, ChannelUpdater>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public SellTradeRequestChannel(ServiceAggregator aggregator) : 
            base("SELL_TRADE_REQUEST", "SELL_TRADE_RESPONSE", aggregator)
        {
            _clientData = aggregator.DataLayer.ClientData;
            _builder = aggregator.Builder;
        }

        /// <summary>
        /// Handle sell trade request.
        /// </summary>
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

        #region Private Data

        private readonly IClientDataInterface _clientData;
        private readonly InvestmentBuilder.InvestmentBuilder _builder;

        #endregion

    }
}
