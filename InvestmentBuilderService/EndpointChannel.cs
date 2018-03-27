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

    internal abstract class EndpointChannel<Request> : IEndpointChannel where Request : Dto, new()
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

        public abstract Dto HandleEndpointRequest(UserSession userSession, Request payload);

        public Request ConvertToRequestPayload(string payload)
        {
            return payload != null ? JsonConvert.DeserializeObject<Request>(payload) : new Request();
        }

        protected UserAccountToken GetCurrentUserToken(UserSession session, string account = null)
        {
            return _accountService.GetUserAccountToken(session, account);
        }

        public void ProcessMessage(IConnectionSession session,  UserSession userSession,  string payload, string sourceId, string requestId)
        {
            var requestPayload = ConvertToRequestPayload(payload);
            var responsePayload = HandleEndpointRequest(userSession, requestPayload);
            if ((responsePayload != null) && (string.IsNullOrEmpty(ResponseName) == false))
            {
                session.SendMessageToChannel(ResponseName, JsonConvert.SerializeObject(responsePayload), sourceId, requestId);
            }
        }
    }
}