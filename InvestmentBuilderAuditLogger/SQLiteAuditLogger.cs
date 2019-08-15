using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using InvestmentBuilderCore;

namespace InvestmentBuilderAuditLogger
{
    /// <summary>
    /// This class logs incoming and outgoing messages, including timestamps and message duration (the time
    /// between the incoming call and the outgoing call). Logs to an SQLite database
    /// </summary>
    public class SQLiteAuditLogger : MessageLogger , IDisposable
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        //public SQLiteAuditLogger(string auditFile)
        //{
        //    LoadDatabase(auditFile, false);
        //}

        /// <summary>
        /// Constructor from injection.
        /// </summary>
        public SQLiteAuditLogger(IConfigurationSettings settings)
        {
            var dataFile = settings.AuditFileName;
            LoadDatabase(dataFile, true);
        }

        #endregion

        #region Protected Override Methods

        /// <summary>
        /// Write an auditmessage to the audit file.
        /// </summary>
        protected override void WriteAuditMessage(AuditMessage auditMessage)
        {
            using (var command = new SQLiteCommand(m_connection))
            {
                command.CommandText = @"INSERT INTO Audit
                                       (TIMESTAMP,USER,ACCOUNT,INCOMING_REQUEST,OUTGOING_REQUEST,DURATION)
                                       VALUES (?,?,?,?,?,?)
                                      ";
                command.Parameters.Add(new SQLiteParameter("TimeStamp", auditMessage.AuditTime));
                command.Parameters.Add(new SQLiteParameter("USER", auditMessage.User));
                command.Parameters.Add(new SQLiteParameter("ACCOUNT", auditMessage.Account));
                command.Parameters.Add(new SQLiteParameter("INCOMING_REQUEST", auditMessage.IncomingChannel));
                command.Parameters.Add(new SQLiteParameter("OUTGOING_REQUEST", auditMessage.OutgoingChannel));
                command.Parameters.Add(new SQLiteParameter("DURATION", auditMessage.DurationMS));

                command.ExecuteNonQuery();
            }
        }

        #endregion

        #region IDisposable 

        /// <summary>
        /// Dispose Method.
        /// </summary>
        public void Dispose()
        {
            m_connection.Dispose();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Load the database.
        /// </summary>
        private void LoadDatabase(string auditFile, bool truncate)
        {
            if (string.IsNullOrEmpty(auditFile) == true)
            {
                throw new ApplicationException("Audit File not defined");
            }

            if(truncate)
            {
                File.Delete(auditFile);
            }

            var datasource = $"DataSource={auditFile}";

            m_connection = new SQLiteConnection(datasource);

            m_connection.Open();

            //create an audit table
            using (var command = new SQLiteCommand(m_connection))
            {
                command.CommandText = @"CREATE TABLE Audit
                                        (
                                            TIMESTAMP DATETIME,
                                            USER TEXT NOT NULL,
                                            ACCOUNT TEXT,
                                            INCOMING_REQUEST TEXT NOT NULL,
                                            OUTGOING_REQUEST TEXT NOT NULL,
                                            DURATION FLOAT        
                                        )
                                      ";
                command.ExecuteNonQuery();
            }
        }

        #endregion

        #region Private Data

        System.Data.SQLite.SQLiteConnection m_connection;

        #endregion
    }
}
