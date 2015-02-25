using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using NLog;

namespace InvestmentBuilderClient.DataModel
{
    //ObservableCollection
    //BindingList

    internal class InvestmentDataModel : IDisposable, IInvestmentDataModel
    {
        private SqlConnection _connection;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private Dictionary<string, string> _typeProcedureLookup = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase)
        {
            {"Dividend", "SELECT NAME FROM Companies WHERE IsActive = 1"},
            {"Subscription", "SELECT NAME FROM Members"}
        };

        public InvestmentDataModel(string dataSource) 
        {
            //var connectstr = @"Data Source=TRAVELPC\SQLEXPRESS;Initial Catalog=InvestmentBuilderTest;Integrated Security=True";
             _connection = new SqlConnection(dataSource);
             _connection.Open();
             logger.Log(LogLevel.Info, "connected to datasource {0}", dataSource);
        }

        public IEnumerable<DateTime> GetValuationDates()
        {
            var dates = new List<DateTime>();
            using(var command = new SqlCommand("SELECT TOP 5 Valuation_Date FROM Valuations ORDER BY Valuation_Date DESC", _connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dates.Add(reader.GetDateTime(0));
                    }
                }
            }

            if(dates.Count > 0)
            {
                if(dates.First().Month != DateTime.Today.Month ||
                    dates.First().Year != DateTime.Today.Year)
                {
                    //nowon a different month so add the current date to the list
                    dates.Insert(0, DateTime.Today);
                }
            }
            return dates;
        }

        public IEnumerable<string> GetsTransactionTypes(string side)
        {
            using (var command = new SqlCommand("sp_GetTransactionTypes", _connection))
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

        public IEnumerable<string> GetParametersForType(string type)
        {
            if(_typeProcedureLookup.ContainsKey(type))
            {
                using (var command = new SqlCommand(_typeProcedureLookup[type], _connection))
                {
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return reader.GetString(0);
                        }
                    }
                }
            }
            else
            {
                yield return type;
            }
        }

       // public void GetCashAccountData(DateTime dtValuationDate, string side, Action<SqlDataReader> fnAddTransaction)
        public void GetCashAccountData(DateTime dtValuationDate, string side, Action<IDataReader> fnAddTransaction)
        {
            using (var sqlCommand = new SqlCommand("sp_GetCashAccountData", _connection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@ValuationDate", dtValuationDate));
                sqlCommand.Parameters.Add(new SqlParameter("@Side", side));
                using(var reader = sqlCommand.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        fnAddTransaction(reader);
                    }
                }
            }
        }

        public DateTime? GetLatestValuationDate()
        {
            using (var sqlCommand = new SqlCommand("sp_GetPreviousValuationDate", _connection))
            {
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

        public double GetBalanceInHand(DateTime dtValuation)
        {
            using (var sqlCommand = new SqlCommand("sp_GetBalanceInHand", _connection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@ValuationDate", dtValuation));
                using (var reader = sqlCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetDouble(0);
                    }
                }
            }
            return 0d;
        }

        public void SaveCashAccountData(DateTime dtValuationDate, DateTime dtTransactionDate,
                                    string type, string parameter, double amount)
        {
            using (var sqlCommand = new SqlCommand("sp_AddCashAccountData", _connection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@ValuationDate", dtValuationDate));
                sqlCommand.Parameters.Add(new SqlParameter("@TransactionDate", dtTransactionDate));
                sqlCommand.Parameters.Add(new SqlParameter("@TransactionType", type));
                sqlCommand.Parameters.Add(new SqlParameter("@Parameter", parameter));
                sqlCommand.Parameters.Add(new SqlParameter("@Amount", amount));
                sqlCommand.ExecuteNonQuery();
            }
        }

        public void ReloadData(string dataSource)
        {
            _connection.Close();
            _connection = new SqlConnection(dataSource);
            _connection.Open();
            logger.Log(LogLevel.Info, "reload from datasource {0}", dataSource);
        }

        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}
