using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Middleware;

namespace InvestmentBuilderService.Session
{
    public delegate void SessionMessageHandler(Message message);

    public interface IConnectionSession
    {
        Task<bool> Connect();
        void RegisterMessageHandler(SessionMessageHandler handler);
        void SendMessageToChannel(string channel, string payload, string destination);
        Task<bool> RegisterAuthenticationServer(string identifier);
        void SendAuthenticationResult(bool result, string message, string requestid);
    }
}
