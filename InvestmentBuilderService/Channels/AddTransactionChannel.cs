using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilder;
using InvestmentBuilderCore;

namespace InvestmentBuilderService.Channels
{
    internal class AddTransactionRequestDto : Dto
    {
        public string TransactionDate { get; set; }
        public string ParamType { get; set; }
        public string Parameter { get; set; }
        public double Amount { get; set; }
        public string DateRequestedFrom { get; set; }
    }

    internal abstract class AddTransactionChannel : EndpointChannel<AddTransactionRequestDto>
    {
        private CashAccountTransactionManager _cashTransactionManager;
        private InvestmentBuilder.InvestmentBuilder _builder;
        private IInvestmentRecordInterface _recordData;

        private string _transactionMnemomic;

        public AddTransactionChannel(string requestName, string responseName, 
                                    AccountService accountService,
                                    CashAccountTransactionManager cashTransactionManager,
                                    string transactionMnemomic) 
            : base(requestName, responseName, accountService)
        {
            _cashTransactionManager = cashTransactionManager;
            _transactionMnemomic = transactionMnemomic;
        }

        public override Dto HandleEndpointRequest(UserSession userSession, AddTransactionRequestDto payload)
        {
            var token = GetCurrentUserToken(userSession);
            var success = false;
            if (payload.TransactionDate != null && payload.Amount > 0)
            {
                var transactionDate = DateTime.Parse(payload.TransactionDate);
                if (payload.Parameter == "ALL")
                {
                    var latestRecordDate = _recordData.GetLatestRecordInvestmentValuationDate(token) ?? DateTime.Today;

                    var parameters = _builder.GetParametersForTransactionType(token, latestRecordDate, payload.ParamType).ToList();
                    foreach (var parameter in parameters)
                    {
                        _cashTransactionManager.AddTransaction(token, userSession.ValuationDate,
                                                transactionDate,
                                                payload.ParamType,
                                                parameter,
                                                payload.Amount);

                    }
                }
                else
                {
                    _cashTransactionManager.AddTransaction(token, userSession.ValuationDate,
                                            transactionDate,
                                            payload.ParamType,
                                            payload.Parameter,
                                            payload.Amount);
                }
                success = true;
            }
            return new ResponseDto { Status = success };
        }
    }

    internal class AddRecieptTransactionChannel : AddTransactionChannel
    {
        public AddRecieptTransactionChannel(AccountService accountService, CashAccountTransactionManager cashTransactionManager) 
            : base("ADD_RECEIPT_TRANSACTION_REQUEST", "ADD_RECEIPT_TRANSACTION_RESPONSE", 
                    accountService, cashTransactionManager, cashTransactionManager.ReceiptMnemomic)
        {
        }
    }

    internal class AddPaymentTransactionChannel : AddTransactionChannel
    {
        public AddPaymentTransactionChannel(AccountService accountService, CashAccountTransactionManager cashTransactionManager)
            : base("ADD_PAYMENT_TRANSACTION_REQUEST", "ADD_PAYMENT_TRANSACTION_RESPONSE",
                    accountService, cashTransactionManager, cashTransactionManager.PaymentMnemomic)
        {
        }
    }

}
