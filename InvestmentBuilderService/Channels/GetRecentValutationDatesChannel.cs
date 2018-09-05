using System;
using System.Collections.Generic;
using System.Linq;
using InvestmentBuilderCore;

namespace InvestmentBuilderService.Channels
{
    internal class RecentValuationDatesListDto : Dto
    {
        public IEnumerable<string> Dates { get; set; }
    }

    /// <summary>
    /// handler class for retreiving list of recent valuation dates
    /// </summary>
    internal class GetRecentValutationDatesChannel : EndpointChannel<Dto, ChannelUpdater>
    {
        protected IClientDataInterface _clientData;

        public GetRecentValutationDatesChannel(AccountService accountService, IDataLayer dataLayer) 
            : base("GET_RECENT_VALUATION_DATES_REQUEST", "GET_RECENT_VALUATION_DATES_RESPONSE", accountService)
        {
            _clientData = dataLayer.ClientData;
        }

        protected override Dto HandleEndpointRequest(UserSession userSession, Dto payload, ChannelUpdater update)
        {
            var userToken = GetCurrentUserToken(userSession);
            return new RecentValuationDatesListDto
            {
                Dates = _clientData.GetRecentValuationDates(userToken, DateTime.Now).Select(x =>
                                         x.ToShortDateString()).ToList()
            };
        }
    }
}
