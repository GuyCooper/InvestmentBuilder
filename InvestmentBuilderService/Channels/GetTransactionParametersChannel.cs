using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderCore;

namespace InvestmentBuilderService.Channels
{
    internal class TransactionParametersRequestDto : Dto
    {
        public string ParameterType { get; set; }
    }

    internal class TransactionParametersResponseDto : Dto
    {
        public IEnumerable<string> Parameters { get; set; }
    }

    /// <summary>
    /// handler class for retreiving a list of transaction parameters
    /// </summary>
    internal class GetTransactionParametersChannel : EndpointChannel<TransactionParametersRequestDto, ChannelUpdater>
    {
        private IInvestmentRecordInterface _recordData;
        private InvestmentBuilder.InvestmentBuilder _builder;

        public GetTransactionParametersChannel(AccountService accountService, 
                                               IDataLayer dataLayer,
                                               InvestmentBuilder.InvestmentBuilder builder) 
            : base("GET_TRANSACTION_PARAMETERS_REQUEST", "GET_TRANSACTION_PARAMETERS_RESPONSE", accountService)
        {
            _recordData = dataLayer.InvestmentRecordData;
            _builder = builder;
        }

        protected override Dto HandleEndpointRequest(UserSession userSession, TransactionParametersRequestDto payload, ChannelUpdater update)
        {
            var token = GetCurrentUserToken(userSession);
            var latestRecordDate = _recordData.GetLatestRecordInvestmentValuationDate(token) ?? DateTime.Today;

            var parameters = _builder.GetParametersForTransactionType(token, latestRecordDate, payload.ParameterType).ToList();
            if (parameters.Count == 0)
            {
                parameters.Add(payload.ParameterType);
            }
            else
            {
                parameters.Add("ALL");
            }

            return new TransactionParametersResponseDto
            {
                Parameters = parameters
            };
    }
    }
}
