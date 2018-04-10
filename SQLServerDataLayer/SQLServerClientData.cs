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
        public SQLServerClientData(string connectionStr)
        {
            ConnectionStr = connectionStr;
        }

        public IEnumerable<DateTime> GetRecentValuationDates(UserAccountToken userToken, DateTime dtDateFrom)
        {
            userToken.AuthorizeUser(AuthorizationLevel.READ);

            using (var connection = OpenConnection())
            {
                using (var command = new SqlCommand("sp_RecentValuationDates", connection))
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
        }

        public IEnumerable<string> GetTransactionTypes(string side)
        {
            using (var connection = OpenConnection())
            {
                using (var command = new SqlCommand("sp_GetTransactionTypes", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@side", side));
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return GetDBValue<string>("type", reader);
                        }
                    }
                }
            }
        }

        public DateTime? GetLatestValuationDate(UserAccountToken userToken)
        {
            userToken.AuthorizeUser(AuthorizationLevel.UPDATE);
            using (var connection = OpenConnection())
            {
                using (var sqlCommand = new SqlCommand("sp_GetLatestValuationDate", connection))
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
            }
            return null;
        }

        public bool IsExistingValuationDate(UserAccountToken userToken, DateTime valuationDate)
        {
            userToken.AuthorizeUser(AuthorizationLevel.READ);
            using (var connection = OpenConnection())
            {
                using (var sqlCommand = new SqlCommand("sp_IsExistingValuationDate", connection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@ValuationDate", valuationDate));
                    sqlCommand.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                    var result = sqlCommand.ExecuteScalar();
                    return result != null;
                }
            }
        }

        public IEnumerable<string> GetAccountTypes()
        {
            using (var connection = OpenConnection())
            {
                using (var sqlCommand = new SqlCommand("SELECT [Type] FROM UserTypes", connection))
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

        public IEnumerable<string> GetAllCompanies()
        {
            using (var connection = OpenConnection())
            {
                using (var sqlCommand = new SqlCommand("SELECT [Name] FROM Companies", connection))
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

        public Stock GetTradeItem(UserAccountToken userToken, string name)
        {
            if(string.IsNullOrEmpty(name))
            {
                return null;
            }

            userToken.AuthorizeUser(AuthorizationLevel.READ);
            using (var connection = OpenConnection())
            {
                using (var sqlCommand = new SqlCommand("sp_GetTradeItem", connection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@Company", name));
                    sqlCommand.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            //var obj = reader["Enabled"];
                            int quantity = GetDBValue<int>("Shares_Bought", reader) - GetDBValue<int>("Shares_Sold", reader);
                            return new Stock
                            {
                                Name = GetDBValue<string>("Name", reader),
                                TransactionDate = GetDBValue<DateTime>("LastBoughtDate", reader),
                                Symbol = GetDBValue<string>("Symbol", reader),
                                Exchange = GetDBValue<string>("Exchange", reader),
                                Currency = GetDBValue<string>("Currency", reader),
                                Quantity = quantity,
                                TotalCost = GetDBValue<double>("Total_Cost", reader),
                                ScalingFactor = GetDBValue<double>("ScalingFactor", reader)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public void UndoLastTransaction(UserAccountToken userToken)
        {
            userToken.AuthorizeUser(AuthorizationLevel.UPDATE);
            using (var connection = OpenConnection())
            {
                using (var sqlCommand = new SqlCommand("sp_UndoLastTransaction", connection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@account", userToken.Account));
                    int rowsUpdated = sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public DateTime? GetPreviousAccountValuationDate(UserAccountToken userToken, DateTime dtValuation)
        {
            if(userToken.Account == null)
            {
                //user is not a member of any account
                return null;
            }
            userToken.AuthorizeUser(AuthorizationLevel.READ);
            DateTime? dtPrevious = null;
            using (var connection = OpenConnection())
            {
                using (var command = new SqlCommand("sp_GetPreviousValuationDate", connection))
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
            }
            return dtPrevious;
        }
    }
}
