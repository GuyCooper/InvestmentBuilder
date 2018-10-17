
using InvestmentBuilderCore;
using NLog;
using SQLServerDataLayer;

namespace UserManagementService
{
    /// <summary>
    /// UserManagementService class
    /// </summary>
    class UserManagementService
    {
        /// <summary>
        /// Main entry point
        /// </summary>
        static void Main(string[] args)
        {
            logger.Info($"Starting UserManagementService");

            var configuration = new Configuration("UserManagementConfiguration.xml");
            var authData = new SQLAuthData(configuration.AuthenticationDatabase);
            var userDatabase = new SQLServerUserAccountData(configuration.ApplicationDatabase);

            using (var endpoint = new Endpoint(configuration.ListenURL))
            {
                endpoint.AddHandler(new Handlers.RegisterNewUserHandler(authData, userDatabase));

                endpoint.Run(configuration.MaxConnections);
            }
        }

        private static Logger logger = LogManager.GetCurrentClassLogger();
    }
}
