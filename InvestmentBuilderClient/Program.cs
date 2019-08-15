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
using System.IO;
using InvestmentBuilderClient.View;

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
                string testDataFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "InvestmentRecordBuilder", "testMarketData.txt");
                ContainerManager.RegisterType(typeof(IMarketDataSource), typeof(TestFileMarketDataSource), true, testDataFile);
            }
            else
            {
                MarketDataRegisterService.RegisterServices();
            }
            //todo,use servicelocator
            ContainerManager.RegisterType(typeof(IDataLayer), typeof(SQLServerDataLayer.SQLServerDataLayer), true);
            ContainerManager.RegisterType(typeof(View.MainView), typeof(View.MainView), true);
            ContainerManager.RegisterType(typeof(IInvestmentReportWriter), typeof(InvestmentReportGenerator.InvestmentReportWriter), true);
            ContainerManager.RegisterType(typeof(IInvestmentRecordDataManager), typeof(InvestmentRecordBuilder), true);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var login = new LoginView("guy@guycooper.plus.com", "rangers");
            if (login.ShowDialog() == DialogResult.OK)
            {
                using (var child = ContainerManager.CreateChildContainer())
                {
                    var mainView = ContainerManager.ResolveValueOnContainer<View.MainView>(child);
                    mainView.UpdateUser(login.GetUserName());
                    Application.Run(mainView);
                }
            }
        }
    }
}
