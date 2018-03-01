using System;
using System.Threading.Tasks;
using MiddlewareNetClient;
using Middleware;
using InvestmentBuilderService.Utils;
using NLog;

namespace InvestmentBuilderService.Session
{
    class MiddlewareSession : IConnectionSession
    {
        private readonly MiddlewareManager _middleware;
        private ISession _session;
        private MiddlewareNetClient.ILogger _logger;
        private SessionMessageHandler _callbackHandler;
        private IConnection _settings;

        public MiddlewareSession(IConnection settings)
        {
            _middleware = new MiddlewareManager();
            _logger = new ServiceLogger();
            _settings = settings;
        }

        public void RegisterMessageHandler(SessionMessageHandler handler)
        {
            _callbackHandler = handler;
        }

        public async Task<bool> Connect()
        {
            _session = await _middleware.CreateSession(_settings.ServerName, _settings.Username, _settings.Password, _logger);
            _middleware.RegisterMessageCallbackFunction(EndpointMessageHandler);
            return _session != null;
        }

        private void EndpointMessageHandler(ISession session, Middleware.Message message)
        {
            if(_callbackHandler != null && message != null)
            {
                _callbackHandler(message);
            }
        }

        public async Task<bool> RegisterAuthenticationServer(string identifier)
        {
            if (_session != null)
            {
                var response = await _middleware.RegisterAuthHandler(_session, identifier);
                return response.Success;
            }
            return false;
        }

        public void SendAuthenticationResult(bool result, string message, string requestid)
        {
            if (_session != null)
            {
                var authResult = new AuthResult
                {
                    Message = message,
                    Success = result
                };

                _middleware.SendAuthenticationResponse(_session, requestid, authResult);
            }
        }

        public void SendMessageToChannel(string channel, string payload, string destination)
        {
            if (_session != null)
            {
                _middleware.SendMessageToChannel(_session, channel, payload, destination);
            }
        }

        public async void RegisterChannelListener(string channel)
        {
            var response = await _middleware.AddChannelListener(_session, channel);
            if(response.Success == false)
            {
                //TODO add functionality to peridoically attempt the register request
                //if it fails. For now we will just log the error
                if(_logger != null)
                {
                    _logger.LogError(string.Format("unable to register listener for channel {0}. {1}", channel, response.Payload));
                }
            }
        }
    }

    internal class ServiceLogger : MiddlewareNetClient.ILogger
    {
        private static Logger logger = LogManager.GetLogger("Middelware connection");

        public void LogError(string error)
        {
            logger.Log(LogLevel.Error, error);
        }

        public void LogMessage(string message)
        {
            logger.Log(LogLevel.Info, message);
        }
    }

}
