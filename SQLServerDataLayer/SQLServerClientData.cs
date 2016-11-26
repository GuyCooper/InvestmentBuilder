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

        public IEnumerable<DateTime> GetRecentValuationDates(UserAccountToken userToken, DateTime dtDateFrom)
        {
            userToken.AuthorizeUser(AuthorizationLevel.READ);

            using (var command = new SqlCommand("sp_RecentValuationDates", Connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                command.Parameters.Add(new SqlParameter("@DateFrom", dtDateFrom));
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

        public DateTime? GetPreviousAccountValuationDate(UserAccountToken userToken, DateTime dtValuation)
        {
            userToken.AuthorizeUser(AuthorizationLevel.READ);
            DateTime? dtPrevious = null;
            using (var command = new SqlCommand("sp_GetPreviousValuationDate", Connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@valuationDate", dtValuation.Date));
                command.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                var result = command.ExecuteScalar();
                if (result != null)
                {
                    dtPrevious = (DateTime)result;
                }
            }
            return dtPrevious;
        }
    }
}
