using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiddlewareNetClient;
using Middleware;
using Newtonsoft.Json;
using InvestmentBuilderCore;
using InvestmentBuilderService.Channels;

namespace InvestmentBuilderService
{
    class ChannelEndpointManager : EndpointManager
    {
        private Dictionary<string, EndpointChannel<Dto>> _channels;
        private ISessionManager _sessionManager;

        public ChannelEndpointManager(string server, string username, string password, ISessionManager sessionManager) 
            : base(server, username, password)
        {
            _channels = new Dictionary<string, EndpointChannel<Dto>>();
            _sessionManager = sessionManager;
        }

        public void RegisterEndpointChannels()
        {
            ContainerManager.RegisterType(typeof(UpdateAccountEndpointChannel), typeof(UpdateAccountEndpointChannel), true);
            //and the rest....
        }

        protected override void EndpointMessageHandler(ISession session, Middleware.Message message)
        {
            if (message.Type != MessageType.REQUEST)
            {
                GetLogger().LogError(string.Format("invalid message type: {0}", message.Type));
                return;
            }

            var userSession = _sessionManager.GetUserSession(message);
            if(userSession == null)
            {
                return;
            }

            EndpointChannel<Dto> channel;
            if (_channels.TryGetValue(message.Channel, out channel) == true)
            {
                Task.Factory.StartNew(() =>
                {
                    var requestPayload = channel.ConvertToRequestPayload(message.Payload);
                    var responsePayload = channel.HandleEndpointRequest(userSession, requestPayload);
                    if ((responsePayload != null) && (string.IsNullOrEmpty(channel.ResponseName) == false))
                    {
                        SendMessageToClient(channel.ResponseName, JsonConvert.SerializeObject(responsePayload), message.SourceId);
                    }
                });
            }
            else
            {
                GetLogger().LogError(string.Format("invalid channel : {0}", message.Channel));
            }
        }
    }
}
