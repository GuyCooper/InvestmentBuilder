using System.Threading.Tasks;
using Middleware;
using InvestmentBuilderService.Session;
using Microsoft.Practices.Unity;

namespace InvestmentBuilderService
{
    /// <summary>
    /// Abstract EndpointManager class
    /// </summary>
    internal abstract class EndpointManager
    {
        private IConnectionSession _session;

        public EndpointManager(IConnectionSession session)
        {
            _session = session;
        }

        public virtual async Task<bool> Connect()
        {
            var connected = await _session.Connect();
            if(connected == true)
            {
                _session.RegisterMessageHandler(EndpointMessageHandler);
            }
            return connected;
        }

        public IConnectionSession GetSession()
        {
            return _session;
        }

        public abstract void RegisterChannels(IUnityContainer container);

        protected abstract void EndpointMessageHandler(Message message);
    }
}
