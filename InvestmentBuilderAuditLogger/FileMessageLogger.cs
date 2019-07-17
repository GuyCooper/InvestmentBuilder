using InvestmentBuilderCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InvestmentBuilderAuditLogger
{
    /// <summary>
    /// This class logs incoming and outgoing messages, including timestamps and message duration (the time
    /// between the incoming call and the outgoing call). Logs to file.
    /// </summary>
    public class FileMessageLogger : MessageLogger
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public FileMessageLogger(IConfigurationSettings settings)
        {
            m_fileName = settings.AuditFileName;
            File.WriteAllLines(m_fileName, new[] { "Time,User,Account,Incoming Request,Outgoing Response, Duration (ms)" });
        }

        #endregion

        #region Protected Override Methods

        /// <summary>
        /// Write an auditmessage to the audit file.
        /// </summary>
        protected override void WriteAuditMessage(AuditMessage auditMessage)
        {
            var outstr = $"{auditMessage.AuditTime},{auditMessage.User},{auditMessage.Account},{auditMessage.IncomingChannel},{auditMessage.OutgoingChannel},{auditMessage.DurationMS}";
            File.AppendAllLines(m_fileName, new[] { outstr });
        }

        #endregion

        #region Private Data

        private readonly string m_fileName;

        #endregion

    }
}
