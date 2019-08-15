using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InvestmentBuilderServiceTestRunner.Requests;

namespace InvestmentBuilderServiceTestRunner
{
    /// <summary>
    /// Class holds all the tests to run.
    /// </summary>
    internal class Tests
    {
        #region Public Methods

        /// <summary>
        /// Run all tests...
        /// </summary>
        public void Run(ConnectionService connectionService)
        {
            //m_getAccountNames.GetAccountNamesEmptyUser(connectionService);
            //m_getCashFlow.GetCashFlowEmptyUser(connectionService);
            //m_getInvestmentSummary.GetInvestmentSummaryEmptyUser(connectionService);
            //m_createAccount.AddNewAccount(connectionService);
            m_getPortfolio.GetPortfolioEmpty(connectionService);
        }

        /// <summary>
        /// Register all the delcared request endpoints in this instance
        /// </summary>
        public async Task<bool> RegisterEndpoints(ConnectionService connectionService)
        {
            var fields = this.GetType().GetFields();
            foreach (var field in fields)
            {
                if (IsEndpointChannelClass(field.FieldType).ToList().Count > 0)
                {
                    var endpoint = (IRequestEndpoint)field.GetValue(this);
                    //#pragma warning disable 4014
                    var subscribed = await connectionService.SubscribeToChannel(endpoint.ResponseChannel);
                    if (!subscribed)
                    {
                        logger.Error($"Failed to subscribe to channel {endpoint.ResponseChannel}");
                        return false;
                    }
                    //#pragma warning restore 4014
                }
            }
            return true;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Helper method for determining if an object is of type IEndpointChannel
        /// </summary>
        private IEnumerable<Type> IsEndpointChannelClass(Type objType)
        {
            return
            from interfaceType in objType.GetInterfaces()
            where interfaceType == typeof(IRequestEndpoint)
            select interfaceType;
        }

        #endregion

        #region Private Data

        private static Logger logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Public request endpoints

        public GetAccountNamesRequest m_getAccountNames = new GetAccountNamesRequest();
        public GetCashFlowRequest m_getCashFlow = new GetCashFlowRequest();
        public GetInvestmentSummaryRequest m_getInvestmentSummary = new GetInvestmentSummaryRequest();
        public CreateAccountChannelRequest m_createAccount = new CreateAccountChannelRequest();
        public UpdateAccountDetailsRequest m_updateAccountDetails = new UpdateAccountDetailsRequest();
        public GetPortfolioRequest m_getPortfolio = new GetPortfolioRequest();

        #endregion

    }
}
