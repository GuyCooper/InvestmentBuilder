using System;
using System.Collections.Generic;
using InvestmentBuilderCore;
using System.Data.SqlClient;
namespace SQLServerDataLayer
{
    /// <summary>
    /// SQLServer implementation of IHistoricalData interface.
    /// </summary>
    class SQLServerHistoricalData : SQLServerBase, IHistoricalDataReader
    {
        public SQLServerHistoricalData(string connectionStr)
        {
            ConnectionStr = connectionStr;
        }

        /// <summary>
        /// Method returns all the historial unit prices for this account. 
        /// </summary>
        public IEnumerable<HistoricalData> GetHistoricalAccountData(UserAccountToken userToken)
        {
            userToken.AuthorizeUser(AuthorizationLevel.READ);
            using (var connection = OpenConnection())
            {
                using (var command = new SqlCommand("sp_GetUnitPriceData", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Account", userToken.Account.AccountId));
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return new HistoricalData
                            (
                                date: GetDBValue<DateTime>("Valuation_Date", reader),
                                price: GetDBValue<double>("Unit_Price", reader)
                            );
                        }
                        reader.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Returns all the historical prices for the index specified by the symbol
        /// </summary>
        public string GetIndexHistoricalData(UserAccountToken userToken, string symbol)
        {
            userToken.AuthorizeUser(AuthorizationLevel.READ);
            string result = null;
            using (var connection = OpenConnection())
            {
                using (var command = new SqlCommand("sp_GetHistoricalData", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Symbol", symbol));
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result = GetDBValue<string>("Data", reader);
                        }
                        reader.Close();
                    }
                }
            }
            return result;
        }

        public Dictionary<string, List<Tuple<int, double>>> GetHistoricalYieldData(UserAccountToken userToken)
        {
            userToken.AuthorizeUser(AuthorizationLevel.READ);
            var result = new Dictionary<string, List<Tuple<int, double>>>();
            using (var connection = OpenConnection())
            {
                using (var command = new SqlCommand("sp_GetHistoricalYieldData", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    using (var reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            var name = GetDBValue<string>("Name", reader);
                            var year = GetDBValue<int>("Year", reader);
                            var yield = GetDBValue<double>("Yield", reader);

                            if(!result.ContainsKey(name))
                            {
                                result.Add(name, new List<Tuple<int, double>>());
                            }

                            result[name].Add(Tuple.Create(year, yield));
                        }
                    }
                }
            }

            return result;

        }
    }
}
