using System.Collections.Generic;
using Newtonsoft.Json;
using InvestmentBuilderCore;
using InvestmentBuilderService.Session;
using NLog;
using System;

namespace InvestmentBuilderService
{
    //base class for all Dtos
    internal class Dto
    {
        public string Name { get; private set; }
        public bool IsError { get; set; }
        public string Error { get; set; }
        public Dto()
        {
            Name = this.GetType().Name;
        }
    }

    //base class for reponse Dtos
    internal class ResponseDto : Dto
    {
        public bool Status { get; set; }
        public string Error { get; set; }
    }

    /// <summary>
    /// Interface for endpoint channels. An endpoint channel can manage both a request and response channel
    /// for the same topic. 
    /// </summary>
    internal interface IEndpointChannel
    {
        string RequestName { get;}
        string ResponseName { get;}
        void ProcessMessage(IConnectionSession session, UserSession userSession, string payload, string sourceId,  string requestId);
    }

    /// <summary>
    /// Base class for an endpoint channel. These classes are instantiated using the
    /// Unity dependency injection framework. require a channel request name and optionally
    /// a channel response name. Concrete classes define the request and response channel
    /// names
    /// </summary>
    internal abstract class EndpointChannel<Request, Update> : IEndpointChannel 
        where Request : Dto, new()
        where Update : IChannelUpdater
    {
        #region Public Properties

        public string RequestName { get; private set; }
        public string ResponseName { get; private set; }

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        protected EndpointChannel(string requestName, string responseName, AccountService accountService)
        {
            RequestName = requestName;
            ResponseName = responseName;
            _accountService = accountService;
        }

        /// <summary>
        /// abstract method for handling requests on this channel
        /// </summary>
        protected abstract Dto HandleEndpointRequest(UserSession userSession, Request payload, Update updater);

        /// <summary>
        /// Factory method for creating an updater to use for this channel. by default does not create one.
        /// An updater is required for for sending updates on the response channel. By default, only a single response
        /// is sent for a request.
        /// </summary>
        public virtual Update GetUpdater(IConnectionSession session, UserSession userSession, string sourceId, string requestId)
        {
            return default(Update);
        }

        /// <summary>
        /// Common method for converting a string payload into a request dto object.
        /// </summary>
        public Request ConvertToRequestPayload(string payload)
        {
            return payload != null ? JsonConvert.DeserializeObject<Request>(payload) : new Request();
        }

        /// <summary>
        /// Helper method for getting the current user token.
        /// </summary>
        protected UserAccountToken GetCurrentUserToken(UserSession session, AccountIdentifier account = null)
        {
            return _accountService.GetUserAccountToken(session, account);
        }

        /// <summary>
        /// Process an incoming request on this endpoint
        /// </summary>
        public void ProcessMessage(IConnectionSession session,  UserSession userSession,  string payload, string sourceId, string requestId)
        {
            var requestPayload = ConvertToRequestPayload(payload);
            var updater = GetUpdater(session, userSession, sourceId, requestId);
            if(updater != null)
            {
                //TODO. this list should be periodically cleaned up
                _updaterList.Add(updater);
            }
            Dto responsePayload;
            try
            {
                responsePayload = HandleEndpointRequest(userSession, requestPayload, updater);
            }
            catch(Exception ex)
            {
                logger.Error(ex);
                responsePayload = new Dto { IsError = true, Error = "Internal Server Error" };
            } 
            
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
        private readonly AccountService _accountService;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        #endregion
    }
}