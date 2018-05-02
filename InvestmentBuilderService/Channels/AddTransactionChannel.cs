using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilder;
using InvestmentBuilderCore;
using InvestmentBuilderService.Dtos;

namespace InvestmentBuilderService.Channels
{
    internal class AddTransactionRequestDto : Dto
    {
        public string TransactionDate { get; set; }
        public string ParamType { get; set; }
        public string[] Parameter { get; set; }
        public double Amount { get; set; }
        public string DateRequestedFrom { get; set; }
    }

    internal abstract class AddTransactionChannel : EndpointChannel<AddTransactionRequestDto>
    {
        private CashAccountTransactionManager _cashTransactionManager;
        private CashFlowManager _cashFlowManager;

        public AddTransactionChannel(string requestName, string responseName, 
                                    AccountService accountService,
                                    CashAccountTransactionManager cashTransactionManager,
                                    CashFlowManager cashFlowManager)
            : base(requestName, responseName, accountService)
        {
            _cashTransactionManager = cashTransactionManager;
            _cashFlowManager = cashFlowManager;
        }

        public override Dto HandleEndpointRequest(UserSession userSession, AddTransactionRequestDto payload)
        {
            var token = GetCurrentUserToken(userSession);
            if (payload.TransactionDate != null && payload.Amount > 0)
            {
                var transactionDate = DateTime.Parse(payload.TransactionDate);
                //parameters list may be null which is valid
                var paramList = payload.Parameter ?? new List<string> { null }.ToArray();
                foreach (var param in paramList)
                {
                    _cashTransactionManager.AddTransaction(token, userSession.ValuationDate,
                                            transactionDate,
                                            payload.ParamType,
                                            param,
                                            payload.Amount);
                }
            }
            return CashFlowModelAndParams.GenerateCashFlowModelAndParams(userSession, _cashFlowManager, payload.DateRequestedFrom);
        }
    }

    internal class AddRecieptTransactionChannel : AddTransactionChannel
    {
        public AddRecieptTransactionChannel(AccountService accountService, CashAccountTransactionManager cashTransactionManager, 
                        CashFlowManager cashFlowManager) 
            : base("ADD_RECEIPT_TRANSACTION_REQUEST", "ADD_RECEIPT_TRANSACTION_RESPONSE", 
                    accountService, cashTransactionManager, cashFlowManager)
        {
        }
    }

    internal class AddPaymentTransactionChannel : AddTransactionChannel
    {
        public AddPaymentTransactionChannel(AccountService accountService, CashAccountTransactionManager cashTransactionManager, 
            CashFlowManager cashFlowManager)
            : base("ADD_PAYMENT_TRANSACTION_REQUEST", "ADD_PAYMENT_TRANSACTION_RESPONSE",
                    accountService, cashTransactionManager, cashFlowManager)
        {
        }
    }

}
