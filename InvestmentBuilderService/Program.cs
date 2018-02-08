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

namespace InvestmentBuilderService
{
    class Program
    {
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

            using (var child = ContainerManager.CreateChildContainer())
            {
                var connectionSettings = new ConnectionSettings("Connections.xml");
                var authData = new SQLAuthData(ContainerManager.ResolveValue<IConfigurationSettings>());
                var authSession = new MiddlewareSession(connectionSettings.AuthServerConnection);
                var userManager = new UserSessionManager(authSession, authData);
                var serverSession = new MiddlewareSession(connectionSettings.ServerConnection);
                var endpointManager = new ChannelEndpointManager(serverSession, userManager);

                endpointManager.RegisterEndpointChannels();
                //now connect to servers and wait
                ConnectToServers(userManager, endpointManager);
            }
        }

        static async void ConnectToServers(EndpointManager authServer, EndpointManager server)
        {
            var connected = await authServer.Connect();
            if(connected == false)
            {
                return;
            }
            connected = await server.Connect();
        }
    }
}
