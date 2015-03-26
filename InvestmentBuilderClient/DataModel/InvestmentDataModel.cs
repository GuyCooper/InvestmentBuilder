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
        private string _Account;

        public DateTime? LatestDate { get; set; }

        private Dictionary<string, string> _typeProcedureLookup = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase)
        {
            {"Dividend", "sp_GetActiveCompanies"},
            {"Subscription", "sp_GetAccountMembers"}
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
            using (var command = new SqlCommand("sp_RecentValuationDates", _connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@Account", _Account));
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
            else
            {
                dates.Add(DateTime.Today);
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
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Account", _Account));
                    command.Parameters.Add(new SqlParameter("@ValuationDate", LatestDate));
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
                sqlCommand.Parameters.Add(new SqlParameter("@Account", _Account));
                using(var reader = sqlCommand.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        fnAddTransaction(reader);
                    }
                }
            }
        }

        private void GetLatestValuationDate()
        {
            using (var sqlCommand = new SqlCommand("sp_GetLatestValuationDate", _connection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@Account", _Account));
                using (var reader = sqlCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        LatestDate = reader.GetDateTime(0);
                    }
                }
            }
        }

        public double GetBalanceInHand(DateTime dtValuation)
        {
            using (var sqlCommand = new SqlCommand("sp_GetBalanceInHand", _connection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@ValuationDate", dtValuation));
                sqlCommand.Parameters.Add(new SqlParameter("@Account", _Account));
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
                sqlCommand.Parameters.Add(new SqlParameter("@Account", _Account));
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

        public IEnumerable<string> GetAccountNames()
        {
            using (var command = new SqlCommand("SELECT Name FROM Users WHERE Enabled = 1", _connection))
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

        public bool IsExistingValuationDate(DateTime dtValuation)
        {
            using (var sqlCommand = new SqlCommand("sp_IsExistingValuationDate", _connection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@ValuationDate", dtValuation));
                sqlCommand.Parameters.Add(new SqlParameter("@Account", _Account));
                var result = sqlCommand.ExecuteScalar();
                return result != null;
            }
        }

        public IEnumerable<string> GetAccountMembers(string account)
        {
            using (var sqlCommand = new SqlCommand("sp_GetAccountMembers", _connection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@Account", account));
                sqlCommand.Parameters.Add(new SqlParameter("@ValuationDate", DateTime.Today));
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        yield return reader.GetString(0);
                    }
                }
            }
        }

        private void _UpdateMemberForAccount(string account, string member, bool bAdd)
        {
            using (var sqlCommand = new SqlCommand("sp_UpdateMemberForAccount", _connection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@Account", account));
                sqlCommand.Parameters.Add(new SqlParameter("@Member", member));
                sqlCommand.Parameters.Add(new SqlParameter("@Add", bAdd ? 1 : 0));
                sqlCommand.ExecuteNonQuery();
            }
        }

        public void UpdateUserAccount(AccountModel account)
        {
            logger.Log(LogLevel.Info, "creating/modifying account {0}", account.Name);
            logger.Log(LogLevel.Info, "Password {0}", account.Password);
            logger.Log(LogLevel.Info, "Description {0}", account.Description);
            logger.Log(LogLevel.Info, "Reporting Currency {0}", account.ReportingCurrency);
            logger.Log(LogLevel.Info, "Account Type {0}", account.Type);
            logger.Log(LogLevel.Info, "Enabled {0}", account.Enabled);

            using (var sqlCommand = new SqlCommand("sp_CreateAccount", _connection)) 

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

            var existingMembers = GetAccountMembers(account.Name);
            foreach(var member in existingMembers)
            {
                if(account.Members.Contains(member, StringComparer.CurrentCultureIgnoreCase) == false)
                {
                    //remove this member
                    logger.Log(LogLevel.Info, "removing member {0} from account {1}", member, account.Name);
                    _UpdateMemberForAccount(account.Name, member, false);
                }
            }

            //now add the members
            foreach (var member in account.Members)
            {
                logger.Log(LogLevel.Info, "adding member {0} to account {1}", member, account.Name);
                _UpdateMemberForAccount(account.Name, member, true);
            }
        }

        public IEnumerable<string> GetAccountTypes()
        {
            using (var sqlCommand = new SqlCommand("SELECT [Type] FROM UserTypes", _connection))
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

        public AccountModel GetAccountData(string account)
        {
            AccountModel data = null;
            using (var sqlCommand = new SqlCommand("sp_GetAccountData", _connection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@Account", account));
                using (var reader = sqlCommand.ExecuteReader())
                {
                    if(reader.Read())
                    {
                        //var obj = reader["Enabled"];
                        data = new AccountModel
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

            if(data != null)
            {
                data.Members = GetAccountMembers(data.Name).ToList();
            }

            return data;
        }

        public void UpdateAccountName(string account)
        {
            logger.Log(LogLevel.Info, "updating to account {0} ", account);
            _Account = account;
            GetLatestValuationDate();
        }

        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}
