using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilder;
using InvestmentBuilderCore;

namespace InvestmentBuilderService.Channels
{
    internal class RemoveTransactionRequestDto : Dto
    {
        public string ValuationDate { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public string Parameter { get; set; }
    }

    internal abstract class RemoveTransactionChannel : EndpointChannel<RemoveTransactionRequestDto>
    {
        private CashAccountTransactionManager _cashTransactionManager;
        private InvestmentBuilder.InvestmentBuilder _builder;
        private IInvestmentRecordInterface _recordData;

        public RemoveTransactionChannel(string requestName, string responseName, 
                                    AccountService accountService,
                                    CashAccountTransactionManager cashTransactionManager)
            : base("REMOVE_TRANSACTION_REQUEST", "REMOVE_TRANSACTION_RESPONSE", accountService)
        {
            _cashTransactionManager = cashTransactionManager;
        }

        public override Dto HandleEndpointRequest(UserSession userSession, RemoveTransactionRequestDto payload)
        {
            var token = GetCurrentUserToken(userSession);
            var dtValuation = DateTime.Parse(payload.ValuationDate);
            var dtTransaction = DateTime.Parse(payload.TransactionDate);
            _cashTransactionManager.RemoveTransaction(token, dtValuation, dtTransaction, payload.TransactionType, payload.Parameter);
            return new ResponseDto { Status = true };
        }
    }
}
