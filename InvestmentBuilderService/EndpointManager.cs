using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiddlewareNetClient;
using Middleware;
using Newtonsoft.Json;
using InvestmentBuilderService.Channels;
using InvestmentBuilderCore;

namespace InvestmentBuilderService
{
    internal class EndpointManager
    {
        private MiddlewareManager _middleware;
        private ISession _session;
        private ILogger _logger;
        private string _server;
        private string _username;
        private string _password;

        private Dictionary<string, EndpointChannel<Dto>> _channels;
        private Dictionary<string, UserSession> _userSessions;

        public EndpointManager(string server, string username, string password)
        {
            _logger = new ServiceLogger();
            _middleware = new MiddlewareManager();
            _server = server;
            _username = username;
            _password = password;

            _channels = new Dictionary<string, EndpointChannel<Dto>>();
            _userSessions = new Dictionary<string, UserSession>();
        }

        public async Task<bool> Connect()
        {
            _session = await _middleware.CreateSession(_server, _username, _password, _logger);
            _middleware.RegisterMessageCallbackFunction(EndpointMessageHandler);
            return _session != null;
        }

        public void RegisterEndpointChannels()
        {
            ContainerManager.RegisterType(typeof(UpdateAccountEndpointChannel), typeof(UpdateAccountEndpointChannel), true);
        }
        private void EndpointMessageHandler(ISession session, Middleware.Message message)
        {
            if(message.Type != MessageType.REQUEST)
            {
                _logger.LogError(string.Format("invalid message type: {0}", message.Type));
                return;
            }

            UserSession userSession;
            if(_userSessions.TryGetValue(message.SourceId, out userSession) == false)
            {
                _logger.LogError(string.Format("unknown user for session: {0} ", message.SourceId));
                return;
            }

            EndpointChannel<Dto> channel;
            if(_channels.TryGetValue(message.Channel, out channel) == true)
            {
                Task.Factory.StartNew(() =>
               {
                   var requestPayload = channel.ConvertToRequestPayload(message.Payload);
                   var responsePayload = channel.HandleEndpointRequest(userSession, requestPayload);
                   if ((responsePayload != null) && (string.IsNullOrEmpty(channel.ResponseName) == false))
                   {
                       _middleware.SendMessageToChannel(session, channel.ResponseName, JsonConvert.SerializeObject(responsePayload), message.SourceId);
                   }
               });
            }
            else
            {
                _logger.LogError(string.Format("invalid channel : {0}", message.Channel));
            }
        }
    }

    internal class ServiceLogger : ILogger
    {
        public void LogError(string error)
        {
            throw new NotImplementedException();
        }

        public void LogMessage(string message)
        {
            throw new NotImplementedException();
        }
    }
}
