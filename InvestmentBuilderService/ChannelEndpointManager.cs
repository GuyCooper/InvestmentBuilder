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
using System.Reflection;
using Microsoft.Practices.Unity;
using InvestmentBuilderService;

namespace InvestmentBuilderService
{
    class ChannelEndpointManager : EndpointManager
    {
        private Dictionary<string, IEndpointChannel> _channels;
        private ISessionManager _sessionManager;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public ChannelEndpointManager(IConnectionSession session, ISessionManager sessionManager) 
            : base(session)
        {
            _channels = new Dictionary<string, IEndpointChannel>();
            _sessionManager = sessionManager;
        }

        public static bool ChannelInterfaceFilter(Type typeObj, object criteriaObj)
        {
            if (typeObj.ToString() == criteriaObj.ToString())
                return true;
            else
                return false;
        }

        private IEnumerable<Type> IsEndpointChannelClass(Type objType)
        {
            return
            from interfaceType in objType.GetInterfaces()
            where interfaceType == typeof(IEndpointChannel)
            select interfaceType;
        }

        public override void RegisterChannels(IUnityContainer container )
        {
            //register and resolve all concrete classes in this module that are derived 
            //from EndpointChannel
            //first find a list of all the endpoint channels types
            var channelTypes = new List<Type>();
            foreach ( var t in System.Reflection.Assembly.GetCallingAssembly().GetTypes())
            {
                if((t.IsAbstract == false)&&(IsEndpointChannelClass(t).ToList().Count > 0))
                {
                    channelTypes.Add(t);
                }
            }

            //now register each endpoint channel
            foreach(var channel in channelTypes)
            {
                ContainerManager.RegisterType(channel, true);
            }

            //now resolve (instantiate) each endpointchannel and register the 
            //channels with the middleware layer
            foreach (var channel in channelTypes)
            {
                var endpoint = ContainerManager.ResolveValueOnContainer<IEndpointChannel>(channel, container);
                if(endpoint != null)
                {
                    if (string.IsNullOrEmpty(endpoint.RequestName) == false)
                    {
                        GetSession().RegisterChannelListener(endpoint.RequestName);
                    }

                    RegisterChannel(endpoint);
                }
            }
        }

        public void RegisterChannel(IEndpointChannel channel)
        {
            if(channel != null)
            {
                if(_channels.ContainsKey(channel.RequestName) == true)
                {
                    logger.Log(LogLevel.Error, "duplicate channel name {0}!!", channel.RequestName);
                    return;
                }
                _channels.Add(channel.RequestName, channel);
            }
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

            IEndpointChannel channel;
            if (_channels.TryGetValue(message.Channel, out channel) == true)
            {
                Task.Factory.StartNew(() =>
                {
                    channel.ProcessMessage(GetSession(), userSession, message.Payload, message.SourceId, message.RequestId);
                });
            }
            else
            {
                logger.Log(LogLevel.Error, "invalid channel : {0}", message.Channel);
            }
        }
    }
}
