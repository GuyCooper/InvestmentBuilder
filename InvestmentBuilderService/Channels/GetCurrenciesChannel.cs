﻿using System.Collections.Generic;
using System.Linq;

namespace InvestmentBuilderService.Channels
{
    //DTO class for returning a list of currencies
    internal class CurrenciesResponseDto : Dto
    {
        public IList<string> Currencies { get; set; }
    }

    /// <summary>
    /// Channel handler class for retriving a list of available currencies
    /// </summary>
    class GetCurrenciesChannel : EndpointChannel<Dto, ChannelUpdater>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public GetCurrenciesChannel(AccountService accountService, InvestmentBuilder.InvestmentBuilder builder) : 
            base("GET_CURRENCIES_REQUEST", "GET_CURRENCIES_RESPONSE", accountService)
        {
            _builder = builder;
        }

        /// <summary>
        /// Method handles the getcurrencies request
        /// </summary>
        protected override Dto HandleEndpointRequest(UserSession userSession, Dto payload, ChannelUpdater updater)
        {
            return new CurrenciesResponseDto
            {
                Currencies = _builder.GetAllCurrencies().ToList()
            };
        }

        #region Private Data Members

        private readonly InvestmentBuilder.InvestmentBuilder _builder;
        
        #endregion
    }
}
