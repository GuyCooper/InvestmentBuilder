using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestmentBuilderService.Channels
{
    internal class CashFlowModelAndParams : Dto
    {
        public IEnumerable<CashFlowModel> CashFlows { get; set; }
        public IEnumerable<string> ReceiptParamTypes { get; set; }
        public IEnumerable<string> PaymentParamTypes { get; set; }
    }

    internal class GetCashFlowRequestDto : Dto
    {
        public string DateFrom { get; set; }
    }

    internal class GetCashFlowChannel : EndpointChannel<GetCashFlowRequestDto>
    {
        private CashFlowManager _cashFlowManager;

        public GetCashFlowChannel(AccountService accountService, CashFlowManager cashFlowManager) : 
            base("GET_CASH_FLOW_REQUEST", "GET_CASH_FLOW_RESPONSE", accountService)
        {
            _cashFlowManager = cashFlowManager;
        }

        public override Dto HandleEndpointRequest(UserSession userSession, GetCashFlowRequestDto payload)
        {
            return new CashFlowModelAndParams
            {
                CashFlows = _cashFlowManager.GetCashFlowModel(userSession, payload.DateFrom),
                ReceiptParamTypes = _cashFlowManager.GetReceiptParamTypes(),
                PaymentParamTypes = _cashFlowManager.GetPaymentParamTypes()
            };
        }
    }
}
