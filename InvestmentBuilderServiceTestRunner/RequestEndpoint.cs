using MiddlewareInterfaces;
using NLog;
using System.Threading;

namespace InvestmentBuilderServiceTestRunner
{
    /// <summary>
    /// Base class for all Dtos.
    /// </summary>
    internal class Dto
    {
        public string Name { get; private set; }
        public bool IsError { get; set; }
        public string Error { get; set; }
    }

    /// <summary>
    /// Interface to define an endpoint
    /// </summary>
    internal interface IRequestEndpoint
    {
        /// <summary>
        /// Request Channel.
        /// </summary>
        string RequestChannel { get; }

        /// <summary>
        /// Response Channel.
        /// </summary>
        string ResponseChannel { get; }
    }

    /// <summary>
    /// Defines an endpoint for a channel.
    /// </summary>
    internal abstract class RequestEndpoint<Request, Result> : IRequestEndpoint
        where Request : Dto, new()
        where Result : Dto, new()
    {
        #region Nested classes

        /// <summary>
        /// TestResult class, wraps the result of the request as well as information about the
        /// success of the request and any error message.
        /// </summary>
        public class TestResult<T>
        {
            public T Result { get; set; }
            public bool Success { get; set; }
            public string Error { get; set; }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Request Channel.
        /// </summary>
        public string RequestChannel { get; private set; }

        /// <summary>
        /// Response Channel.
        /// </summary>
        public string ResponseChannel { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        public RequestEndpoint(string requestChannel, string resposeChannel)
        {
            RequestChannel = requestChannel;
            ResponseChannel = resposeChannel;
        }

        /// <summary>
        /// Method sends a request to the server. Does not return until either it receives a response
        /// or the timeout is reached.
        /// </summary>
        public TestResult<Result> SendRequest(Request request, ConnectionService connectionService)
        {
            m_payload = null;
            var payload = MiddlewareUtils.SerialiseObjectToString(request);
#pragma warning disable 4014
            connectionService.SendRequest(RequestChannel, payload, ProcessResponse);
#pragma warning restore 4014
            //if(!success)
            //{
            //    // Failed to send the request.
            //    return null;
            //}

            // Send the request ok. now just wait until we get a response
            if (m_ReceiveEvent.WaitOne(m_timeoutMS) == false)
            {
                // We have timed out waiting for the response. clear the request and return an error
                return new TestResult<Result>
                {
                    Success = false,
                    Error = "Timed out waiting for response"
                };
            }

            return new TestResult<Result>
            {
                Result = MiddlewareUtils.DeserialiseObject<Result>(m_payload),
                Success = true
            };
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Log a message
        /// </summary>
        protected void LogMessage(string message)
        {
            logger.Info(message);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Method handles the response for the request.
        /// </summary>
        private void ProcessResponse(string payload)
        {
            m_payload = payload;
            m_ReceiveEvent.Set();
        }

        #endregion

        #region Private Data

        private readonly ManualResetEvent m_ReceiveEvent = new ManualResetEvent(false);

        private readonly int m_timeoutMS = 30000;
        private string m_payload;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        #endregion
    }
}
