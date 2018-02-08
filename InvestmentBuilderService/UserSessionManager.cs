using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiddlewareNetClient;
using Middleware;
using Newtonsoft.Json;
using InvestmentBuilderCore;
using NLog;
using InvestmentBuilderService.Session;
using InvestmentBuilderService.Utils;

namespace InvestmentBuilderService
{
    /// <summary>
    /// this endpoint manager class handles all the authentication requests from the 
    /// middleware server. it creates a usersession if the authentiction is 
    /// successful 
    /// </summary>
    internal interface ISessionManager
    {
        UserSession GetUserSession(Middleware.Message message);
    }

    internal class UserSessionManager : EndpointManager, ISessionManager
    {
        private Dictionary<string, UserSession> _userSessions = new Dictionary<string, UserSession>();
        private IAuthDataLayer _authtdata;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public UserSessionManager(IConnectionSession session, IAuthDataLayer authtdata)
            : base(session)
        {
            _authtdata = authtdata;
        }

        //this method handles authentication calls from the middleware server. authenitcate
        //user against the authentication database. password must be stored as encrypted
        protected override void EndpointMessageHandler(Message message)
        {
            //todo...
            if (message.Command == HandlerNames.LOGIN)
            {
                Task.Factory.StartNew(() =>
                {
                    var login = JsonConvert.DeserializeObject<LoginPayload>(message.Payload);
                    var salt = _authtdata.GetSalt(login.UserName);
                    var hash = SaltedHash.GenerateHash(login.Password, salt);
                    bool authenticated = _authtdata.AuthenticateUser(login.UserName, hash);

                    GetSession().SendAuthenticationResult(authenticated, authenticated ? "authentication succeded" : "authenitcation failed", message.RequestId);

                    if(authenticated == true)
                    {
                        _userSessions.Add(message.SourceId, new UserSession(login.UserName, message.SourceId));
                    }
                });
            }
        }

        public UserSession GetUserSession(Middleware.Message message)
        {
            UserSession userSession = null;
            if (_userSessions.TryGetValue(message.SourceId, out userSession) == false)
            {
                logger.Log(LogLevel.Error, "unknown user for session: {0} ", message.SourceId);
            }
            return userSession;
        }
    }
}
