using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiddlewareNetClient;
using Middleware;

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

        public UserSessionManager(string server, string username, string password)
            : base(server, username, password)
        {
        }

        //this method handles authentication calls from the middleware server. authenitcate
        //user against the authentication database. password must be stored as encrypted
        protected override void EndpointMessageHandler(ISession session, Middleware.Message message)
        {
            //todo...
        }

        public UserSession GetUserSession(Middleware.Message message)
        {
            UserSession userSession = null;
            if (_userSessions.TryGetValue(message.SourceId, out userSession) == false)
            {
                GetLogger().LogError(string.Format("unknown user for session: {0} ", message.SourceId));
            }
            return userSession;
        }
    }
}
