using System;
using System.Collections.Generic;
using InvestmentBuilderCore;
using NLog;

namespace UserManagementService.Handlers
{
    /// <summary>
    /// ForgottonPassword Request Dto
    /// </summary>
    class ForgottonPasswordRequest
    {
        public string EMailAddress { get; set; }
    }

    /// <summary>
    /// Class for handling a forgotton password request. Validates the email address and sends
    /// a new password to this address.
    /// </summary>
    class ForgottonPasswordHandler : GetRequestHandler<ForgottonPasswordRequest, UserManagementResponse>
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        public ForgottonPasswordHandler(IUserNotifier notifier, string changePasswordUrl, IAuthDataLayer authdata)
             :  base("ForgottonPassword")
        {
            _changePasswordUrl = changePasswordUrl;
            _authdata = authdata;
            _notifier = notifier;
        }

        /// <summary>
        /// Handle request. Generate a temporary password and set it in the auth table. Send email to user
        /// with temporary password.
        /// </summary>
        protected override UserManagementResponse ProcessRequest(ForgottonPasswordRequest request, Dictionary<string, List<string>> headers)
        {
            var response = new UserManagementResponse();

            logger.Info("Processing Forgoten Password request");

            var token = Guid.NewGuid().ToString();
            var ok = _authdata.PasswordChangeRequest(request.EMailAddress, token);

            if (ok == true)
            {
                var link = $"{_changePasswordUrl}?token={token}";

                _notifier.NotifyUser(request.EMailAddress, link, "PasswordReset.html", "Investment Builder Password Reset");

                response.Result = UserManagementResponse.UserManagementResponseType.SUCCESS;
                response.ResultMessage = "Forgotten Password Request succeded. You will receive an email shortly.";
            }
            else
            {
                response.Result = UserManagementResponse.UserManagementResponseType.FAIL;
                response.ResultMessage = "Failed to Validate Email Address";
            }

            logger.Info($"Forgotten Password Response {response.Result}");
            return response;
        }

        #endregion

        #region Private Data Members

        private readonly IAuthDataLayer _authdata;
        private readonly string _changePasswordUrl;
        private readonly IUserNotifier _notifier;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        #endregion

    }
}
