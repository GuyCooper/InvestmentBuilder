using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using MarketDataServices;

namespace InvestmentBuilder
{
    internal class CompanyInformation
    {
        public string Symbol { get; set; }
        public string Currency { get; set; }
        public double ScalingFactor { get; set; }
    }

    class DatabaseInvestment : IInvestment
    {
        SqlConnection _connection;
        DateTime _currentDate;

        public DatabaseInvestment(SqlConnection connection, DateTime dtValuationDate)
        {
            _connection = connection;
            _currentDate = dtValuationDate;
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

        public void UpdateRow(DateTime valuationDate, DateTime? previousDate)
        {
            using (var command = new SqlCommand("sp_RollInvestment", _connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@valuationDate", valuationDate));
                command.Parameters.Add(new SqlParameter("@previousDate", previousDate.Value));
                command.Parameters.Add(new SqlParameter("@investment", Name));
                command.ExecuteNonQuery();
            }
        }

        public void ChangeShareHolding(int holding)
        {
            using (var command = new SqlCommand("sp_UpdateHolding", _connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@holding", holding));
                command.Parameters.Add(new SqlParameter("@valuationDate", _currentDate));
                command.Parameters.Add(new SqlParameter("@investment", Name));
                command.ExecuteNonQuery();
            }
        }

        public void AddNewShares(Stock stock)
        {
            using (var command = new SqlCommand("sp_AddNewShares", _connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@valuationDate", _currentDate));
                command.Parameters.Add(new SqlParameter("@investment", Name));
                command.Parameters.Add(new SqlParameter("@shares", stock.Number));
                command.Parameters.Add(new SqlParameter("@totalCost", stock.TotalCost));
                command.ExecuteNonQuery();
            }
        }

        private CompanyInformation GetCompanyData()
        {
            CompanyInformation data = null;
            using (var command = new SqlCommand("sp_GetCompanyData", _connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@Name", Name));
                 using (var reader = command.ExecuteReader())
                 {
                     if (reader.Read())
                     {
                         var symbol = (string)reader["Symbol"];
                         var ccy = (string)reader["Currency"];
                         data = new CompanyInformation
                         {
                             Symbol = symbol.Trim(),
                             Currency = ccy.Trim(),
                             ScalingFactor = (double)reader["ScalingFactor"]
                         };
                     }
                     reader.Close();
                 }
            }
            return data;
        }

        public void UpdateClosingPrice()
        {
            var companyData = GetCompanyData();
            if(companyData != null)
            {
                double dClosing;
                if (MarketDataService.TryGetClosingPrice(companyData.Symbol, Name, companyData.Currency, companyData.ScalingFactor, out dClosing))
                {
                    using (var updateCommand = new SqlCommand("sp_UpdateClosingPrice", _connection))
                    {
                        updateCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        updateCommand.Parameters.Add(new SqlParameter("@valuationDate", _currentDate));
                        updateCommand.Parameters.Add(new SqlParameter("@investment", Name));
                        updateCommand.Parameters.Add(new SqlParameter("@closingPrice", dClosing));
                        updateCommand.ExecuteNonQuery();
                    }
                }
            }
        }

        public void UpdateDividend(double dDividend)
        {
            using (var updateCommand = new SqlCommand("sp_UpdateDividend", _connection))
            {
                updateCommand.CommandType = System.Data.CommandType.StoredProcedure;
                updateCommand.Parameters.Add(new SqlParameter("@valuationDate", _currentDate));
                updateCommand.Parameters.Add(new SqlParameter("@investment", Name));
                updateCommand.Parameters.Add(new SqlParameter("@dividend", dDividend));
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

        override protected IEnumerable<IInvestment> GetInvestments(DateTime valuationDate)
        {
            using (var command = new SqlCommand("SELECT Name FROM Companies", _connection))
            {
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    yield return new DatabaseInvestment(_connection, valuationDate)
                    {
                        Name = (string)reader["Name"]
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
                command.Parameters.Add(new SqlParameter("@valuationDate", valuationDate));
                command.Parameters.Add(new SqlParameter("@investment", newTrade.Name));
                command.Parameters.Add(new SqlParameter("@symbol", newTrade.Symbol));
                command.Parameters.Add(new SqlParameter("@currency", newTrade.Currency));
                command.Parameters.Add(new SqlParameter("@scalingFactor", newTrade.ScalingFactor));
                command.Parameters.Add(new SqlParameter("@shares", newTrade.Number));
                command.Parameters.Add(new SqlParameter("@totalCost", newTrade.TotalCost));
                command.Parameters.Add(new SqlParameter("@closingPrice", dClosing));
                command.Parameters.Add(new SqlParameter("@dividend", 0d));

                command.ExecuteNonQuery();
            }
        }
    }
}
