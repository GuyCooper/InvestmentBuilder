using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using NLog;
using MarketDataServices;
using InvestmentBuilderCore;
using SQLServerDataLayer;
using PerformanceBuilderLib;
using Microsoft.Practices.Unity;
using InvestmentBuilder;

namespace InvestmentBuilderClient
{
    static class Program
    {
        static bool UseTestDatasource = false;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //registerunity dependencies.
            ContainerManager.RegisterType(typeof(IAuthorizationManager), typeof(SQLAuthorizationManager), true);
            ContainerManager.RegisterType(typeof(IConfigurationSettings), typeof(ConfigurationSettings), true, "InvestmentBuilderConfig.xml");
            ContainerManager.RegisterType(typeof(IMarketDataService), typeof(MarketDataService), true);
            if (UseTestDatasource == true)
            {
                ContainerManager.RegisterType(typeof(IMarketDataSource), typeof(TestFileMarketDataSource), true, "testMarketData.txt");
            }
            else
            {
                MarketDataRegisterService.RegisterServices();
            }
            //todo,use servicelocator
            ContainerManager.RegisterType(typeof(IDataLayer), typeof(SQLServerDataLayer.SQLServerDataLayer), true);
            //ContainerManager.RegisterType(typeof(InvestmentBuilder.InvestmentBuilder), typeof(InvestmentBuilder.InvestmentBuilder), true);
            //ContainerManager.RegisterType(typeof(PerformanceBuilder), typeof(PerformanceBuilder), true);
            //ContainerManager.RegisterType(typeof(DataModel.InvestmentDataModel), typeof(DataModel.InvestmentDataModel), true);
            ContainerManager.RegisterType(typeof(View.MainView), typeof(View.MainView), true);
            //ContainerManager.RegisterType(typeof(InvestmentBuilder.BrokerManager), typeof(InvestmentBuilder.BrokerManager), true);
            //ContainerManager.RegisterType(typeof(InvestmentBuilder.CashAccountTransactionManager), typeof(InvestmentBuilder.CashAccountTransactionManager), true);
            ContainerManager.RegisterType(typeof(IInvestmentReportWriter), typeof(InvestmentReportGenerator.InvestmentReportWriter), true);
            ContainerManager.RegisterType(typeof(IInvestmentRecordDataManager), typeof(InvestmentRecordBuilder), true);
            //var connectstr = @"Data Source=TRAVELPC\SQLEXPRESS;Initial Catalog=InvestmentBuilderTest;Integrated Security=True";
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (var child = ContainerManager.CreateChildContainer())
            {
                Application.Run(ContainerManager.ResolveValueOnContainer<View.MainView>(child));
            }
        }
    }
}
