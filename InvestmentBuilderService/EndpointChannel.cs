using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiddlewareNetClient;
using Newtonsoft.Json;
using InvestmentBuilderCore;

namespace InvestmentBuilderService
{
    //base class for all Dtos
    internal class Dto
    {
        public string Name { get; set; }
    }

    //base class for reponse Dtos
    internal class ResponseDto : Dto
    {
        public bool Status { get; set; }
    }

    internal abstract class EndpointChannel<Request> where Request : Dto
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
            return JsonConvert.DeserializeObject<Request>(payload);
        }

        protected UserAccountToken GetCurrentUserToken(UserSession session, string account = null)
        {
            return _accountService.GetUserAccountToken(session, account);
        }
    }
}