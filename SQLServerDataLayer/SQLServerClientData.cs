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
    /// <summary>
    /// SQL Serverimplementation of the IClientDataInterface
    /// </summary>
    public class SQLServerClientData : SQLServerBase, IClientDataInterface
    {
        public SQLServerClientData(SqlConnection connection)
        {
            Connection = connection;
        }

        public IEnumerable<DateTime> GetRecentValuationDates(UserAccountToken userToken)
        {
            userToken.AuthorizeUser(AuthorizationLevel.READ);

            using (var command = new SqlCommand("sp_RecentValuationDates", Connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader.GetDateTime(0);
                    }
                }
            }
        }

        public IEnumerable<string> GetTransactionTypes(string side)
        {
            using (var command = new SqlCommand("sp_GetTransactionTypes", Connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@side", side));
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return (string)reader["type"];
                    }
                }
            }
        }

        public IEnumerable<string> GetActiveCompanies(UserAccountToken userToken, DateTime valuationDate)
        {
            userToken.AuthorizeUser(AuthorizationLevel.READ);
            using (var command = new SqlCommand("sp_GetActiveCompanies", Connection))
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

        public IEnumerable<string> GetAccountMembers(UserAccountToken userToken, DateTime valuationDate)
        {
            return GetAccountMemberDetails(userToken, valuationDate).Select(x => x.Key);
        }

        public IEnumerable<KeyValuePair<string, AuthorizationLevel>> GetAccountMemberDetails(UserAccountToken userToken, DateTime valuationDate)
        {
            userToken.AuthorizeUser(AuthorizationLevel.UPDATE);
            using (var command = new SqlCommand("sp_GetAccountMembers", Connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                command.Parameters.Add(new SqlParameter("@ValuationDate", valuationDate));
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return new KeyValuePair<string, AuthorizationLevel>(
                                                (string)reader["Name"],
                                                (AuthorizationLevel)reader["Authorization"]);
                    }
                }
            }
        }

        public void GetCashAccountData(UserAccountToken userToken, string side, DateTime valuationDate, Action<System.Data.IDataReader> fnAddTransaction)
        {
            userToken.AuthorizeUser(AuthorizationLevel.READ);
            if (fnAddTransaction == null)
            {
                return;
            }

            using (var sqlCommand = new SqlCommand("sp_GetCashAccountData", Connection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@ValuationDate", valuationDate));
                sqlCommand.Parameters.Add(new SqlParameter("@Side", side));
                sqlCommand.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        fnAddTransaction(reader);
                    }
                }
            }
        }

        public DateTime? GetLatestValuationDate(UserAccountToken userToken)
        {
            userToken.AuthorizeUser(AuthorizationLevel.UPDATE);
            using (var sqlCommand = new SqlCommand("sp_GetLatestValuationDate", Connection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                using (var reader = sqlCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetDateTime(0);
                    }
                }
            }
            return null;
        }

        public double GetBalanceInHand(UserAccountToken userToken, DateTime valuationDate)
        {
            userToken.AuthorizeUser(AuthorizationLevel.READ);
            using (var sqlCommand = new SqlCommand("sp_GetBalanceInHand", Connection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@ValuationDate", valuationDate));
                sqlCommand.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                var result = sqlCommand.ExecuteScalar();
                if (result is double)
                {
                    return (double)result;
                }
            }
            return 0d;

        }

        public void AddCashAccountData(UserAccountToken userToken, DateTime valuationDate, DateTime transactionDate, string type, string parameter, double amount)
        {
            userToken.AuthorizeUser(AuthorizationLevel.UPDATE);
            using (var sqlCommand = new SqlCommand("sp_AddCashAccountData", Connection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@ValuationDate", valuationDate));
                sqlCommand.Parameters.Add(new SqlParameter("@TransactionDate", transactionDate));
                sqlCommand.Parameters.Add(new SqlParameter("@TransactionType", type));
                sqlCommand.Parameters.Add(new SqlParameter("@Parameter", parameter));
                sqlCommand.Parameters.Add(new SqlParameter("@Amount", amount));
                sqlCommand.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                sqlCommand.ExecuteNonQuery();
            }
        }

        public IEnumerable<string> GetAccountNames(string user)
        {
            //return the list of accounts that this user is able to see

            using (var sqlCommand = new SqlCommand("sp_GetAccountsForUser", Connection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@User", user));
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        yield return reader.GetString(0);
                    }
                }
            }
        }

        public bool IsExistingValuationDate(UserAccountToken userToken, DateTime valuationDate)
        {
            userToken.AuthorizeUser(AuthorizationLevel.READ);
            using (var sqlCommand = new SqlCommand("sp_IsExistingValuationDate", Connection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@ValuationDate", valuationDate));
                sqlCommand.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                var result = sqlCommand.ExecuteScalar();
                return result != null;
            }
        }

        public void UpdateMemberForAccount(UserAccountToken userToken, string member, AuthorizationLevel level, bool add)
        {
            userToken.AuthorizeUser(AuthorizationLevel.ADMINISTRATOR);
            using (var sqlCommand = new SqlCommand("sp_UpdateMemberForAccount", Connection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                sqlCommand.Parameters.Add(new SqlParameter("@Member", member));
                sqlCommand.Parameters.Add(new SqlParameter("@Level", (int)level));
                sqlCommand.Parameters.Add(new SqlParameter("@Add", add ? 1 : 0));
                sqlCommand.ExecuteNonQuery();
            }
        }

        public void CreateAccount(UserAccountToken userToken, AccountModel account)
        {
            userToken.AuthorizeUser(AuthorizationLevel.ADMINISTRATOR);
            using (var sqlCommand = new SqlCommand("sp_CreateAccount", Connection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@Name", account.Name));
                sqlCommand.Parameters.Add(new SqlParameter("@Password", account.Password));
                sqlCommand.Parameters.Add(new SqlParameter("@Description", account.Description));
                sqlCommand.Parameters.Add(new SqlParameter("@Currency", account.ReportingCurrency));
                sqlCommand.Parameters.Add(new SqlParameter("@AccountType", account.Type));
                sqlCommand.Parameters.Add(new SqlParameter("@Enabled", account.Enabled));
                sqlCommand.Parameters.Add(new SqlParameter("@Broker", account.Broker));
                sqlCommand.ExecuteNonQuery();
            }
        }

        public AccountModel GetAccount(UserAccountToken userToken)
        {
            userToken.AuthorizeUser(AuthorizationLevel.ADMINISTRATOR);
            using (var sqlCommand = new SqlCommand("sp_GetAccountData", Connection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                using (var reader = sqlCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        //var obj = reader["Enabled"];
                        return new AccountModel
                        {
                            Name = (string)reader["Name"],
                            Password = (string)reader["Password"],
                            Description = (string)reader["Description"],
                            ReportingCurrency = (string)reader["Currency"],
                            Enabled = (byte)reader["Enabled"] != 0 ? true : false,
                            Broker = (string)reader["Broker"],
                            Type = (string)reader["Type"]
                        };
                    }
                }
            }
            return null;
        }

        public IEnumerable<string> GetAccountTypes()
        {
            using (var sqlCommand = new SqlCommand("SELECT [Type] FROM UserTypes", Connection))
            {
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader.GetString(0);
                    }
                }
            }

        }

        public IEnumerable<string> GetAllCompanies()
        {
            using (var sqlCommand = new SqlCommand("SELECT [Name] FROM Companies", Connection))
            {
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader.GetString(0);
                    }
                }
            }
        }

        public Stock GetTradeItem(UserAccountToken userToken, string name)
        {
            userToken.AuthorizeUser(AuthorizationLevel.READ);
            using (var sqlCommand = new SqlCommand("sp_GetTradeItem", Connection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@Company", name));
                sqlCommand.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                using (var reader = sqlCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        //var obj = reader["Enabled"];
                        int quantity = (int)reader["Shares_Bought"] - (int)reader["Shares_Sold"];
                        var dtBoughtDate = (DateTime)reader["LastBoughtDate"];
                        var exchange = reader["Exchange"];
                        return new Stock
                        {
                            Name = (string)reader["Name"],
                            TransactionDate = dtBoughtDate,
                            Symbol = (string)reader["Symbol"],
                            Exchange = exchange.GetType() != typeof(System.DBNull) ? (string)exchange : null,
                            Currency = (string)reader["Currency"],
                            Quantity = quantity,
                            TotalCost = (double)reader["Total_Cost"],
                            ScalingFactor = (double)reader["ScalingFactor"]
                        };
                    }
                }
            }
            return null;
        }

        public void UndoLastTransaction(UserAccountToken userToken)
        {
            userToken.AuthorizeUser(AuthorizationLevel.UPDATE);
            using (var sqlCommand = new SqlCommand("sp_UndoLastTransaction", Connection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@account", userToken.Account));
                int rowsUpdated = sqlCommand.ExecuteNonQuery();
            }
        }
    }
}
