using System;
using InvestmentBuilderCore;
using NLog;
using MarketDataServices;
using SQLServerDataLayer;
using InvestmentBuilder;
using InvestmentBuilderService.Utils;
using InvestmentBuilderService.Session;
using InvestmentBuilderCore.Schedule;
using InvestmentBuilderAuditLogger;
using System.Collections.Generic;
using Unity;
using Microsoft.Extensions.CommandLineUtils;
using System.Linq;

namespace InvestmentBuilderService
{
    /// <summary>
    /// Main Entry point to InvestmentBuilderService.
    /// </summary>
    class InvestmentBuilderService
    {
        #region Main

        /// <summary>
        /// console app for hosting the investment builder core. interfaces to the middleware
        /// service to allow remote clients access to the investment builder services
        /// any configuration parameter overrides can be passed in on the command line
        /// should have the format name=value
        /// </summary>
        static void Main(string[] args)
        {
            string helpTemplate = "-h | --help";
            string certificateTemplate = "-c | --certificate";
            string debugTemplate = "-d | --debugmode";
            string configTemplate = "-c | --config";

            var application = new CommandLineApplication();

            var certificateOption =  application.Option(certificateTemplate, "Certificate file for config encryption", CommandOptionType.SingleValue);
            var debugOption = application.Option(debugTemplate, "Use debug parameters", CommandOptionType.NoValue);
            var configOption = application.Option(configTemplate, "Configuration overrides", CommandOptionType.MultipleValue);

            application.HelpOption(helpTemplate);


            logger.Info($"InvestmentBuilderService starting...");
            logger.Info($"command line {string.Join(",",args)}");

            application.OnExecute(() =>
            {
                   string certificate = certificateOption.HasValue() ? certificateOption.Value() : "";
                   var overrides = configOption.HasValue() ? configOption.Values
                   .Where(c => c.IndexOf('=') > 0)
                   .Select(c =>
                   {
                       var index = c.IndexOf('=');
                       var key = c.Substring(0, index);
                       var val = c.Substring(index + 1);
                       return Tuple.Create(key, val);
                   })
                   .ToList() : new List<Tuple<string, string>>();

                   var configfile = "InvestmentBuilderConfig";
                   var connectionsFile = "Connections";
                   var ext = string.IsNullOrEmpty(certificate) ? ".xml" : ".enc";

                   logger.Info("InvestmentBuilderService starting...");
                   ContainerManager.RegisterType(typeof(ScheduledTaskFactory));
                   ContainerManager.RegisterType(typeof(IAuthorizationManager), typeof(SQLAuthorizationManager));
                   ContainerManager.RegisterType(typeof(IConfigurationSettings), typeof(ConfigurationSettings), configfile + ext, overrides, certificate, debugOption.HasValue());
                   ContainerManager.RegisterType(typeof(IConnectionSettings), typeof(ConnectionSettings), connectionsFile + ext, certificate);
                   ContainerManager.RegisterType(typeof(IMarketDataService), typeof(MarketDataService));
                   MarketDataRegisterService.RegisterServices();
                   ContainerManager.RegisterType(typeof(IDataLayer), typeof(SQLServerDataLayer.SQLServerDataLayer));
                   ContainerManager.RegisterType(typeof(IInvestmentRecordDataManager), typeof(InvestmentRecordBuilder));
                   ContainerManager.RegisterType(typeof(AccountService));
                   ContainerManager.RegisterType(typeof(InvestmentBuilder.InvestmentBuilder));
                   ContainerManager.RegisterType(typeof(PerformanceBuilderLib.PerformanceBuilder));
                   ContainerManager.RegisterType(typeof(CashAccountTransactionManager));
                   ContainerManager.RegisterType(typeof(CashFlowManager));
                   ContainerManager.RegisterType(typeof(IInvestmentReportWriter), typeof(InvestmentReportGenerator.InvestmentReportWriter));
                   ContainerManager.RegisterType(typeof(IMessageLogger), typeof(SQLiteAuditLogger));
                   ContainerManager.RegisterType(typeof(ServiceAggregator));

                   using (var child = ContainerManager.CreateChildContainer())
                   {
                       var dataLayer = ContainerManager.ResolveValue<IDataLayer>();
                       var configSettings = ContainerManager.ResolveValue<IConfigurationSettings>();
                       var connectionSettings = ContainerManager.ResolveValue<IConnectionSettings>();
                       var authData = new SQLAuthData(configSettings.AuthDatasourceString);
                       var authSession = new MiddlewareSession(connectionSettings.AuthServerConnection, "InvestmentBuilder-AuthService");
                       var accountManager = ContainerManager.ResolveValue<AccountManager>();
                       var userManager = new UserSessionManager(authSession, authData, accountManager, dataLayer.UserAccountData);
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
                   return 0;
            });

            try
            {
                application.Execute(args);
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
