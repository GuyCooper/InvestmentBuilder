using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using MarketDataServices;

namespace InvestmentBuilder
{
    class DatabaseInvestment : IInvestment
    {
        SqlConnection _connection;
        DateTime _currentDate;

        public DatabaseInvestment(SqlConnection connection)
        {
            _connection = connection;
            _currentDate = _GetCurrentValuationDate();
        }

        public string Name { get; set; }

        public void DeactivateInvestment()
        {
            string strSql = string.Format("UPDATE Companies SET IsActive = 0 WHERE Name = '{0}'", Name);
            using (var command = new SqlCommand(strSql, _connection))
            {
                command.ExecuteNonQuery();
            }
        }

        private DateTime _GetCurrentValuationDate()
        {
            DateTime dtReturn = DateTime.Now;
            //string strSql = string.Format("SELECT MAX(Valuation_Date) as latestDate FROM InvestmentRecord");
            using (var command = new SqlCommand("sp_GetLatestValuationDate", _connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@valuationDate", dtReturn)
                {
                    Direction = System.Data.ParameterDirection.Output,
                    DbType = System.Data.DbType.DateTime
                });
                var reader = command.ExecuteNonQuery();
            }
            return dtReturn;
        }

        public DateTime? UpdateRow(DateTime valuationDate)
        {
            DateTime? dtPreviousValaution = _GetCurrentValuationDate();

            using (var command = new SqlCommand("sp_RollInvestment", _connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@ValuationDate", valuationDate));
                command.Parameters.Add(new SqlParameter("@Investment", Name));
                command.ExecuteNonQuery();
            }
            return dtPreviousValaution;
        }

        public void ChangeShareHolding(int holding)
        {
            using (var command = new SqlCommand("sp_UpdateHolding", _connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@Holding", holding));
                command.Parameters.Add(new SqlParameter("@ValuationDate", _currentDate));
                command.Parameters.Add(new SqlParameter("@Investment", Name));
                command.ExecuteNonQuery();
            }
        }

        public void AddNewShares(Stock stock)
        {
            using (var command = new SqlCommand("sp_AddNewShares", _connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@ValuationDate", _currentDate));
                command.Parameters.Add(new SqlParameter("@Investment", Name));
                command.Parameters.Add(new SqlParameter("@Shares", stock.Number));
                command.Parameters.Add(new SqlParameter("@TotalCost", stock.TotalCost));
                command.ExecuteNonQuery();
            }
        }

        public void UpdateClosingPrice()
        {
            DateTime dtReturn = DateTime.Now;
            string strSql = string.Format("SELECT Symbol, Currency, ScalingFactor");
            using (var command = new SqlCommand(strSql, _connection))
            {
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    string symbol = (string)reader["Symbol"];
                    string currency = (string)reader["Currency"];
                    double scalingFactor = (double)reader["ScalingFactor"];
                    double dClosing;
                    if (MarketDataService.TryGetClosingPrice(Name, symbol, currency, scalingFactor, out dClosing))
                    {
                        using (var updateCommand = new SqlCommand("sp_UpdateClosingPrice", _connection))
                        {
                            updateCommand.CommandType = System.Data.CommandType.StoredProcedure;
                            updateCommand.Parameters.Add(new SqlParameter("@ValuationDate", _currentDate));
                            updateCommand.Parameters.Add(new SqlParameter("@Investment", Name));
                            updateCommand.Parameters.Add(new SqlParameter("@ClosingPrice", dClosing));
                            updateCommand.ExecuteNonQuery();
                        }
                    }
                }
                reader.Close();
            }
        }

        public void UpdateDividend(double dDividend)
        {
            using (var updateCommand = new SqlCommand("sp_UpdateDividend", _connection))
            {
                updateCommand.CommandType = System.Data.CommandType.StoredProcedure;
                updateCommand.Parameters.Add(new SqlParameter("@ValuationDate", _currentDate));
                updateCommand.Parameters.Add(new SqlParameter("@Investment", Name));
                updateCommand.Parameters.Add(new SqlParameter("@Dividend", dDividend));
                updateCommand.ExecuteNonQuery();
            }
        }
    }

    class InvestmentRecordBuilderDatabase : InvestmentRecordBuilder
    {
        SqlConnection _connection;
        public InvestmentRecordBuilderDatabase(SqlConnection connection)
        {
            _connection = connection;
        }

        override protected IEnumerable<IInvestment> GetInvestments()
        {
            using (var command = new SqlCommand("SELECT Name FROM Companies", _connection))
            {
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    yield return new DatabaseInvestment(_connection)
                    {
                        Name = (string)reader["name"]
                    };

                }
                reader.Close();
            }
        }

        override protected void CreateNewInvestment(Stock newTrade, DateTime valuationDate)
        {
            double dClosing;
            MarketDataService.TryGetClosingPrice(newTrade.Name, newTrade.Symbol, newTrade.Currency, newTrade.ScalingFactor, out dClosing);
            
            using (var command = new SqlCommand("sp_CreateNewInvestment", _connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@ValuationDate", valuationDate));
                command.Parameters.Add(new SqlParameter("@Investment", newTrade.Name));
                command.Parameters.Add(new SqlParameter("@Symbol", newTrade.Symbol));
                command.Parameters.Add(new SqlParameter("@Currency", newTrade.Currency));
                command.Parameters.Add(new SqlParameter("@ScalingFactor", newTrade.ScalingFactor));
                command.Parameters.Add(new SqlParameter("@Shares", newTrade.Number));
                command.Parameters.Add(new SqlParameter("@TotalCost", newTrade.TotalCost));
                command.Parameters.Add(new SqlParameter("@ClosingPrice", dClosing));

                command.ExecuteNonQuery();
            }
        }
    }
}
