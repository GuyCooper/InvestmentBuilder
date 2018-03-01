using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderCore;

namespace InvestmentBuilderService.Channels
{
    internal class RecentValuationDates : Dto
    {
        public IEnumerable<string> RecentReports { get; set; }
    }

    internal class GetRecentValutationDatesChannel : EndpointChannel<Dto>
    {
        protected IClientDataInterface _clientData;

        public GetRecentValutationDatesChannel(AccountService accountService, IDataLayer dataLayer) 
            : base("GET_RECENT_VALUATION_DATES_REQUEST", "GET_RECENT_VALUATION_DATES_RESPONSE", accountService)
        {
            _clientData = dataLayer.ClientData;
        }

        public override Dto HandleEndpointRequest(UserSession userSession, Dto payload)
        {
            var userToken = GetCurrentUserToken(userSession);
            return new RecentValuationDates
            {
                RecentReports = _clientData.GetRecentValuationDates(userToken, DateTime.Now).Select(x =>
                                         x.ToShortDateString()).ToList()
            };
        }
    }
}
