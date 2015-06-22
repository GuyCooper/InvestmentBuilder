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

        public IEnumerable<DateTime> GetRecentValuationDates(string account)
        {
            using (var command = new SqlCommand("sp_RecentValuationDates", Connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@Account", account));
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

        public IEnumerable<string> GetActiveCompanies(string account, DateTime valuationDate)
        {
            using (var command = new SqlCommand("sp_GetActiveCompanies", Connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@Account", account));
                command.Parameters.Add(new SqlParameter("@ValuationDate", valuationDate));
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader.GetString(0);
                    }
                }
            }
        }

        public IEnumerable<string> GetAccountMembers(string account, DateTime valuationDate)
        {
            using (var command = new SqlCommand("sp_GetAccountMembers", Connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@Account", account));
                command.Parameters.Add(new SqlParameter("@ValuationDate", valuationDate));
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader.GetString(0);
                    }
                }
            }

        }

        public void GetCashAccountData(string account, string side, DateTime valuationDate, Action<System.Data.IDataReader> fnAddTransaction)
        {
            if (fnAddTransaction == null)
            {
                return;
            }

            using (var sqlCommand = new SqlCommand("sp_GetCashAccountData", Connection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@ValuationDate", valuationDate));
                sqlCommand.Parameters.Add(new SqlParameter("@Side", side));
                sqlCommand.Parameters.Add(new SqlParameter("@Account", account));
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        fnAddTransaction(reader);
                    }
                }
            }
        }

        public DateTime? GetLatestValuationDate(string account)
        {
            using (var sqlCommand = new SqlCommand("sp_GetLatestValuationDate", Connection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@Account", account));
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

        public double GetBalanceInHand(string account, DateTime valuationDate)
        {
            using (var sqlCommand = new SqlCommand("sp_GetBalanceInHand", Connection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@ValuationDate", valuationDate));
                sqlCommand.Parameters.Add(new SqlParameter("@Account", account));
                var result = sqlCommand.ExecuteScalar();
                if (result is double)
                {
                    return (double)result;
                }
            }
            return 0d;

        }

        public void AddCashAccountData(string account, DateTime valuationDate, DateTime transactionDate, string type, string parameter, double amount)
        {
            using (var sqlCommand = new SqlCommand("sp_AddCashAccountData", Connection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@ValuationDate", valuationDate));
                sqlCommand.Parameters.Add(new SqlParameter("@TransactionDate", transactionDate));
                sqlCommand.Parameters.Add(new SqlParameter("@TransactionType", type));
                sqlCommand.Parameters.Add(new SqlParameter("@Parameter", parameter));
                sqlCommand.Parameters.Add(new SqlParameter("@Amount", amount));
                sqlCommand.Parameters.Add(new SqlParameter("@Account", account));
                sqlCommand.ExecuteNonQuery();
            }
        }

        public IEnumerable<string> GetAccountNames()
        {
            using (var command = new SqlCommand("SELECT Name FROM Users WHERE Enabled = 1", Connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return (string)reader["Name"];
                    }
                }
            }
        }

        public bool IsExistingValuationDate(string account, DateTime valuationDate)
        {
            using (var sqlCommand = new SqlCommand("sp_IsExistingValuationDate", Connection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@ValuationDate", valuationDate));
                sqlCommand.Parameters.Add(new SqlParameter("@Account", account));
                var result = sqlCommand.ExecuteScalar();
                return result != null;
            }
        }

        public void UpdateMemberForAccount(string account, string member, bool add)
        {
            using (var sqlCommand = new SqlCommand("sp_UpdateMemberForAccount", Connection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@Account", account));
                sqlCommand.Parameters.Add(new SqlParameter("@Member", member));
                sqlCommand.Parameters.Add(new SqlParameter("@Add", add ? 1 : 0));
                sqlCommand.ExecuteNonQuery();
            }
        }

        public void CreateAccount(AccountModel account)
        {
            using (var sqlCommand = new SqlCommand("sp_CreateAccount", Connection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@Name", account.Name));
                sqlCommand.Parameters.Add(new SqlParameter("@Password", account.Password));
                sqlCommand.Parameters.Add(new SqlParameter("@Description", account.Description));
                sqlCommand.Parameters.Add(new SqlParameter("@Currency", account.ReportingCurrency));
                sqlCommand.Parameters.Add(new SqlParameter("@AccountType", account.Type));
                sqlCommand.Parameters.Add(new SqlParameter("@Enabled", account.Enabled));
                sqlCommand.ExecuteNonQuery();
            }
        }

        public AccountModel GetAccount(string account)
        {
            using (var sqlCommand = new SqlCommand("sp_GetAccountData", Connection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@Account", account));
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
                            Enabled = (byte)reader["Enabled"] != 0 ? true : false
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
    }
}
