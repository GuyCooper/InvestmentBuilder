using NLog;
using SQLServerDataLayer;
using System;
using System.Threading;

namespace UserManagementService
{
    /// <summary>
    /// UserManagementService class
    /// </summary>
    class UserManagementService
    {
        #region Main entry point

        /// <summary>
        /// Main entry point
        /// </summary>
        public static void Main(string[] args)
        {
            logger.Info($"Starting UserManagementService");

            var configuration = new Configuration("UserManagementConfiguration.xml");

            //Instantiate the listener...
            using (var endpoint = new Endpoint(configuration.ListenURL, configuration.HostURL))
            {
                //Register all the handlers.
                RegisterHandlers(endpoint, configuration);
                endpoint.Run();

                //Wait until user shuts down the application...
                Console.CancelKeyPress += (sender, e) =>
                {
                    e.Cancel = true;
                    _shutdownEvent.Set();
                };

                _shutdownEvent.WaitOne();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Register user manager handlers
        /// </summary>
        private static void RegisterHandlers(Endpoint endpoint, Configuration configuration)
        {
            var authData = new SQLAuthData(configuration.AuthenticationDatabase);
            var userDatabase = new SQLServerUserAccountData(configuration.ApplicationDatabase);
            var userNotifer = new TestNotifier();
            //var smptNotifer = new SmtpNotifier(configuration.SmtpServer,
            //                                   configuration.OurEmailAddress,
            //                                   configuration.SmtpUserName,
            //                                   configuration.SmtpPassword
            //                                   );
        
            var changePasswordHandler = new Handlers.ChangePasswordHandler(authData);
            var changePasswordUrl = $"{configuration.HostURL}/{configuration.ChangePasswordPage}";
            var validateNewUserUrl= $"{configuration.HostURL}/{configuration.ValidateNewUserPage}";

            endpoint.AddHandler(new Handlers.RegisterNewUserHandler(authData, userDatabase, userNotifer, validateNewUserUrl));
            endpoint.AddHandler(new Handlers.ForgottonPasswordHandler(userNotifer, changePasswordUrl, authData));
            endpoint.AddHandler(changePasswordHandler);
            endpoint.AddHandler(new Handlers.ValidateNewUserHandler(authData));
        }

        #endregion

        #region Private Data

        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static AutoResetEvent _shutdownEvent = new AutoResetEvent(false);

        #endregion

    }
}
