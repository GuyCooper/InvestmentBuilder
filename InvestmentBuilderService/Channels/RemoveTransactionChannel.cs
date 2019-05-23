using InvestmentBuilder;
using InvestmentBuilderService.Dtos;

namespace InvestmentBuilderService.Channels
{
    /// <summary>
    /// Dto for RemoveTransaction request.
    /// </summary>
    internal class RemoveTransactionRequestDto : Dto
    {
        public int TransactionID { get; set; }
    }

    /// <summary>
    /// handler class for removing a transaction from the database
    /// </summary>
    internal class RemoveTransactionChannel : EndpointChannel<RemoveTransactionRequestDto, ChannelUpdater>
    {
        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        public RemoveTransactionChannel(AccountService accountService,
                                        CashAccountTransactionManager cashTransactionManager)
            : base("REMOVE_TRANSACTION_REQUEST", "REMOVE_TRANSACTION_RESPONSE", accountService)
        {
            _cashTransactionManager = cashTransactionManager;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Handle the remove transaction  request.
        /// </summary>
        protected override Dto HandleEndpointRequest(UserSession userSession, RemoveTransactionRequestDto payload, ChannelUpdater update)
        {
            var token = GetCurrentUserToken(userSession);
            _cashTransactionManager.RemoveTransaction(token, payload.TransactionID);
            return new ResponseDto { Status = true };
        }

        #endregion

        #region Private Data

        private readonly CashAccountTransactionManager _cashTransactionManager;

        #endregion
    }
}
