using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderService.Dtos;

namespace InvestmentBuilderService.Channels
{
    internal class GetCashFlowRequestDto : Dto
    {
        public string DateFrom { get; set; }
    }

    /// <summary>
    /// handler class for getting the cash flow details 
    /// </summary>
    internal class GetCashFlowChannel : EndpointChannel<GetCashFlowRequestDto, ChannelUpdater>
    {
        private CashFlowManager _cashFlowManager;

        public GetCashFlowChannel(AccountService accountService, CashFlowManager cashFlowManager) : 
            base("GET_CASH_FLOW_REQUEST", "GET_CASH_FLOW_RESPONSE", accountService)
        {
            _cashFlowManager = cashFlowManager;
        }

        protected override Dto HandleEndpointRequest(UserSession userSession, GetCashFlowRequestDto payload, ChannelUpdater update)
        {
            return CashFlowModelAndParams.GenerateCashFlowModelAndParams(userSession, _cashFlowManager, payload.DateFrom);
        }
    }
}
