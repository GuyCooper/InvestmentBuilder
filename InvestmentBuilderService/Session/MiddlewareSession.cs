using System;
using System.Threading.Tasks;
using MiddlewareNetClient;
using Middleware;
using InvestmentBuilderService.Utils;
using NLog;

namespace InvestmentBuilderService.Session
{
    /// <summary>
    /// Middleware Session class. session to Middleware server 
    /// </summary>
    class MiddlewareSession : IConnectionSession, IDisposable
    {
        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        public MiddlewareSession(IConnection settings, string appName)
        {
            _middleware = new MiddlewareManager();
            _settings = settings;
            _appName = appName;
        }

        /// <summary>
        /// Register callback handler for session
        /// </summary>
        public void RegisterMessageHandler(SessionMessageHandler handler)
        {
            _callbackHandler = handler;
        }

        /// <summary>
        /// Connect to Middleware session
        /// </summary>
        public async Task<bool> Connect()
        {
            logger.Log(LogLevel.Debug, $"Connecting {_appName} to Middleware server..");
            _session = await _middleware.CreateSession(_settings.ServerName, _settings.Username, _settings.Password, _appName);
            _middleware.RegisterMessageCallbackFunction(EndpointMessageHandler);
            return _session != null;
        }

        /// <summary>
        /// Register this session as an authentication handler
        /// </summary>
        public async Task<bool> RegisterAuthenticationServer(string identifier)
        {
            if (_session != null)
            {
                logger.Log(LogLevel.Debug, $"Registering {_appName} as an authentication server");

                var response = await _middleware.RegisterAuthHandler(_session, identifier);
                return response.Success;
            }
            return false;
        }

        /// <summary>
        /// Send authentication result 
        /// </summary>
        public void SendAuthenticationResult(bool result, string message, string requestid)
        {
            logger.Log(LogLevel.Trace, $"Authentication result: {result}. {message}");
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

        /// <summary>
        /// Send Message to Channel
        /// </summary>
        public void SendMessageToChannel(string channel, string payload, string destination, string requestId)
        {
            if (_session != null)
            {
                _middleware.SendMessageToChannel(_session, channel, payload, destination, requestId);
            }
        }

        /// <summary>
        /// Register Channel Listener
        /// </summary>
        public async void RegisterChannelListener(string channel)
        {
            logger.Log(LogLevel.Info, $"Registering as channel listener for channel {channel}");
            var response = await _middleware.AddChannelListener(_session, channel);
            if (response.Success == false)
            {
                //TODO add functionality to peridoically attempt the register request
                //if it fails. For now we will just log the error
                logger.Log(LogLevel.Error, $"Unable to register listener for channel {channel}. {response.Payload}");
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Handle message from Middleware session
        /// </summary>
        private void EndpointMessageHandler(ISession session, Middleware.Message message)
        {
            if(_callbackHandler != null && message != null)
            {
                _callbackHandler(message);
            }
        }

        #endregion

        #region IDisposable
        /// <summary>
        /// Clean up the session object
        /// </summary>
        public void Dispose()
        {
            if(IsDisposed == true)
            {
                return;
            }

            var dispose = _session as IDisposable;
            if(dispose != null)
            {
                dispose.Dispose();
            }
            IsDisposed = true;
        }

        private bool IsDisposed;

        #endregion

        #region Private Data Members
        private readonly MiddlewareManager _middleware;
        private ISession _session;
        private SessionMessageHandler _callbackHandler;
        private IConnection _settings;

        //logger instance
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private readonly string _appName;

        #endregion
    }
}
