using System;
using System.Collections.Generic;
using System.Linq;
using InvestmentBuilderCore;
using System.Data.SqlClient;
using System.Data;

namespace SQLServerDataLayer
{
    public class SQLServerUserAccountData : SQLServerBase, IUserAccountInterface
    {
        public SQLServerUserAccountData(string connectionStr)
        {
            ConnectionStr = connectionStr;
        }

        public void RollbackValuationDate(UserAccountToken userToken, DateTime dtValuation)
        {
            userToken.AuthorizeUser(AuthorizationLevel.UPDATE);
            using (var connection = OpenConnection())
            {
                using (var updateCommand = new SqlCommand("sp_RollbackUpdate", connection))
                {
                    updateCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    updateCommand.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                    updateCommand.Parameters.Add(new SqlParameter("@ValuationDate", dtValuation));
                    updateCommand.ExecuteNonQuery();
                }
            }
        }

        public void UpdateMemberAccount(UserAccountToken userToken, DateTime dtValuation, string member, double dAmount)
        {
            userToken.AuthorizeUser(AuthorizationLevel.UPDATE);
            using (var connection = OpenConnection())
            {
                using (var updateCommand = new SqlCommand("sp_UpdateMembersCapitalAccount", connection))
                {
                    updateCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    updateCommand.Parameters.Add(new SqlParameter("@ValuationDate", dtValuation.Date));
                    updateCommand.Parameters.Add(new SqlParameter("@Member", member));
                    updateCommand.Parameters.Add(new SqlParameter("@Units", dAmount));
                    updateCommand.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                    updateCommand.ExecuteNonQuery();
                }
            }
        }

        public double GetMemberSubscription(UserAccountToken userToken, DateTime dtValuation, string member)
        {
            userToken.AuthorizeUser(AuthorizationLevel.UPDATE);
            double dSubscription = 0d;
            using (var connection = OpenConnection())
            {
                using (var command = new SqlCommand("sp_GetMemberSubscriptionAmount", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Member", member));
                    command.Parameters.Add(new SqlParameter("@ValuationDate", dtValuation.Date));
                    command.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                    //var oSubscription = (double)command.ExecuteScalar();
                    //if(oSubscription != null)
                    //{
                    //    dSubscription = (double)oSubscription;
                    //}
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            dSubscription = reader.GetDouble(0);
                        }
                        reader.Close();
                    }
                }
            }
            return dSubscription;
        }

