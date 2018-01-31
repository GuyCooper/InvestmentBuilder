using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilder;

namespace InvestmentBuilderService.Channels
{
    //ateTime valuationDate, DateTime transactionDate, string transactionType, string parameter, string dateRequestedFrom
    internal class DeleteTransactionRequestDto : Dto
    {
        public DateTime ValuationDate { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public string Parameter { get; set; }
    }

    internal class DeleteTransactionChannel : EndpointChannel<DeleteTransactionRequestDto>
    {
        private CashAccountTransactionManager _cashTransactionManager;

        public DeleteTransactionChannel(AccountService accountService,
                                        CashAccountTransactionManager cashTransactionManager) 
            : base("DELETE_TRANSACTION_REQUEST", "DELETE_TRANSACTION_RESPONSE", accountService)
        {
            _cashTransactionManager = cashTransactionManager;
        }

        public override Dto HandleEndpointRequest(UserSession userSession, DeleteTransactionRequestDto payload)
        {
            var token = GetCurrentUserToken(userSession);
            _cashTransactionManager.RemoveTransaction(token, payload.ValuationDate,
                                                payload.TransactionDate,
                                                payload.TransactionType,
                                                payload.Parameter);

            return new ResponseDto { Status = true };
        }
    }
}
