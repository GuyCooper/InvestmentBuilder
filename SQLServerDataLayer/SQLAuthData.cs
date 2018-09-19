using InvestmentBuilderCore;
using System.Data.SqlClient;

namespace SQLServerDataLayer
{
    public class SQLAuthData : IAuthDataLayer
    {
        #region Private Member Data

        private string _connectionStr;

        #endregion

        #region Public Methods

        private SqlConnection OpenConnection()
        {
            var connection = new SqlConnection(_connectionStr);
            connection.Open();
            return connection;
        }

        public SQLAuthData(IConfigurationSettings settings)
        {
            _connectionStr = settings.AuthDatasourceString;
        }

        /// <summary>
        /// add new user to auth table
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="eMail"></param>
        /// <param name="salt"></param>
        /// <param name="passwordHash"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="twoFactorEnabled"></param>
        /// <returns>0: success. -1 : command failed , 1 : empty email, 2: empty password, 3, user already exists</returns>
        public int AddNewUser(string userName, string eMail, string salt, string passwordHash, string phoneNumber, bool twoFactorEnabled)
        {
            using (var connection = OpenConnection())
            {
                using (var command = new SqlCommand("sp_AuthAddNewUser", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@UserName", userName ?? ""));
                    command.Parameters.Add(new SqlParameter("@EMail", eMail));
                    command.Parameters.Add(new SqlParameter("@Salt", salt));
                    command.Parameters.Add(new SqlParameter("@PasswordHash", passwordHash));
                    command.Parameters.Add(new SqlParameter("@PhoneNumber", phoneNumber));
                    command.Parameters.Add(new SqlParameter("@TwoFactorEnabled", twoFactorEnabled));
                    var objResult = command.ExecuteScalar();
                    if (objResult != null)
                    {
                        return (int)objResult;
                    }
                }
            }
            return -1;
        }

        public bool AuthenticateUser(string email, string passwordHash)
        {
            using (var connection = OpenConnection())
            {
                using (var command = new SqlCommand("sp_AuthenticateUser", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@EMail", email));
                    command.Parameters.Add(new SqlParameter("@PasswordHash", passwordHash));
                    var objResult = command.ExecuteScalar();
                    if (objResult != null)
                    {
                        return (int)objResult == 1;
                    }
                }
            }
            return false;
        }

        public bool ChangePassword(string email, string oldPasswordHash, string newPasswordHash, string newSalt)
        {
            using (var connection = OpenConnection())
            {
                using (var command = new SqlCommand("sp_ChangePassword", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@EMail", email));
                    command.Parameters.Add(new SqlParameter("@OldPasswordHash", oldPasswordHash));
                    command.Parameters.Add(new SqlParameter("@NewPasswordHash", newPasswordHash));
                    command.Parameters.Add(new SqlParameter("@NewSalt", newSalt));
                    var objResult = command.ExecuteScalar();
                    if (objResult != null)
                    {
                        return (int)objResult == 1;
                    }
                }
            }
            return false;
        }

        public void RemoveUser(string email)
        {
            using (var connection = OpenConnection())
            {
                using (var command = new SqlCommand("sp_AuthRemoveUser", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@EMail", email));
                    command.ExecuteNonQuery();
                }
            }
        }

        public string GetSalt(string email)
        {
            using (var connection = OpenConnection())
            {
                using (var command = new SqlCommand("sp_AuthGetSalt", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@EMail", email));
                    var objResult = command.ExecuteScalar();
                    if (objResult != null)
                    {
                        return (string)objResult;
                    }
                }
            }
            return null;
        }

        #endregion

    }
}
