﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderService.Dtos;
using InvestmentBuilderService.Translators;

namespace InvestmentBuilderService.Channels
{
    /// <summary>
    /// Class handles request to update a trade (investment)
    /// </summary>
    class UpdateTradeChannel : EndpointChannel<TradeItemDto, ChannelUpdater>
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public UpdateTradeChannel(ServiceAggregator aggregator) :
            base("UPDATE_TRADE_REQUEST", "UPDATE_TRADE_RESPONSE", aggregator)
        {
            _builder = aggregator.Builder;
        }

        #endregion

        #region Protected Overrides

        /// <summary>
        /// Handles request to update a trade (investment).
        /// </summary>
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

        #endregion

        #region Private Data

        private readonly InvestmentBuilder.InvestmentBuilder _builder;

        #endregion
    }
}
