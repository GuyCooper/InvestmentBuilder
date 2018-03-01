using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderCore;
using NLog;
using MarketDataServices;
using SQLServerDataLayer;
using PerformanceBuilderLib;
using Microsoft.Practices.Unity;
using InvestmentBuilder;
using System.IO;
using InvestmentBuilderService.Utils;
using InvestmentBuilderService.Session;
using System.Threading;

namespace InvestmentBuilderService
{
    class Program
    {
        private static Logger logger = LogManager.GetLogger("InvestmentBuilderService");
        /// <summary>
        /// console app for hosting the investment builder core. interfaces to the middleware
        /// service to allow remote clients access to the investment builder services
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            ContainerManager.RegisterType(typeof(IAuthorizationManager), typeof(SQLAuthorizationManager), true);
            ContainerManager.RegisterType(typeof(IConfigurationSettings), typeof(ConfigurationSettings), true, "InvestmentBuilderConfig.xml");
            ContainerManager.RegisterType(typeof(IMarketDataService), typeof(MarketDataService), true);
            MarketDataRegisterService.RegisterServices();
            ContainerManager.RegisterType(typeof(IDataLayer), typeof(SQLServerDataLayer.SQLServerDataLayer), true);
            ContainerManager.RegisterType(typeof(IInvestmentRecordDataManager), typeof(InvestmentRecordBuilder), true);
            ContainerManager.RegisterType(typeof(AccountService),true);
            ContainerManager.RegisterType(typeof(InvestmentBuilder.InvestmentBuilder), true);
            ContainerManager.RegisterType(typeof(PerformanceBuilderLib.PerformanceBuilder), true);
            ContainerManager.RegisterType(typeof(CashAccountTransactionManager), true);
            ContainerManager.RegisterType(typeof(CashFlowManager), true);
            ContainerManager.RegisterType(typeof(CashFlowManager), true);
            ContainerManager.RegisterType(typeof(IInvestmentReportWriter),typeof(InvestmentReportGenerator.InvestmentReportWriter), true);

            using (var child = ContainerManager.CreateChildContainer())
            { 
                var connectionSettings = new ConnectionSettings("Connections.xml");
                var authData = new SQLAuthData(ContainerManager.ResolveValue<IConfigurationSettings>());
                var authSession = new MiddlewareSession(connectionSettings.AuthServerConnection);
                var userManager = new UserSessionManager(authSession, authData);
                var serverSession = new MiddlewareSession(connectionSettings.ServerConnection);
                var endpointManager = new ChannelEndpointManager(serverSession, userManager);

                //now connect to servers and wait

                Console.WriteLine("connecting to servers...");
                ConnectToServers(userManager, endpointManager, child);

                ManualResetEvent closeEvent = new ManualResetEvent(false);
                Console.CancelKeyPress += (s, e) =>
                {
                    closeEvent.Set();
                    e.Cancel = true;
                };

                closeEvent.WaitOne();
            }
        }

        static async void ConnectToServers(EndpointManager authServer, EndpointManager server, IUnityContainer container)
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
        }
    }
}
