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
using Microsoft.Practices.Unity;
using InvestmentBuilder;

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
        void RemoveUserSession(string sessionId);
    }

    internal class UserSessionManager : EndpointManager, ISessionManager
    {
        private Dictionary<string, UserSession> _userSessions = new Dictionary<string, UserSession>();
        private IAuthDataLayer _authdata;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private AccountManager _accountManager;

        public UserSessionManager(IConnectionSession session, IAuthDataLayer authtdata, AccountManager accountManager)
            : base(session)
        {
            _authdata = authtdata;
            _accountManager = accountManager;
        }

        //this method handles authentication calls from the middleware server. authenitcate
        //user against the authentication database. password must be stored as encrypted
        protected override void EndpointMessageHandler(Message message)
        {
            if (message.Command == HandlerNames.NOTIFY_CLOSE)
            {
                logger.Log(LogLevel.Info, "session closing: {0}", message.Payload);
                _userSessions.Remove(message.Payload);
                return;
            }

            //todo...
            if (message.Command == HandlerNames.LOGIN)
            {
                Task.Factory.StartNew(() =>
                {
                    var login = JsonConvert.DeserializeObject<LoginPayload>(message.Payload);
                    var salt = _authdata.GetSalt(login.UserName);
                    var hash = SaltedHash.GenerateHash(login.Password, salt);
                    bool authenticated = _authdata.AuthenticateUser(login.UserName, hash);

                    GetSession().SendAuthenticationResult(authenticated, authenticated ? "authentication succeded" : "authenitcation failed", message.RequestId);

                    if(authenticated == true)
                    {
                        var userSession = new UserSession(login.UserName, message.SourceId);
                        var accounts = _accountManager.GetAccountNames(login.UserName).ToList();
                        userSession.AccountName = accounts.FirstOrDefault();
                        _userSessions.Add(message.SourceId, userSession);
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

        public void RemoveUserSession(string sessionId)
        {
            _userSessions.Remove(sessionId);
        }

        public override void RegisterChannels(IUnityContainer container)
        {

        }
    }
}
