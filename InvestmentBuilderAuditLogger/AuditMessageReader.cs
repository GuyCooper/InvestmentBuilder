using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestmentBuilderAuditLogger
{
    /// <summary>
    /// Class reads audit messages from a datastore
    /// </summary>
    public interface IAuditMessageReader
    {
        /// <summary>
        /// Return a list of audit messages.
        /// </summary>
        IEnumerable<AuditMessage> GetMessages();
    }

    /// <summary>
    /// Class reads audit messages from a datastore
    /// </summary>
    public class FileAuditMessageReader : IAuditMessageReader
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public FileAuditMessageReader(string fileName)
        {
            m_fileName = fileName;
        }

        #endregion

        #region IMessageLogger

        /// <summary>
        /// Returns a list of all the audit messages.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AuditMessage> GetMessages()
        {
            var lines = File.ReadAllLines(m_fileName).Skip(1);
            foreach (var line in lines)
            {
                var parts = line.Split(',');
                yield return new AuditMessage
                {
                    AuditTime = DateTime.Parse(parts[0]),
                    User = parts[1],
                    Account = parts[2],
                    IncomingChannel = parts[3],
                    OutgoingChannel = parts[4],
                    DurationMS = double.Parse(parts[5])
                };
            }
        }

        #endregion

        #region Private Data

        private readonly string m_fileName;

        #endregion

    }
}
