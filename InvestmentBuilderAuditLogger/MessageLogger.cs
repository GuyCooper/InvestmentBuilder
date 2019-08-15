using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestmentBuilderAuditLogger
{
    /// <summary>
    /// IMessageLogger interface. logs incoming and outgoing messages.
    /// </summary>
    public interface IMessageLogger
    {
        /// <summary>
        /// Log incoming message.
        /// </summary>
        void LogIncomingMessage(string requestID, string user, string account, string channel, string payload);
        /// <summary>
        /// Log outgoing message.
        /// </summary>
        void LogOutgoingMessage(string requestID, string channel, string payload);
    }

    /// <summary>
    /// Audit message contains a message including message timestamp
    /// </summary>
    public class AuditMessage
    {
        // Audit time of message
        public DateTime AuditTime { get; set; }

        // Username.
        public string User { get; set; }

        // Account.
        public string Account { get; set; }

        // Incoming Channel.
        public string IncomingChannel { get; set; }

        // Outgoing Channel.
        public string OutgoingChannel { get; set; }

        // Duration of request in milliseconds.
        public double DurationMS { get; set; }
    }

    /// <summary>
    /// Base class for IMessageLogger. Cmmon functionality.
    /// </summary>
    public abstract class MessageLogger : IMessageLogger
    {
        #region IMessageLogger

        /// <summary>
        /// Log an incoming message.
        /// </summary>
        public void LogIncomingMessage(string requestID, string user, string account, string channel, string payload)
        {
            m_messageLookup[requestID] = new AuditMessage { User = user, Account = account, IncomingChannel = channel, AuditTime = DateTime.UtcNow };
        }

        /// <summary>
        /// Log an outgoing message
        /// </summary>
        public void LogOutgoingMessage(string requestID, string channel, string payload)
        {
            AuditMessage auditMessage;
            if (m_messageLookup.TryGetValue(requestID, out auditMessage) == true)
            {
                auditMessage.OutgoingChannel = channel;
                auditMessage.DurationMS = TimeSpan.FromTicks(DateTime.UtcNow.Ticks - auditMessage.AuditTime.Ticks).Milliseconds;
                WriteAuditMessage(auditMessage);
                m_messageLookup.Remove(requestID);
            }
        }

        #endregion

        #region Protected Abstract Methods

        /// <summary>
        /// Write audit message to storage.
        /// </summary>
        protected abstract void WriteAuditMessage(AuditMessage auditMessage);

        #endregion

        #region Private Data

        private readonly Dictionary<string, AuditMessage> m_messageLookup = new Dictionary<string, AuditMessage>();

        #endregion
    }
}
