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

            using (var endpoint = new Endpoint(configuration.ListenURL, configuration.HostURL))
            {
                RegisterHandlers(endpoint, configuration);
                endpoint.Run(configuration.MaxConnections);
            }
        }

        /// <summary>
        /// Register user manager handlers
        /// </summary>
        private static void RegisterHandlers(Endpoint endpoint, Configuration configuration)
        {
            var authData = new SQLAuthData(configuration.AuthenticationDatabase);
            var userDatabase = new SQLServerUserAccountData(configuration.ApplicationDatabase);
            var userNotifer = new TestNotifier();
            //var userNotifer = new SmtpNotifier(configuration.SmtpServer,
            //                                   configuration.SmtpUserName,
            //                                   configuration.SmtpPassword,
            //                                   configuration.OurEmailAddress);

            var changePasswordHandler = new Handlers.ChangePasswordHandler(authData);
            var changePasswordUrl = configuration.ChangePasswordURL;

            endpoint.AddHandler(new Handlers.RegisterNewUserHandler(authData, userDatabase));
            endpoint.AddHandler(new Handlers.ForgottonPasswordHandler(userNotifer, changePasswordUrl, authData));
            endpoint.AddHandler(changePasswordHandler);
        }

        private static Logger logger = LogManager.GetCurrentClassLogger();
    }
}
