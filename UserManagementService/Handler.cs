using System.IO;
using System.Collections.Generic;

namespace UserManagementService
{
    /// <summary>
    /// IHandler interface
    /// </summary>
    internal interface IHandler
    {
        /// <summary>
        /// Name of handler
        /// </summary>
        string Name { get;}

        /// <summary>
        /// Entry point for handler.
        /// </summary>
        void HandleRequest(Stream requestStream, string requestString, Stream responseStream, Dictionary<string, List<string>> headers);
    }

    /// <summary>
    /// Abstract Handler class.
    /// </summary>
    internal abstract class Handler<Request, Response> : IHandler
    {
        #region Public Properties

        public string Name { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        public Handler(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Handle request from endpoint
        /// </summary>
        public void HandleRequest(Stream requestStream, string requestString, Stream responseStream, Dictionary<string, List<string>> headers)
        {
            Request request = DeserialiseRequest(requestStream, requestString);
            var response  = ProcessRequest(request, headers);
            Serialiser.SerialiseObject(response, responseStream);
        }

        #endregion

        #region Protected Abstract Methods

        /// <summary>
        /// Handler specific method for processing request. implemented by concrete handler
        /// </summary>
        protected abstract Response ProcessRequest(Request request, Dictionary<string, List<string>> headers);

        /// <summary>
        /// Deserialise the request. THis depends on either a POST or GET request
        /// </summary>
        protected abstract Request DeserialiseRequest(Stream requestStream, string requestString);
         
        #endregion
    }

    /// <summary>
    /// POST request handler. Request is generated from the request stream.
    /// </summary>
    internal abstract class PostRequestHandler<Request, Response> : Handler<Request, Response>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public PostRequestHandler(string name) : base(name) { }

        /// <summary>
        /// Deserialise the request stream
        /// </summary>
        protected override Request DeserialiseRequest(Stream requestStream, string requestString)
        {
            return Serialiser.DeserialiseObject<Request>(requestStream);
        }
    }

    /// <summary>
    /// GET request handler. Request is generated from the headers
    /// </summary>
    internal abstract class GetRequestHandler<Request, Response> : Handler<Request, Response>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public GetRequestHandler(string name) : base(name) { }

        /// <summary>
        /// Deserialise the request headers
        /// </summary>
        protected override Request DeserialiseRequest(Stream requestStream, string requestString)
        {
            return Serialiser.DeserialiseObject<Request>(requestString);
        }
    }

    /// <summary>
    /// Standard user management response DTO. Should only contain pass / fail and brief
    /// resultmessage.
    /// </summary>
    internal class UserManagementResponse
    {
        public enum UserManagementResponseType
        {
            FAIL,
            SUCCESS
        };

        public UserManagementResponseType Result { get; set; }
        public string ResultMessage { get; set; }
    }

}
