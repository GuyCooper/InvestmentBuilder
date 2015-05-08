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

namespace InvestmentBuilderClient
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //registerunity dependencies.
            //ContainerManager.RegisterType(typeof(IMarketDataSource), typeof(AggregatedMarketDataSource));
            ContainerManager.RegisterType(typeof(IMarketDataSource), typeof(TestFileMarketDataSource), false);
            ContainerManager.RegisterType(typeof(IMarketDataService), typeof(MarketDataService), false);
            ContainerManager.RegisterType(typeof(IConfigurationSettings), typeof(ConfigurationSettings), false, "InvestmentBuilderConfig.xml");
            //todo,use servicelocator
            ContainerManager.RegisterType(typeof(IDataLayer), typeof(SQLServerDataLayer.SQLServerDataLayer), false);
            ContainerManager.RegisterType(typeof(InvestmentBuilder.InvestmentBuilder), typeof(InvestmentBuilder.InvestmentBuilder), false);
            ContainerManager.RegisterType(typeof(PerformanceBuilder), typeof(PerformanceBuilder), false);

            //var connectstr = @"Data Source=TRAVELPC\SQLEXPRESS;Initial Catalog=InvestmentBuilderTest;Integrated Security=True";
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //settings.DatasourceString
            var settings = ContainerManager.ResolveValue<IConfigurationSettings>();

            using (var dataModel = new DataModel.InvestmentDataModel(ContainerManager.ResolveValue<IDataLayer>()))
            {
                var mainView = new View.MainView(dataModel, settings);
                Application.Run(mainView);
            }
        }
    }
}
