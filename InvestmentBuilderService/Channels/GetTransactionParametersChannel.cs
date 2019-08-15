using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderCore;

namespace InvestmentBuilderService.Channels
{
    /// <summary>
    /// request dto for Transaction parameters.
    /// </summary>
    internal class TransactionParametersRequestDto : Dto
    {
        public string ParameterType { get; set; }
    }

    /// <summary>
    /// Response dto for Transaction parameters.
    /// </summary>
    internal class TransactionParametersResponseDto : Dto
    {
        public IEnumerable<string> Parameters { get; set; }
    }

    /// <summary>
    /// handler class for retreiving a list of transaction parameters
    /// </summary>
    internal class GetTransactionParametersChannel : EndpointChannel<TransactionParametersRequestDto, ChannelUpdater>
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="aggregator"></param>
        public GetTransactionParametersChannel(ServiceAggregator aggregator)
            : base("GET_TRANSACTION_PARAMETERS_REQUEST", "GET_TRANSACTION_PARAMETERS_RESPONSE", aggregator)
        {
            _recordData = aggregator.DataLayer.InvestmentRecordData;
            _builder = aggregator.Builder;
        }

        #endregion

        #region Protected Overrides

        /// <summary>
        /// Handle GetTransaction parameters request.
        /// </summary>
        /// <param name="userSession"></param>
        /// <param name="payload"></param>
        /// <param name="update"></param>
        /// <returns></returns>
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

        #endregion

        #region Private Data

        private readonly IInvestmentRecordInterface _recordData;
        private readonly InvestmentBuilder.InvestmentBuilder _builder;

        #endregion
    }
}
