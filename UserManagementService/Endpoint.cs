using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using NLog;
using System.IO;
using System.Text;

namespace UserManagementService
{
    /// <summary>
    /// Class represents a listener endpoint. All communications to the service will come through this class.
    /// </summary>
    class Endpoint : IDisposable
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        public Endpoint(string url)
        {
            logger.Info($"endpoint listener binding to url {url}");
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add(url);
        }

        /// <summary>
        /// Add a handler to the endpoint
        /// </summary>
        /// <param name="handler"></param>
        public void AddHandler(IHandler handler)
        {
            _handlers.Add(handler.Name, handler);
        }

        /// <summary>
        /// Starts the http listener.
        /// </summary>
        public void Run(int maxConnections)
        {
            logger.Info("Endpoint listener starting...");

            _httpListener.Start();

            var sem = new Semaphore(maxConnections, maxConnections);
            while (true)
            {
                sem.WaitOne();
                //_httpListener.BeginGetContext(_ProcessConnection, null);
                //var context = await _httpListener.GetContextAsync();
                _httpListener.GetContextAsync().ContinueWith(async (t) =>
                {
                    var context = await t;
                    _ProcessConnection(context);
                    sem.Release();
                });
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Method processes a connection to this endpoint
        /// </summary>
        private void _ProcessConnection(HttpListenerContext context)
        //private void _ProcessConnection(IAsyncResult result)
        {
            //var context = _httpListener.EndGetContext(result);
            var response = context.Response;
            string error = null;
            List<string> origins = null;
            try
            {
                var request = context.Request;
                logger.Info($"endpoint processing connection from {request.RawUrl}");
                var headers = new Dictionary<string, List<string>>();
                foreach (var name in request.Headers.AllKeys)
                {
                    var value = request.Headers.GetValues(name);
                    headers.Add(name, value.ToList());
                }

                headers.TryGetValue("Origin", out origins);

                if(request.HttpMethod == "OPTIONS")
                {
                    //looks like a CORS ORIGIN request. inform client that we allow
                    //CORS requests.
                    //response.Headers.Add($"Access-Control-Allow-Origin:{origins[0]}");
                    response.Headers.Add($"Access-Control-Allow-Origin:*");
                    response.Headers.Add("Access-Control-Allow-Method: POST, GET, OPTIONS");
                    response.Headers.Add("Access-Control-Max-Age: 86400");
                    response.Headers.Add("Access-Control-Allow-Headers: Content-Type");
                    response.Close();
                    return;
                }

                if (origins != null && origins.Count > 0)
                {
                    response.Headers.Add($"Access-Control-Allow-Origin:{origins[0]}");
                }

                var query = Path.GetFileName(request.Url.AbsolutePath);
                IHandler handler;
                if(_handlers.TryGetValue(query, out handler) == true)
                {
                    response.StatusCode = 200;
                    response.ContentType = "application/json";
                    handler.HandleRequest(request.InputStream, _ValidateQuery(request.Url.Query), response.OutputStream, headers);
                }
                else
                {
                    response.StatusCode = 404;
                    error = "Unknown Request";
                }
            }
            catch(Exception e)
            {
                logger.Error(e.Message);
                error = e.Message;
                response.StatusCode = 500;
            }

            if(string.IsNullOrEmpty(error) == false)
            {
                Serialiser.SerialiseObject(error, response.OutputStream);
            }

            response.OutputStream.Close();
            response.Close();

            //response.OutputStream.Close();
            //response.Headers.Add("Access-Control-Allow-Origin:*");
            //response.Headers.Add("Content-Type", "application/json");
        }

        /// <summary>
        /// Dispose object.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed) return;

            try
            {
                _httpListener.Close();
            }
            catch(Exception)
            {
            }
        }
        protected bool IsDisposed { get; set; }

        #endregion

        #region Private Methods

        /// <summary>
        /// Method takes the http query string and validates and normalises it. 
        /// Converts from format:
        /// 
        /// name1=value1;name2=value2;
        /// 
        /// into a json object:
        /// { "name1" : "value1", "name2" : "value2"}
        /// 
        /// it also checks there is no funny business going on (checking for embedded < or > in query</or>
        /// </summary>
        private string _ValidateQuery(string query)
        {
            var startIndex = query.IndexOf('?');
            if (startIndex > -1)
            {
                query = query.Substring(query.IndexOf('?')+1);
            }
            else
            {
                return query;
            }

            List<char> badChars = new List<char>
            {
                '>',
                '<'
            };

            char badOne = query.FirstOrDefault(ch => badChars.Contains(ch));
            if(badOne != default(char))
            {
                throw new ArgumentException($"Unsafe characters in request. cannot process for security reasons.");
            }

            var parts = query.Split(';');
            var jsonStringBuilder = new StringBuilder("{");
            for(var index = 0; index < parts.Length; index++)
            {
                var element = parts[index].Split('=');
                if(element.Length != 2)
                {
                    throw new ArgumentException("Badly formed query string!");
                }

                jsonStringBuilder.Append($"\"{element[0]}\" : ");
                double dVal;
                if(double.TryParse(element[1], out dVal) == true)
                {
                    jsonStringBuilder.Append($"{element[1]}");
                }
                else
                {
                    jsonStringBuilder.Append($"\"{element[1]}\"");
                }

                if(index < (parts.Length - 1))
                {
                    jsonStringBuilder.Append(",");
                }
            }
            jsonStringBuilder.Append("}");

            return jsonStringBuilder.ToString();
        }

        #endregion

        #region Private Data Members

        private readonly HttpListener _httpListener;

        private readonly Dictionary<string, IHandler> _handlers = new Dictionary<string, IHandler>();

        private static Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

    }
}
