using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderCore;
using InvestmentBuilderService.Translators;

namespace InvestmentBuilderService.Channels
{
    internal class InvestmentSummaryRequestDto : Dto
    {
        public string AccountName { get; set; }
    }

    class GetInvestmentSummaryChannel : EndpointChannel<InvestmentSummaryRequestDto>
    {
        private IClientDataInterface _clientData;
        private InvestmentBuilder.InvestmentBuilder _builder;

        public GetInvestmentSummaryChannel(AccountService accountService, IClientDataInterface clientData, InvestmentBuilder.InvestmentBuilder builder) :
            base("GET_INVESTMENT_SUMMARY_REQUEST", "GET_INVESTMENT_SUMMARY_RESPONSE", accountService)
        {
            _clientData = clientData;
            _builder = builder;
        }

        public override Dto HandleEndpointRequest(UserSession userSession, InvestmentSummaryRequestDto payload)
        {
            var userToken = GetCurrentUserToken(userSession);
            var dtPrevious = _clientData.GetPreviousAccountValuationDate(userToken, userSession.ValuationDate);
            if (dtPrevious.HasValue)
            {
                return _builder.BuildAssetReport(userToken, dtPrevious.Value, false, null, null).ToInvestmentSummaryModel();
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
