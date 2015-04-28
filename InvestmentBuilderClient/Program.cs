using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using NLog;
using MarketDataServices;

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
            //registerunity dependencies.temporary,move to config file
            //ContainerManager.RegisterType(typeof(IMarketDataSource), typeof(AggregatedMarketDataSource));
            ContainerManager.RegisterType(typeof(IMarketDataSource), typeof(TestFileMarketDataSource));
            ContainerManager.RegisterType(typeof(IMarketDataService), typeof(MarketDataService));

            //var connectstr = @"Data Source=TRAVELPC\SQLEXPRESS;Initial Catalog=InvestmentBuilderTest;Integrated Security=True";
            var settings = new ConfigurationSettings();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //settings.DatasourceString
            using (var dataModel = new DataModel.InvestmentDataModel(settings.DatasourceString))
            {
                var mainView = new View.MainView(dataModel, settings);
                Application.Run(mainView);
            }
        }
    }
}
