using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiddlewareNetClient;
using Middleware;
using Newtonsoft.Json;
using InvestmentBuilderService.Channels;

namespace InvestmentBuilderService
{
    internal abstract class EndpointManager
    {
        private MiddlewareManager _middleware;
        private ISession _session;
        private ILogger _logger;
        private string _server;
        private string _username;
        private string _password;

        public EndpointManager(string server, string username, string password)
        {
            _logger = new ServiceLogger();
            _middleware = new MiddlewareManager();
            _server = server;
            _username = username;
            _password = password;
        }

        public async Task<bool> Connect()
        {
            _session = await _middleware.CreateSession(_server, _username, _password, _logger);
            _middleware.RegisterMessageCallbackFunction(EndpointMessageHandler);
            return _session != null;
        }

        protected abstract void EndpointMessageHandler(ISession session, Middleware.Message message);

        protected void SendMessageToClient(string channelName, string payload, string destination)
        {
            _middleware.SendMessageToChannel(_session, channelName, payload, destination);
        }

        protected ILogger GetLogger()
        {
            return _logger;
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
