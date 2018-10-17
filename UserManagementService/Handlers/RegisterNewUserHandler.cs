using InvestmentBuilderCore;
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
    /// Register new user response dto.
    /// </summary>
    class RegisterNewUserResponse
    {
        public NewUserResponseEnum Response { get; set; }
        public string Message { get; set; }
    }

    /// <summary>
    /// Class for handling a Register NewUser request
    /// </summary>
    class RegisterNewUserHandler : PostRequestHandler<RegisterNewUserRequest, RegisterNewUserResponse>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public RegisterNewUserHandler(IAuthDataLayer authtdata, IUserAccountInterface userAccountData) : base("RegisterUser")
        {
            _authdata = authtdata;
            _userAccountData = userAccountData;
        }

        /// <summary>
        /// Process the RegisterNewUser request
        /// </summary>
        protected override RegisterNewUserResponse ProcessRequest(RegisterNewUserRequest request, Dictionary<string, List<string>> headers)
        {
            var response = new RegisterNewUserResponse();

            if (request.Password != request.ConfirmPassword)
            {
                response.Response = NewUserResponseEnum.PASSWORDS_NOT_MATCHING;
            }
            else if(string.IsNullOrWhiteSpace(request.Password))
            {
                //TODO, validate password
                response.Response = NewUserResponseEnum.INVALID_PASSWORD;
            }
            else
            {
                var salt = SaltedHash.GenerateSalt();
                var result = _authdata.AddNewUser(request.UserName,
                                    request.EMailAddress,
                                    salt,
                                    SaltedHash.GenerateHash(request.Password, salt),
                                    request.PhoneNumber,
                                    true);

                response.Response = toResponse(result);

                if (response.Response == NewUserResponseEnum.SUCCESS)
                {
                    _userAccountData.AddUser(request.EMailAddress, request.UserName);
                }
            }

            return response;
        }

        /// <summary>
        /// convert database response to dto response
        /// </summary>
        /// <returns></returns>
        private NewUserResponseEnum toResponse(int result)
        {
            switch (result)
            {
                case 0:
                    return NewUserResponseEnum.SUCCESS;
                case 1:
                    return NewUserResponseEnum.INVALID_USERNAME;
                case 2:
                    return NewUserResponseEnum.INVALID_PASSWORD;
                case 3:
                    return NewUserResponseEnum.USER_ALREADY_EXISTS;
            }
            return NewUserResponseEnum.INVALID;
        }

        #region Private Member Data

        private readonly IAuthDataLayer _authdata;
        private readonly IUserAccountInterface _userAccountData;

        #endregion
    }
}
