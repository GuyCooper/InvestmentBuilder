using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using NLog;
namespace InvestmentBuilder
{
    internal abstract class UserData
    {
        public string Name { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }

        public abstract void RollbackValuationDate(DateTime dtValuation);
        public abstract void UpdateMemberAccount(DateTime dtValuation, string member, double dAmount);
        public abstract double GetMemberSubscription(DateTime dtValuation, string member);
        public abstract IEnumerable<KeyValuePair<string, double>> GetMemberAccountData(DateTime dtValuation);
        public abstract double GetPreviousUnitValuation(DateTime dtValuation, DateTime? previousDate);
        public abstract void SaveNewUnitValue(DateTime dtValuation, double dUnitValue);
        public abstract double GetIssuedUnits(DateTime dtValuation);
        public abstract DateTime? GetPreviousValuationDate(DateTime dtValuation);
    }

    internal class DBUserData : UserData
    {
        private SqlConnection _connection;

        public DBUserData(SqlConnection conn)
        {
            _connection = conn;
        }

        public override void RollbackValuationDate(DateTime dtValuation)
        {
            using (var updateCommand = new SqlCommand("sp_RollbackUpdate", _connection))
            {
                updateCommand.CommandType = System.Data.CommandType.StoredProcedure;
                updateCommand.Parameters.Add(new SqlParameter("@Account", Name));
                updateCommand.Parameters.Add(new SqlParameter("@ValuationDate", dtValuation));
                updateCommand.ExecuteNonQuery();
            }
        }

        public override void UpdateMemberAccount(DateTime dtValuation, string member, double dAmount)
        {
            using (var updateCommand = new SqlCommand("sp_UpdateMembersCapitalAccount", _connection))
            {
                updateCommand.CommandType = System.Data.CommandType.StoredProcedure;
                updateCommand.Parameters.Add(new SqlParameter("@ValuationDate", dtValuation));
                updateCommand.Parameters.Add(new SqlParameter("@Member", member));
                updateCommand.Parameters.Add(new SqlParameter("@Units", dAmount));
                updateCommand.Parameters.Add(new SqlParameter("@Account", Name));
                updateCommand.ExecuteNonQuery();
            }
        }

        public override double GetMemberSubscription(DateTime dtValuation, string member)
        {
            double dSubscription = 0d;
            using (var command = new SqlCommand("sp_GetMemberSubscriptionAmount", _connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@Member", member));
                command.Parameters.Add(new SqlParameter("@ValuationDate", dtValuation));
                command.Parameters.Add(new SqlParameter("@Account", Name));
                dSubscription = (double)command.ExecuteScalar();
            }
            return dSubscription;
        }

        public override IEnumerable<KeyValuePair<string, double>> GetMemberAccountData(DateTime dtValuation)
        {
            using (var command = new SqlCommand("sp_GetMembersCapitalAccount", _connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@ValuationDate", dtValuation));
                command.Parameters.Add(new SqlParameter("@Account", Name));

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

        public override double GetPreviousUnitValuation(DateTime dtValuation, DateTime? previousDate)
        {
            using (var command = new SqlCommand("sp_GetUnitValuation", _connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@valuationDate", previousDate.Value));
                command.Parameters.Add(new SqlParameter("@Account", Name));
                return (double)command.ExecuteScalar();
            }
        }

        public override void SaveNewUnitValue(DateTime dtValuation, double dUnitValue)
        {
            //TODO - save new unit value
            using (var command = new SqlCommand("sp_AddNewUnitValuation", _connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@valuationDate", dtValuation));
                command.Parameters.Add(new SqlParameter("@unitValue", dUnitValue));
                command.Parameters.Add(new SqlParameter("@Account", Name));
                command.ExecuteNonQuery();
            }
        }

        public override double GetIssuedUnits(DateTime dtValuation)
        {
            using (var command = new SqlCommand("sp_GetIssuedUnits", _connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@valuationDate", dtValuation));
                command.Parameters.Add(new SqlParameter("@AccountName", Name));

                var result = command.ExecuteScalar();
                if(result != null)
                {
                    return (double)result;
                }
            }
            return 0d;
        }

        public override DateTime? GetPreviousValuationDate(DateTime dtValuation)
        {
            DateTime? dtPrevious = null;
            using (var command = new SqlCommand("sp_GetPreviousValuationDate", _connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@valuationDate", dtValuation));
                command.Parameters.Add(new SqlParameter("@Account", Name));
                var result = command.ExecuteScalar();
                if(result != null)
                {
                    dtPrevious = (DateTime)result;
                }
            }
            return dtPrevious;
        }
    }

    interface IUserDataReader
    {
        UserData GetUserData(string name);
    }

    class UserDataReaderDB :IUserDataReader
    {
        private  SqlConnection _connection;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public UserDataReaderDB(SqlConnection connection)
        {
            _connection = connection;
        }

        public UserData GetUserData(string name)
        {
            using (var command = new SqlCommand("sp_GetUserData", _connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@Name", name));
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new DBUserData(_connection)
                        {
                            Name = name,
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
