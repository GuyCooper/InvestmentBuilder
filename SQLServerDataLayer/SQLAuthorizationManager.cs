using System;
using System.Collections.Generic;
using System.Linq;
using InvestmentBuilderCore;
using System.Data.SqlClient;

namespace SQLServerDataLayer
{
    //implementation class returns the authorization level for a user  
    public class SQLAuthorizationManager : AuthorizationManager,IDisposable
    {
        private string _connectionStr;

        private SqlConnection OpenConnection()
        {
            var connection = new SqlConnection(_connectionStr);
            connection.Open();
            return connection;
        }

        public SQLAuthorizationManager(IConfigurationSettings settings)
        {
            _connectionStr = settings.DatasourceString;
        }

        protected override bool IsGlobalAdministrator(string user)
        {
            using (var connection = OpenConnection())
            {
                using (var command = new SqlCommand("sp_IsAdministrator", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@User", user));
                    var objResult = command.ExecuteScalar();
                    if (objResult != null)
                    {
                        return string.Equals((string)objResult, user, StringComparison.InvariantCultureIgnoreCase);
                    }
                }
            }
            return false;
        }

        protected override AuthorizationLevel GetUserAuthorizationLevel(string user, string account)
        {
            using (var connection = OpenConnection())
            {
                using (var command = new SqlCommand("sp_GetAuthorizationLevel", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@User", user));
                    command.Parameters.Add(new SqlParameter("@Account", account));
                    var objResult = command.ExecuteScalar();
                    if (objResult != null)
                    {
                        return (AuthorizationLevel)objResult;
                    }
                }
            }
            return AuthorizationLevel.NONE;
        }

        public void ConnectNewDatasource(string datasource)
        {
            _connectionStr = datasource;
        }

        public void Dispose()
        {
            //_connection.Close();
        }
    }
}
