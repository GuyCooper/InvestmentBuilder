
using InvestmentBuilderCore;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace InvestmentBuilderServiceTestRunner
{
    /// <summary>
    /// Runs full tests against investmentbuilder service process.
    /// </summary>
    class TestRunner
    {
        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        public TestRunner(Tests tests)
        {
            m_tests = tests;
        }

        /// <summary>
        /// Initialise the tests.
        /// </summary>
        public async void Initialise(bool startup)
        {
            logger.Info($"Running InvestmentBuilderTestRunner. Connecting to url: {m_url}");

            if (startup)
            {
                // Refresh the unit test database
                var ret = ProcessLauncher.RunProcessAndWaitForCompletion(@"C:\Projects\InvestmentBuilder\scripts\GenerateUnitTestDatabase.bat", "");
                logger.Info("return code from initialisedatabase : {0}", ret);

                //Before starting the investmentbuildersrvice change the investmentbuilderservice config to use the
                //unit test database.

                // Start the investmentbuilderservice.
                var commandLine = "\"dataSource=Data Source=DESKTOP-JJ9QOJA\\SQLEXPRESS;Initial Catalog=InvestmentBuilderUnitTest1;Integrated Security=True\"";
                ProcessLauncher.RunProcess(@"C:\Projects\InvestmentBuilder\InvestmentBuilderService\bin\Release\InvestmentBuilderService.exe", commandLine);

                // Give the process some time to start
                m_ServiceReadyEvent.WaitOne(30000);
            }

            logger.Info("InvestmentBuilderServiceRunning...");

            //Now try and connect to the service...
            //connect to the middleware
            logger.Info($"Connecting to server...");

            var connected = await m_connectionService.Connect(m_url, "user@test.com", "TestUserPassword123");
            if(connected)
            {
                logger.Info("Connected to server");
                m_connectionSucceded = await m_tests.RegisterEndpoints(m_connectionService);
            }

            if(!m_connectionSucceded)
            {
                logger.Error($"Failed to connect to service {m_url}");
            }

            m_ServiceReadyEvent.Set();
        }

        public void Run()
        {
            logger.Info("Waiting for investmentbuilderservice to signal ready...");

            m_ServiceReadyEvent.WaitOne();

            if (m_connectionSucceded)
            {
                logger.Info("Connection good and InvestmentBuilderService started. lets go...");
                m_tests.Run(m_connectionService);
            }

            logger.Info("TestRunner finished. Goodbye!");
        }

        #endregion

        #region Private Data

        private readonly ConnectionService m_connectionService = new ConnectionService();

        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly string m_url = "ws://localhost:8080/MWARE";

        private readonly ManualResetEvent m_ServiceReadyEvent = new ManualResetEvent(false);

        private bool m_connectionSucceded = false;

        private readonly Tests m_tests;

        #endregion
    }
}
