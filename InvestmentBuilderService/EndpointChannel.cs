using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiddlewareNetClient;
using Newtonsoft.Json;
using InvestmentBuilderCore;
using InvestmentBuilderService.Session;

namespace InvestmentBuilderService
{
    //base class for all Dtos
    internal class Dto
    {
        public string Name { get; private set; }
        public Dto()
        {
            Name = this.GetType().Name;
        }
    }

    //base class for reponse Dtos
    internal class ResponseDto : Dto
    {
        public bool Status { get; set; }
    }

    internal interface IEndpointChannel
    {
        string RequestName { get;}
        string ResponseName { get;}
        void ProcessMessage(IConnectionSession session, UserSession userSession, string payload, string sourceId,  string requestId);
    }

    internal abstract class EndpointChannel<Request, Update> : IEndpointChannel 
        where Request : Dto, new()
        where Update : IChannelUpdater
    {
        public string RequestName { get; private set; }
        public string ResponseName { get; private set; }

        private AccountService _accountService;

        protected EndpointChannel(string requestName, string responseName, AccountService accountService)
        {
            RequestName = requestName;
            ResponseName = responseName;
            _accountService = accountService;
        }

        /// <summary>
        /// abstract method for handling requests on this channel
        /// </summary>
        /// <param name="userSession"></param>
        /// <param name="payload"></param>
        /// <param name="updater"></param>
        /// <returns></returns>
        protected abstract Dto HandleEndpointRequest(UserSession userSession, Request payload, Update updater);

        /// <summary>
        /// factory method for creating an updater to use for this channel. by default does not create one
        /// </summary>
        /// <returns></returns>
        public virtual Update GetUpdater(IConnectionSession session, UserSession userSession, string sourceId)
        {
            return default(Update);
        }

        /// <summary>
        /// common method for converting a string payload into a request dto object
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        public Request ConvertToRequestPayload(string payload)
        {
            return payload != null ? JsonConvert.DeserializeObject<Request>(payload) : new Request();
        }

        /// <summary>
        /// helper method for getting the current user token
        /// </summary>
        /// <param name="session"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        protected UserAccountToken GetCurrentUserToken(UserSession session, string account = null)
        {
            return _accountService.GetUserAccountToken(session, account);
        }

        /// <summary>
        /// entry method for class. called when processing a request on this channel
        /// </summary>
        /// <param name="session"></param>
        /// <param name="userSession"></param>
        /// <param name="payload"></param>
        /// <param name="sourceId"></param>
        /// <param name="requestId"></param>
        public void ProcessMessage(IConnectionSession session,  UserSession userSession,  string payload, string sourceId, string requestId)
        {
            var requestPayload = ConvertToRequestPayload(payload);
            var updater = GetUpdater(session, userSession, sourceId);
            if(updater != null)
            {
                //TODO. this list should be periodically cleaned up
                _updaterList.Add(updater);
            }
            var responsePayload = HandleEndpointRequest(userSession, requestPayload, updater);
            if ((responsePayload != null) && (string.IsNullOrEmpty(ResponseName) == false))
            {
                session.SendMessageToChannel(ResponseName, JsonConvert.SerializeObject(responsePayload), sourceId, requestId);
            }
        }

        protected AccountService GetAccountService()
        {
            return _accountService;
        }

        #region Private Data Members
        private readonly List<IChannelUpdater> _updaterList = new List<IChannelUpdater>();
        #endregion
    }
}