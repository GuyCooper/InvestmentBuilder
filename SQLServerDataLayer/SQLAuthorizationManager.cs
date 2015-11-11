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
        private SqlConnection _connection;

        public SQLAuthorizationManager(IConfigurationSettings settings)
        {
            _connection = new SqlConnection(settings.DatasourceString);
            _connection.Open();
        }

        protected override bool IsGlobalAdministrator(string user)
        {
            using (var command = new SqlCommand("sp_IsAdministrator", _connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@User", user));
                var objResult = command.ExecuteScalar();
                if (objResult != null)
                {
                    return string.Equals((string)objResult, user, StringComparison.InvariantCultureIgnoreCase);
                }
            }
            return false;
        }

        protected override AuthorizationLevel GetUserAuthorizationLevel(string user, string account)
        {
            using (var command = new SqlCommand("sp_GetAuthorizationLevel", _connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@User", user));
                command.Parameters.Add(new SqlParameter("@Account", account));
                var objResult = command.ExecuteScalar();
               if(objResult != null)
               {
                   return (AuthorizationLevel)objResult;
               }
            }
            return AuthorizationLevel.NONE;
        }

        public void ConnectNewDatasource(string datasource)
        {
            _connection.Close();
            _connection = new SqlConnection(datasource);
            _connection.Open();
        }

        public void Dispose()
        {
            _connection.Close();
        }
    }
}