        public IEnumerable<KeyValuePair<string, double>> GetMemberAccountData(UserAccountToken userToken, DateTime dtValuation)
        {
            userToken.AuthorizeUser(AuthorizationLevel.UPDATE);
            using (var connection = OpenConnection())
            {
                using (var command = new SqlCommand("sp_GetMembersCapitalAccount", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@ValuationDate", dtValuation.Date));
                    command.Parameters.Add(new SqlParameter("@Account", userToken.Account));

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var member = GetDBValue<string>("Member", reader);
                            var units = GetDBValue<double>("Units", reader);
                            yield return new KeyValuePair<string, double>(member, units);
                        }
                        reader.Close();
                    }
                }
            }
        }

        public double GetPreviousUnitValuation(UserAccountToken userToken, DateTime? previousDate)
        {
            userToken.AuthorizeUser(AuthorizationLevel.READ);
            if (previousDate.HasValue == false)
            {
                return 1d;  //if first time then unit value starts at 1
            }

            using (var connection = OpenConnection())
            {
                using (var command = new SqlCommand("sp_GetUnitValuation", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@valuationDate", previousDate.Value));
                    command.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                    return (double)command.ExecuteScalar();
                }
            }
        }

        public void SaveNewUnitValue(UserAccountToken userToken, DateTime dtValuation, double dUnitValue)
        {
            userToken.AuthorizeUser(AuthorizationLevel.UPDATE);
            using (var connection = OpenConnection())
            {
                using (var command = new SqlCommand("sp_AddNewUnitValuation", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@valuationDate", dtValuation));
                    command.Parameters.Add(new SqlParameter("@unitValue", dUnitValue));
                    command.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                    command.ExecuteNonQuery();
                }
            }
        }

        public double GetIssuedUnits(UserAccountToken userToken, DateTime dtValuation)
        {
            userToken.AuthorizeUser(AuthorizationLevel.UPDATE);
            using (var connection = OpenConnection())
            {
                using (var command = new SqlCommand("sp_GetIssuedUnits", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@valuationDate", dtValuation.Date));
                    command.Parameters.Add(new SqlParameter("@AccountName", userToken.Account));

                    var result = command.ExecuteScalar();
                    if (result is double)
                    {
                        return (double)result;
                    }
                }
            }
            return 0d;
        }

        public UserAccountData GetUserAccountData(UserAccountToken userToken)
        {
            if(userToken == null || userToken.Account == null)
            {
                return null;
            }

            userToken.AuthorizeUser(AuthorizationLevel.UPDATE);
            using (var connection = OpenConnection())
            {
                using (var command = new SqlCommand("sp_GetUserData", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Name", userToken.Account));
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new UserAccountData
                            (
                                userToken.Account,
                                (string)reader["Currency"],
                                GetDBValue<string>("Description", reader),
                                GetDBValue<string>("Broker", reader)
                            );
                        }
                    }
                }
            }
            return null;
        }

        public double GetStartOfYearValuation(UserAccountToken userToken, DateTime valuationDate)
        {
            userToken.AuthorizeUser(AuthorizationLevel.READ);

            DateTime dtStartOfYear = valuationDate.Month > 1 ? valuationDate.AddMonths(1 - valuationDate.Month) : valuationDate;
            if (dtStartOfYear.Day > 1)
                dtStartOfYear = dtStartOfYear.AddDays(1 - dtStartOfYear.Day);

            using (var connection = OpenConnection())
            {
                using (var command = new SqlCommand("sp_GetStartOfYearValuation", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@valuationDate", dtStartOfYear));
                    command.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                    var objResult = command.ExecuteScalar();
                    if (objResult != null)
                    {
                        return (double)command.ExecuteScalar();
                    }
                }
            }
            return 1d;
        }

        public IEnumerable<Redemption> GetRedemptions(UserAccountToken userToken, DateTime valuationDate)
        {
            using (var connection = OpenConnection())
            {
                using (var sqlCommand = new SqlCommand("sp_GetRedemptions", connection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                    sqlCommand.Parameters.Add(new SqlParameter("@TransactionDate", valuationDate));
                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return new Redemption
                            (
                                GetDBValue<string>("UserName", reader),
                                GetDBValue<double>("amount", reader),
                                GetDBValue<DateTime>("transaction_date", reader),
                                (RedemptionStatus)Enum.Parse(typeof(RedemptionStatus), (string)reader["status"])
                            );
                        }
                    }
                }
            }
        }
 
        public void AddRedemption(UserAccountToken userToken, string user, DateTime transactionDate, double amount)
        {
            userToken.AuthorizeUser(AuthorizationLevel.UPDATE);

            using (var connection = OpenConnection())
            {
                using (var command = new SqlCommand("sp_AddRedemption", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                    command.Parameters.Add(new SqlParameter("@User", user));
                    command.Parameters.Add(new SqlParameter("@TransactionDate", transactionDate.Date));
                    command.Parameters.Add(new SqlParameter("@Amount", amount));
                    command.Parameters.Add(new SqlParameter("@Status", RedemptionStatus.Pending.ToString()));

                    command.ExecuteScalar();
                }
            }
        }

        public void UpdateRedemption(UserAccountToken userToken, string user, DateTime transactionDate, double amount, double units)
        {
            userToken.AuthorizeUser(AuthorizationLevel.UPDATE);

            using (var connection = OpenConnection())
            {
                using (var command = new SqlCommand("sp_UpdateRedemption", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                    command.Parameters.Add(new SqlParameter("@User", user));
                    command.Parameters.Add(new SqlParameter("@TransactionDate", transactionDate.Date));
                    command.Parameters.Add(new SqlParameter("@Amount", amount));
                    command.Parameters.Add(new SqlParameter("@UnitsRedeemed", units));
                    command.Parameters.Add(new SqlParameter("@Status", RedemptionStatus.Complete.ToString()));

                    command.ExecuteScalar();
                }
            }
        }

        public IEnumerable<string> GetAccountMembers(UserAccountToken userToken, DateTime valuationDate)
        {
            return GetAccountMemberDetails(userToken, valuationDate).Select(x => x.Name);
        }

        public IEnumerable<AccountMember> GetAccountMemberDetails(UserAccountToken userToken, DateTime valuationDate)
        {
            userToken.AuthorizeUser(AuthorizationLevel.UPDATE);
            using (var connection = OpenConnection())
            {
                using (var command = new SqlCommand("sp_GetAccountMembers", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                    command.Parameters.Add(new SqlParameter("@ValuationDate", valuationDate));
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return new AccountMember(
                                GetDBValue<string>("UserName", reader),
                                (AuthorizationLevel)reader["Authorization"]
                            );
                        }
                    }
                }
            }
        }

        public void UpdateMemberForAccount(UserAccountToken userToken, string member, AuthorizationLevel level, bool add)
        {
            userToken.AuthorizeUser(AuthorizationLevel.ADMINISTRATOR);
            using (var connection = OpenConnection())
            {
                using (var sqlCommand = new SqlCommand("sp_UpdateMemberForAccount", connection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                    sqlCommand.Parameters.Add(new SqlParameter("@Member", member));
                    sqlCommand.Parameters.Add(new SqlParameter("@Level", (int)level));
                    sqlCommand.Parameters.Add(new SqlParameter("@Add", add ? 1 : 0));
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public void CreateAccount(UserAccountToken userToken, AccountModel account)
        {
            userToken.AuthorizeUser(AuthorizationLevel.ADMINISTRATOR);
            using (var connection = OpenConnection())
            {
                using (var sqlCommand = new SqlCommand("sp_CreateAccount", connection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@Name", account.Name));
                    sqlCommand.Parameters.Add(new SqlParameter("@Currency", account.ReportingCurrency));
                    sqlCommand.Parameters.Add(new SqlParameter("@AccountType", account.Type));
                    sqlCommand.Parameters.Add(new SqlParameter("@Enabled", account.Enabled));
                    sqlCommand.Parameters.Add(new SqlParameter("@Description", account.Description));
                    sqlCommand.Parameters.Add(new SqlParameter("@Broker", account.Broker));
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public AccountModel GetAccount(UserAccountToken userToken)
        {
            userToken.AuthorizeUser(AuthorizationLevel.ADMINISTRATOR);
            using (var connection = OpenConnection())
            {
                using (var sqlCommand = new SqlCommand("sp_GetAccountData", connection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            //var obj = reader["Enabled"];
                            return new AccountModel(GetDBValue<string>("Name", reader),
                                                    GetDBValue<string>("Description", reader),
                                                    GetDBValue<string>("Currency", reader),
                                                    GetDBValue<string>("Type", reader),
                                                    (byte)reader["Enabled"] != 0 ? true : false,
                                                    GetDBValue<string>("Broker", reader),
                                                    null
                                                    );
                        }
                    }
                }
            }
            return null;
        }

        public IEnumerable<string> GetAccountNames(string user, bool bCheckAdmin)
        {
            //return the list of accounts that this user is able to see
            using (var connection = OpenConnection())
            {
                using (var sqlCommand = new SqlCommand("sp_GetAccountsForUser", connection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@User", user));
                    sqlCommand.Parameters.Add(new SqlParameter("@CheckAdmin", bCheckAdmin ? 1 : 0));
                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return reader.GetString(0);
                        }
                    }
                }
            }
        }

        public IEnumerable<string> GetActiveCompanies(UserAccountToken userToken, DateTime valuationDate)
        {
            userToken.AuthorizeUser(AuthorizationLevel.READ);
            using (var connection = OpenConnection())
            {
                using (var command = new SqlCommand("sp_GetActiveCompanies", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                    command.Parameters.Add(new SqlParameter("@ValuationDate", valuationDate));
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return (string)reader["Name"];
                        }
                    }
                }
            }
        }

        public bool InvestmentAccountExists(string accountName)
        {
            using (var connection = OpenConnection())
            {
                using (var command = new SqlCommand("sp_InvestmentAccountExists", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Name", accountName));
                    var objResult = command.ExecuteScalar();
                    if (objResult != null)
                    {
                        return (int)objResult == 1;
                    }
                }
            }
            return false;
        }

        public IEnumerable<double> GetUnitValuationRange(UserAccountToken userToken, DateTime dateFrom, DateTime dateTo)
        {
            userToken.AuthorizeUser(AuthorizationLevel.READ);
            var ret = new List<double>();
            using (var connection = OpenConnection())
            {
                using (var command = new SqlCommand("sp_GetUnitValuationRange", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                    command.Parameters.Add(new SqlParameter("@dateFrom", dateFrom));
                    command.Parameters.Add(new SqlParameter("@dateTo", dateTo));
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ret.Add((double)reader["Unit_Price"]);
                        }
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// return the user id for the specified user. throws an exception if the user does not exist.
        /// </summary>
        public int GetUserId(string userName)
        {
            using (var connection = OpenConnection())
            {
                using (var command = new SqlCommand("sp_GetUserId", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@UserName", userName));
                    var objResult = command.ExecuteScalar();
                    if (objResult != null)
                    {
                        return (int)objResult;
                    }
                 }
            }
            return -1;
        }

        /// <summary>
        /// Add a new user to the database
        /// </summary>
        public void AddUser(string userName, string description)
        {
            using (var connection = OpenConnection())
            {
                using (var command = new SqlCommand("sp_AddNewUser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@UserName", userName));
                    command.Parameters.Add(new SqlParameter("@Description", description));
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
