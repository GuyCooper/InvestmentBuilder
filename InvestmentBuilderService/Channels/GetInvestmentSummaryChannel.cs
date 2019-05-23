using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderCore;
using InvestmentBuilderService.Translators;

namespace InvestmentBuilderService.Channels
{
    /// <summary>
    /// handler class for retreiving the investment summary
    /// </summary>
    class GetInvestmentSummaryChannel : EndpointChannel<Dto, ChannelUpdater>
    {
        private IClientDataInterface _clientData;
        private InvestmentBuilder.InvestmentBuilder _builder;

        public GetInvestmentSummaryChannel(AccountService accountService, IDataLayer dataLayer, InvestmentBuilder.InvestmentBuilder builder) :
            base("GET_INVESTMENT_SUMMARY_REQUEST", "GET_INVESTMENT_SUMMARY_RESPONSE", accountService)
        {
            _clientData = dataLayer.ClientData;
            _builder = builder;
        }

        /// <summary>
        /// Method called when a request for an investnent summary is received.
        /// </summary>
        protected override Dto HandleEndpointRequest(UserSession userSession, Dto payload, ChannelUpdater update)
        {
            var userToken = GetCurrentUserToken(userSession);
            var dtValuation = _clientData.GetLatestValuationDate(userToken);
            //var dtPrevious = _clientData.GetPreviousAccountValuationDate(userToken, userSession.ValuationDate);
            if (dtValuation.HasValue)
            {
                return _builder.BuildAssetReport(userToken, dtValuation.Value, false, null, null).ToInvestmentSummaryModel();
            }
            else
            {
                return new Dtos.InvestmentSummaryModel
                {
                    AccountName = userToken.Account,
                    ValuationDate = userSession.ValuationDate,
                    ValuePerUnit = "1"
                };
            }
        }
    }
}
