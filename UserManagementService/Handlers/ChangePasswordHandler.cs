using InvestmentBuilderCore;
using NLog;
using System;
using System.Collections.Generic;

namespace UserManagementService.Handlers
{
    /// <summary>
    /// ChangePasswordRequest DTO
    /// </summary>
    class ChangePasswordRequest
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    /// <summary>
    /// Class handles a request to change a password. request must contain the new password as well
    /// as the token sent to the users email address so the user can be validated
    /// </summary>
    class ChangePasswordHandler : PostRequestHandler<ChangePasswordRequest, UserManagementResponse>
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public ChangePasswordHandler(IAuthDataLayer authdata) : base("ChangePassword")
        {
            _authData = authdata;
        }

        #endregion

        #region Protected Override Methods

        /// <summary>
        /// Handle request. Validate the user and token. if successful change password
        /// </summary>
        protected override UserManagementResponse ProcessRequest(ChangePasswordRequest request, Dictionary<string, List<string>> headers)
        {
            logger.Info("Processing ChangePassword request.");

            var salt = SaltedHash.GenerateSalt();
            var result = _authData.ChangePassword( 
                                                request.Email,
                                                request.Token,
                                                SaltedHash.GenerateHash(request.Password, salt),
                                                salt);

            var response = new UserManagementResponse();
            if(result == true)
            {
                response.Result = UserManagementResponse.UserManagementResponseType.SUCCESS;
                response.ResultMessage = "Change Password Request Succeded";
            }
            else
            {
                response.Result = UserManagementResponse.UserManagementResponseType.FAIL;
                response.ResultMessage = "Failed to change password. Token may have expired.";
            }

            logger.Info($"ChangePassword Response : {response.Result}");
            return response;
        }

        #endregion

        #region Private Data Members

        private readonly IAuthDataLayer _authData;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        #endregion
    }
}
