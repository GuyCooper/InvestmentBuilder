using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

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
        private SqlConnection _connection;
        private DateTime _currentDate;
        private UserData _account;

        public DatabaseInvestment(UserData account, SqlConnection connection, DateTime dtValuationDate, string name)
        {
            _connection = connection;
            _currentDate = dtValuationDate;
            _account = account;
            Name = name;
            CompanyData = _GetCompanyData();
        }

        public string Name { get; private set; }

        public CompanyInformation CompanyData { get; private set; }

        public void DeactivateInvestment()
        {
            using (var command = new SqlCommand("sp_DeactivateCompany", _connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@Name", Name));
                command.Parameters.Add(new SqlParameter("@Account", _account.Name));
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
                command.Parameters.Add(new SqlParameter("@company", Name));
                command.Parameters.Add(new SqlParameter("@account", _account.Name));
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
                command.Parameters.Add(new SqlParameter("@company", Name));
                command.Parameters.Add(new SqlParameter("@account", _account.Name));
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
                command.Parameters.Add(new SqlParameter("@account", _account.Name));
                command.ExecuteNonQuery();
            }
        }

        public void UpdateClosingPrice(double dClosing)
        {
            using (var updateCommand = new SqlCommand("sp_UpdateClosingPrice", _connection))
            {
                updateCommand.CommandType = System.Data.CommandType.StoredProcedure;
                updateCommand.Parameters.Add(new SqlParameter("@valuationDate", _currentDate));
                updateCommand.Parameters.Add(new SqlParameter("@investment", Name));
                updateCommand.Parameters.Add(new SqlParameter("@closingPrice", dClosing));
                updateCommand.Parameters.Add(new SqlParameter("@account", _account.Name));
                updateCommand.ExecuteNonQuery();
            }
        }

        public void UpdateDividend(double dDividend)
        {
            using (var updateCommand = new SqlCommand("sp_UpdateDividend", _connection))
            {
                updateCommand.CommandType = System.Data.CommandType.StoredProcedure;
                updateCommand.Parameters.Add(new SqlParameter("@valuationDate", _currentDate));
                updateCommand.Parameters.Add(new SqlParameter("@company", Name));
                updateCommand.Parameters.Add(new SqlParameter("@dividend", dDividend));
                updateCommand.Parameters.Add(new SqlParameter("@account", _account.Name));
                updateCommand.ExecuteNonQuery();
            }
        }

        private CompanyInformation _GetCompanyData()
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
    }

    class InvestmentRecordBuilderDatabase : InvestmentRecordBuilder
    {
        SqlConnection _connection;
        public InvestmentRecordBuilderDatabase(SqlConnection connection)
        {
            _connection = connection;
        }

        override protected IEnumerable<IInvestment> GetInvestments(UserData account, DateTime valuationDate)
        {
            var companies = new List<string>();
            using (var command = new SqlCommand("sp_GetUserCompanies", _connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@ValuationDate", valuationDate));
                command.Parameters.Add(new SqlParameter("@Account", account.Name));
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    companies.Add((string)reader["Name"]);
                }
                reader.Close();
            }

            return companies.Select( c => new DatabaseInvestment(account, _connection, valuationDate, c));
        }

        override protected void CreateNewInvestment(UserData account, Stock newTrade, DateTime valuationDate, double dClosing)
        {   
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
                command.Parameters.Add(new SqlParameter("@account", account.Name));

                command.ExecuteNonQuery();
            }
        }
    }
}
