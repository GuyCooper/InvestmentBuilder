using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace InvestmentBuilderAuditLogger
{
    /// <summary>
    /// Class used for reading an SQL Lite audit log
    /// </summary>
    public class SQLiteAuditMessageReader : IDisposable
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public SQLiteAuditMessageReader(string auditFile)
        {
            var datasource = $"DataSource={auditFile}";

            m_connection = new SQLiteConnection(datasource);

            m_connection.Open();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a list of all the audit messages.
        /// </summary>
        public IEnumerable<AuditMessage> GetMessages()
        {
            using (var command = new SQLiteCommand(m_connection))
            {
                command.CommandText = @"SELECT 
                                        TIMESTAMP,USER,ACCOUNT,INCOMING_REQUEST,OUTGOING_REQUEST,DURATION
                                        FROM
                                        Audit
                                      ";
                var reader = command.ExecuteReader();
                int column = 0;
                while (reader.Read())
                {
                    yield return new AuditMessage
                    {
                        AuditTime = reader.GetDateTime(column++),
                        User = reader.GetString(column++),
                        Account = reader.GetString(column++),
                        IncomingChannel = reader.GetString(column++),
                        OutgoingChannel = reader.GetString(column++),
                        DurationMS = reader.GetDouble(column)
                    };

                    reader.NextResult();
                }
            }
        }

        /// <summary>
        /// Dispose method.
        /// </summary>
        public void Dispose()
        {
            m_connection.Dispose();
        }

        #endregion

        #region Private Data

        System.Data.SQLite.SQLiteConnection m_connection;

        #endregion
    }
}
