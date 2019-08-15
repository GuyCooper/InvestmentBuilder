
using InvestmentBuilderCore;
using NLog;
using System.Collections.Generic;

namespace UserManagementService.Handlers
{
    /// <summary>
    /// Validate new user dto.
    /// </summary>
    class ValidateNewUserRequest
    {
        public string Token { get; set; }
    }

    /// <summary>
    /// Class handles a request to validate a new user. The link to validate a new user is sent
    /// to the users email address. This handler is invoked when that link is clicked. THe link will
    /// include a token that was generated when the new user was registered. User does not become valid
    /// until they have been validated.
    /// </summary>
    class ValidateNewUserHandler : PostRequestHandler<ValidateNewUserRequest, UserManagementResponse>
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public ValidateNewUserHandler(IAuthDataLayer authdata) : base("ValidateNewUser")
        {
            m_authData = authdata;
        }

        #endregion

        #region Protected Overrides

        /// <summary>
        /// Handle request. Validate the new user with the token. User must be validated here before they
        /// are valid.
        /// </summary>
        protected override UserManagementResponse ProcessRequest(ValidateNewUserRequest request, Dictionary<string, List<string>> headers)
        {
            var result = m_authData.ValidateNewUser(request.Token);
            var response = new UserManagementResponse();

            if (result == true)
            {
                response.Result = UserManagementResponse.UserManagementResponseType.SUCCESS;
                response.ResultMessage = "Validate new user Succeded";
            }
            else
            {
                response.Result = UserManagementResponse.UserManagementResponseType.FAIL;
                response.ResultMessage = "Validate new user failed.";
            }

            return response;
        }

        #endregion

        #region Private Data

        private readonly IAuthDataLayer m_authData;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        #endregion

    }
}
