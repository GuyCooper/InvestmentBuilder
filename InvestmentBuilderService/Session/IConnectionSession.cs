using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Middleware;

namespace InvestmentBuilderService.Session
{
    public delegate void SessionMessageHandler(Message message);

    /// <summary>
    /// Interface defines a session to a middleware layer
    /// </summary>
    public interface IConnectionSession
    {
        Task<bool> Connect();
        void RegisterMessageHandler(SessionMessageHandler handler);
        void SendMessageToChannel(string channel, string payload, string destination, string requestId, byte[] binaryPayload);
        void BroadcastMessage(string channel, string payload);
        Task<bool> RegisterAuthenticationServer(string identifier);
        void SendAuthenticationResult(bool result, string message, string requestid);
        void RegisterChannelListener(string channel);
    }
}
