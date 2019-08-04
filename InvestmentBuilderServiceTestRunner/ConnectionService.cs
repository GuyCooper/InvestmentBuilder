using Middleware;
using MiddlewareNetClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using NLog;

namespace InvestmentBuilderServiceTestRunner
{
    /// <summary>
    /// Class handles middleware connection.
    /// </summary>
    class ConnectionService
    {
        #region Delegates

        public delegate void MessageHandler(string payload);

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public ConnectionService()
        {
            m_middleware = new MiddlewareManager();
            m_middleware.RegisterMessageCallbackFunction(EndpointMessageHandler);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Connect to Middleware session
        /// </summary>
        public async Task<bool> Connect(string url, string username, string password)
        {
            m_session = await m_middleware.CreateSession(url, username, password, s_appName);
            m_middleware.RegisterMessageCallbackFunction(EndpointMessageHandler);

            if(m_session == null)
            {
                logger.Error($"Connection failed {url}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Send a request to the specified channel. Asynchronous call.
        /// </summary>
        public async Task<bool> SendRequest(string channel, string payload, MessageHandler handler)
        {
            if (m_session != null)
            {
                var ack = await m_middleware.SendRequest(m_session, channel, payload);
                m_pendingRequests.Add(ack.RequestId, handler);

                if(!ack.Success)
                {
                    logger.Error($"SendRequest to channel {channel} failed. error: {ack.Payload}");
                }

                return ack.Success;
            }

            return false;
        }

        /// <summary>
        /// Subscribe to a channel.
        /// </summary>
        public async Task<bool> SubscribeToChannel(string channel)
        {
            var response = await m_middleware.SubscribeToChannel(m_session, channel);
            return response.Success;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Handle message from Middleware session
        /// </summary>
        private void EndpointMessageHandler(ISession session, Message message)
        {
            if(message != null)
            {
                MessageHandler handler;
                if(m_pendingRequests.TryGetValue(message.RequestId, out handler))
                {
                    handler(message.Payload);
                }
                else
                {
                    logger.Error($"EndpointMessageHandler. request id not found. {message.RequestId}");
                }
                m_pendingRequests.Remove(message.RequestId);
            }
        }

        #endregion

        #region Private Data

        private readonly MiddlewareManager m_middleware;

        private ISession m_session;

        private static readonly string s_appName = "InvestmentBuilderServiceTestApp";

        private readonly Dictionary<string, MessageHandler> m_pendingRequests = new Dictionary<string, MessageHandler>();

        private static Logger logger = LogManager.GetCurrentClassLogger();

        #endregion

    }
}
