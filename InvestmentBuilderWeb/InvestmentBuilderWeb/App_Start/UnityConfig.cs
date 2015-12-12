using System;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using InvestmentBuilderWeb.Services;
using Microsoft.AspNet.Identity;
using InvestmentBuilderWeb.Models;
using Microsoft.Owin.Security;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using InvestmentBuilderCore;
using InvestmentBuilder;
using SQLServerDataLayer;
using MarketDataServices;
using PerformanceBuilderLib;
using InvestmentBuilderWeb.Interfaces;
using System.IO;

namespace InvestmentBuilderWeb.App_Start
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your types here

            container.RegisterType<ApplicationDbContext>();
            container.RegisterType<ApplicationSignInManager>();
            container.RegisterType<ApplicationUserManager>();
            container.RegisterType<IAuthenticationManager>(
                            new InjectionFactory(c => HttpContext.Current.GetOwinContext().Authentication));

            container.RegisterType<IUserStore<ApplicationUser>, UserStore<ApplicationUser>>(
            new InjectionConstructor(typeof(ApplicationDbContext)));

            //we only want single instances of the investmenrecord app specific classes generated
            container.RegisterType<IAuthorizationManager, SQLAuthorizationManager>(new ContainerControlledLifetimeManager());
            //container.RegisterType<IMarketDataSource, AggregatedMarketDataSource>(new ContainerControlledLifetimeManager());
            container.RegisterType<IMarketDataService, MarketDataService>(new ContainerControlledLifetimeManager());

            string testDataFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "InvestmentRecordBuilder", "testMarketData.txt");
            container.RegisterType<IMarketDataSource, TestFileMarketDataSource>(new ContainerControlledLifetimeManager(), new InjectionConstructor(testDataFile));

            //%userprofile%\documents\iisexpress\config\applcicationhost.config

            string configFile= Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),"InvestmentRecordBuilder", "InvestmentBuilderWebConfig.xml");
            container.RegisterType<IConfigurationSettings, ConfigurationSettings>(new ContainerControlledLifetimeManager(), new InjectionConstructor(configFile));
            //todo,use servicelocator
            container.RegisterType<IDataLayer, SQLServerDataLayer.SQLServerDataLayer>(new ContainerControlledLifetimeManager());
            container.RegisterType<InvestmentBuilder.InvestmentBuilder>(new ContainerControlledLifetimeManager());
            container.RegisterType<PerformanceBuilder>(new ContainerControlledLifetimeManager());

            container.RegisterType<IApplicationSessionService, InvestmentRecordSessionService>(new ContainerControlledLifetimeManager());
        }
    }
}
