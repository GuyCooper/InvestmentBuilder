using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Middleware;
using Newtonsoft.Json;
using InvestmentBuilderService.Channels;
using InvestmentBuilderService.Session;

namespace InvestmentBuilderService
{
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

        protected abstract void EndpointMessageHandler(Message message);
    }
}
