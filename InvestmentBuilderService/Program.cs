using System;
using InvestmentBuilderCore;
using NLog;
using MarketDataServices;
using SQLServerDataLayer;
using Microsoft.Practices.Unity;
using InvestmentBuilder;
using InvestmentBuilderService.Utils;
using InvestmentBuilderService.Session;
using System.Threading;
using InvestmentBuilderCore.Schedule;
using System.Threading.Tasks;

namespace InvestmentBuilderService
{
    /// <summary>
    /// Main Entry point to InvestmentBuilderService.
    /// </summary>
    class Program
    {
        #region Main

        /// <summary>
        /// console app for hosting the investment builder core. interfaces to the middleware
        /// service to allow remote clients access to the investment builder services
        /// </summary>
        static void Main(string[] args)
        {
            try
            {
                logger.Info("InvestmentBuilderService starting...");
                ContainerManager.RegisterType(typeof(ScheduledTaskFactory), true);
                ContainerManager.RegisterType(typeof(IAuthorizationManager), typeof(SQLAuthorizationManager), true);
                ContainerManager.RegisterType(typeof(IConfigurationSettings), typeof(ConfigurationSettings), true, "InvestmentBuilderConfig.xml");
                ContainerManager.RegisterType(typeof(IConnectionSettings), typeof(ConnectionSettings), true, "Connections.xml");
                ContainerManager.RegisterType(typeof(IMarketDataService), typeof(MarketDataService), true);
                MarketDataRegisterService.RegisterServices();
                ContainerManager.RegisterType(typeof(IDataLayer), typeof(SQLServerDataLayer.SQLServerDataLayer), true);
                ContainerManager.RegisterType(typeof(IInvestmentRecordDataManager), typeof(InvestmentRecordBuilder), true);
                ContainerManager.RegisterType(typeof(AccountService), true);
                ContainerManager.RegisterType(typeof(InvestmentBuilder.InvestmentBuilder), true);
                ContainerManager.RegisterType(typeof(PerformanceBuilderLib.PerformanceBuilder), true);
                ContainerManager.RegisterType(typeof(CashAccountTransactionManager), true);
                ContainerManager.RegisterType(typeof(CashFlowManager), true);
                ContainerManager.RegisterType(typeof(IInvestmentReportWriter), typeof(InvestmentReportGenerator.InvestmentReportWriter), true);

                using (var child = ContainerManager.CreateChildContainer())
                {
                    var configSettings = ContainerManager.ResolveValue<IConfigurationSettings>();
                    var connectionSettings = ContainerManager.ResolveValue<IConnectionSettings>();                    

                    var authData = new SQLAuthData(configSettings.AuthDatasourceString);
                    var authSession = new MiddlewareSession(connectionSettings.AuthServerConnection, "InvestmentBuilder-AuthService");
                    var userManager = new UserSessionManager(authSession, authData, ContainerManager.ResolveValue<AccountManager>());
                    var serverSession = new MiddlewareSession(connectionSettings.ServerConnection, "InvestmentBuilder-Channels");
                    var endpointManager = new ChannelEndpointManager(serverSession, userManager);

                    //now connect to servers and wait

                    logger.Info("Connecting to servers");

                    var schedulerFactory = ContainerManager.ResolveValueOnContainer<ScheduledTaskFactory>(child);

                    ConnectToServers(userManager, endpointManager, child);

                    logger.Info("InvestmentBuilderService Started.");

                    
                    var scheduler = new Scheduler(schedulerFactory, configSettings.ScheduledTasks);
                    scheduler.Run();

                    logger.Info("Shutting down InvestmentBuilderService");


                    authSession.Dispose();
                    serverSession.Dispose();
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Attach the endpoints to the correct server connections. 
        /// </summary>
        private static async void ConnectToServers(EndpointManager authServer, EndpointManager server, IUnityContainer container)
        {
            var connected = await authServer.Connect();
            if (connected == false)
            {
                logger.Log(LogLevel.Error, "failed to connect to auth server");
                return;
            }

            //now register the authserver as an auth server
            connected = await authServer.GetSession().RegisterAuthenticationServer("InvestmentBuilder");
            if(connected == false)
            {
                logger.Log(LogLevel.Error, "failed to register auth server");
                return;
            }

            connected = await server.Connect();

            if (connected == false)
            {
                logger.Log(LogLevel.Error, "failed to connect to channel server");
            }

            if(connected == true)
            {
                //now register all the channels that will be used by the service
                authServer.RegisterChannels(container);
                server.RegisterChannels(container);
                Console.WriteLine("connection succeded!");
            }

            return;
        }

        #endregion

        #region Private Data

        private static Logger logger = LogManager.GetLogger("InvestmentBuilderService");

        #endregion
    }
}
