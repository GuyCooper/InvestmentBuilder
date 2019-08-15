using InvestmentBuilderCore;
using NLog;
using System;
using System.Collections.Generic;

namespace UserManagementService.Handlers
{
    /// <summary>
    /// RegisterNewUser request dto.
    /// </summary>
    class RegisterNewUserRequest
    {
        public string UserName { get; set; }
        public string EMailAddress { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string PhoneNumber { get; set; }
    }

    enum NewUserResponseEnum
    {
        INVALID = 0,
        SUCCESS,
        INVALID_USERNAME,
        INVALID_PASSWORD,
        PASSWORDS_NOT_MATCHING,
        USER_ALREADY_EXISTS
    }

    /// <summary>
    /// Class for handling a Register NewUser request
    /// </summary>
    class RegisterNewUserHandler : PostRequestHandler<RegisterNewUserRequest, UserManagementResponse>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public RegisterNewUserHandler(IAuthDataLayer authtdata, IUserNotifier notifier, string validateNewUserUrl) : base("RegisterUser")
        {
            m_authdata = authtdata;
            m_userNotifier = notifier;
            m_validateNewUserUrl = validateNewUserUrl;
        }

        /// <summary>
        /// Process the RegisterNewUser request
        /// </summary>
        protected override UserManagementResponse ProcessRequest(RegisterNewUserRequest request, Dictionary<string, List<string>> headers)
        {
            logger.Info("processing new user request");

            var response = new UserManagementResponse();

            if (request.Password != request.ConfirmPassword)
            {
                response.Result = UserManagementResponse.UserManagementResponseType.FAIL;
            }
            else if(string.IsNullOrWhiteSpace(request.Password))
            {
                //TODO, validate password
                response.Result = UserManagementResponse.UserManagementResponseType.FAIL;
            }
            else
            {
                var token = Guid.NewGuid().ToString();

                var salt = SaltedHash.GenerateSalt();
                var result = m_authdata.AddNewUser(request.UserName,
                                    request.EMailAddress,
                                    salt,
                                    SaltedHash.GenerateHash(request.Password, salt),
                                    request.PhoneNumber,
                                    true,
                                    token);

                response.Result = toResponse(result);

                if (response.Result == UserManagementResponse.UserManagementResponseType.SUCCESS)
                {
                    var link = $"{m_validateNewUserUrl}?token={token}";
                    m_userNotifier.NotifyUser(request.EMailAddress, link, "ValidateNewUser.html", "Validate New User");

                    response.ResultMessage = "Register New User Succeded.\n\nYou will shortly receive an email to validate the request.";
                }
                else
                {
                    response.ResultMessage = "Register New User Failed.";
                }
            }

            logger.Info($"New User Request finished with response {response.Result}");
            return response;
        }

        /// <summary>
        /// convert database response to dto res = ponse
        /// </summary>
        /// <returns></returns>
        private UserManagementResponse.UserManagementResponseType toResponse(int result)
        {
            if(result == 0)
            {
                return UserManagementResponse.UserManagementResponseType.SUCCESS;
            }
            return UserManagementResponse.UserManagementResponseType.SUCCESS;
        }

        #region Private Member Data

        private readonly IAuthDataLayer m_authdata;

        private static Logger logger = LogManager.GetCurrentClassLogger();
        private readonly IUserNotifier m_userNotifier;
        private readonly string m_validateNewUserUrl;

        #endregion
    }
}
