using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderCore;
using System.Data.SqlClient;
using System.Data;

namespace SQLServerDataLayer
{
    public class SQLServerUserAccountData : SQLServerBase, IUserAccountInterface
    {
        public SQLServerUserAccountData(SqlConnection connection)
        {
            Connection = connection;
        }

        public void RollbackValuationDate(UserAccountToken userToken, DateTime dtValuation)
        {
            userToken.AuthorizeUser(AuthorizationLevel.UPDATE);
            using (var updateCommand = new SqlCommand("sp_RollbackUpdate", Connection))
            {
                updateCommand.CommandType = System.Data.CommandType.StoredProcedure;
                updateCommand.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                updateCommand.Parameters.Add(new SqlParameter("@ValuationDate", dtValuation));
                updateCommand.ExecuteNonQuery();
            }
        }

        public void UpdateMemberAccount(UserAccountToken userToken, DateTime dtValuation, string member, double dAmount)
        {
            userToken.AuthorizeUser(AuthorizationLevel.UPDATE);
            using (var updateCommand = new SqlCommand("sp_UpdateMembersCapitalAccount", Connection))
            {
                updateCommand.CommandType = System.Data.CommandType.StoredProcedure;
                updateCommand.Parameters.Add(new SqlParameter("@ValuationDate", dtValuation));
                updateCommand.Parameters.Add(new SqlParameter("@Member", member));
                updateCommand.Parameters.Add(new SqlParameter("@Units", dAmount));
                updateCommand.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                updateCommand.ExecuteNonQuery();
            }
        }

        public double GetMemberSubscription(UserAccountToken userToken, DateTime dtValuation, string member)
        {
            userToken.AuthorizeUser(AuthorizationLevel.UPDATE);
            double dSubscription = 0d;
            using (var command = new SqlCommand("sp_GetMemberSubscriptionAmount", Connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@Member", member));
                command.Parameters.Add(new SqlParameter("@ValuationDate", dtValuation));
                command.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                //var oSubscription = (double)command.ExecuteScalar();
                //if(oSubscription != null)
                //{
                //    dSubscription = (double)oSubscription;
                //}
                using (var reader = command.ExecuteReader())
                {
                    if(reader.Read())
                    {
                        dSubscription = reader.GetDouble(0);
                    }
                    reader.Close();
                }
            }
            return dSubscription;
        }

        public IEnumerable<KeyValuePair<string, double>> GetMemberAccountData(UserAccountToken userToken, DateTime dtValuation)
        {
            userToken.AuthorizeUser(AuthorizationLevel.UPDATE);
            using (var command = new SqlCommand("sp_GetMembersCapitalAccount", Connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@ValuationDate", dtValuation));
                command.Parameters.Add(new SqlParameter("@Account", userToken.Account));

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var member = (string)reader["Member"];
                        var units = (double)reader["Units"];
                        yield return new KeyValuePair<string, double>(member, units);
                    }
                    reader.Close();
                }
            }
        }

        public double GetPreviousUnitValuation(UserAccountToken userToken, DateTime dtValuation, DateTime? previousDate)
        {
            userToken.AuthorizeUser(AuthorizationLevel.READ);
            if (previousDate.HasValue == false)
            {
                return 1d;  //if first time then unit value starts at 1
            }
            using (var command = new SqlCommand("sp_GetUnitValuation", Connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@valuationDate", previousDate.Value));
                command.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                return (double)command.ExecuteScalar();
            }
        }

        public void SaveNewUnitValue(UserAccountToken userToken, DateTime dtValuation, double dUnitValue)
        {
            userToken.AuthorizeUser(AuthorizationLevel.UPDATE);
            using (var command = new SqlCommand("sp_AddNewUnitValuation", Connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@valuationDate", dtValuation));
                command.Parameters.Add(new SqlParameter("@unitValue", dUnitValue));
                command.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                command.ExecuteNonQuery();
            }
        }

        public double GetIssuedUnits(UserAccountToken userToken, DateTime dtValuation)
        {
            userToken.AuthorizeUser(AuthorizationLevel.UPDATE);
            using (var command = new SqlCommand("sp_GetIssuedUnits", Connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@valuationDate", dtValuation));
                command.Parameters.Add(new SqlParameter("@AccountName", userToken.Account));

                var result = command.ExecuteScalar();
                if (result is double)
                {
                    return (double)result;
                }
            }
            return 0d;
        }

        public DateTime? GetPreviousAccountValuationDate(UserAccountToken userToken, DateTime dtValuation)
        {
            userToken.AuthorizeUser(AuthorizationLevel.READ);
            DateTime? dtPrevious = null;
            using (var command = new SqlCommand("sp_GetPreviousValuationDate", Connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@valuationDate", dtValuation));
                command.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                var result = command.ExecuteScalar();
                if (result != null)
                {
                    dtPrevious = (DateTime)result;
                }
            }
            return dtPrevious;
        }

        public IEnumerable<string> GetAccountMembers(UserAccountToken userToken)
        {
            userToken.AuthorizeUser(AuthorizationLevel.UPDATE);
            using (var sqlCommand = new SqlCommand("sp_GetAccountMembers", Connection))
            {
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                sqlCommand.Parameters.Add(new SqlParameter("@ValuationDate", DateTime.Today));
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return (string)reader["Name"];
                    }
                }
            }
        }

        public UserAccountData GetUserAccountData(UserAccountToken userToken)
        {
            userToken.AuthorizeUser(AuthorizationLevel.UPDATE);
            using (var command = new SqlCommand("sp_GetUserData", Connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@Name", userToken.Account));
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new UserAccountData
                        {
                            Name = userToken.Account,
                            Currency = (string)reader["Currency"],
                            Description = (string)reader["Description"]
                        };
                    }
                }
            }
            return null;
        }
    }
}
