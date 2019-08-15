using InvestmentBuilder;
using InvestmentBuilderAuditLogger;
using InvestmentBuilderCore;
using InvestmentBuilderService.Utils;
using MarketDataServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestmentBuilderService
{
    /// <summary>
    /// Class aggregates a number of injected services so they can be accessed from this class.
    /// </summary>
    class ServiceAggregator
    {
        #region Public Properties

        public AccountService AccountService { get; private set; }

        public IMessageLogger AuditLogger { get; private set; }

        public CashAccountTransactionManager CashTransactionManager { get; private set; }

        public CashFlowManager CashFlowManager { get; private set; }

        public InvestmentBuilder.InvestmentBuilder Builder { get; private set; }

        public PerformanceBuilderLib.PerformanceBuilder ChartBuilder { get; private set; }

        public IConfigurationSettings Settings { get; private set; }

        public IConnectionSettings ConnectionSettings { get; private set; }

        public BrokerManager BrokerManager { get; private set; }

        public IDataLayer DataLayer { get; private set; }

        public IMarketDataSource MarketDataSource { get; private set; }
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor. Inject all dependant services
        /// </summary>
        public ServiceAggregator(AccountService accountService, IMessageLogger auditLogger,
            CashAccountTransactionManager cashTransactionManager, CashFlowManager cashFlowManager,
            InvestmentBuilder.InvestmentBuilder builder, PerformanceBuilderLib.PerformanceBuilder chartBuilder,
            IConfigurationSettings settings, IConnectionSettings connectionSettings, BrokerManager brokerManager,
            IDataLayer dataLayer, IMarketDataSource marketDataSource)
        {
            AccountService = accountService;
            AuditLogger = auditLogger;
            CashTransactionManager = cashTransactionManager;
            CashFlowManager = cashFlowManager;
            Builder = builder;
            ChartBuilder = chartBuilder;
            Settings = settings;
            ConnectionSettings = connectionSettings;
            BrokerManager = brokerManager;
            DataLayer = dataLayer;
            MarketDataSource = marketDataSource;
        }

        #endregion
    }
}
