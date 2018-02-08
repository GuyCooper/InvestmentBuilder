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
using InvestmentBuilderService.Utils;
using InvestmentBuilderService.Session;
using NLog;

namespace InvestmentBuilderService
{
    class ChannelEndpointManager : EndpointManager
    {
        private Dictionary<string, EndpointChannel<Dto>> _channels;
        private ISessionManager _sessionManager;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public ChannelEndpointManager(IConnectionSession session, ISessionManager sessionManager) 
            : base(session)
        {
            _channels = new Dictionary<string, EndpointChannel<Dto>>();
            _sessionManager = sessionManager;
        }

        public void RegisterEndpointChannels()
        {
            ContainerManager.RegisterType(typeof(UpdateAccountEndpointChannel), typeof(UpdateAccountEndpointChannel), true);
            //and the rest....
        }

        protected override void EndpointMessageHandler(Message message)
        {
            if (message.Type != MessageType.REQUEST)
            {
                //GetLogger().LogError(string.Format("invalid message type: {0}", message.Type));
                logger.Log(LogLevel.Error, "invalid message type");
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
                        GetSession().SendMessageToChannel(channel.ResponseName, JsonConvert.SerializeObject(responsePayload), message.SourceId);
                    }
                });
            }
            else
            {
                logger.Log(LogLevel.Error, "invalid channel : {0}", message.Channel);
            }
        }
    }
}
