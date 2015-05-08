using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderCore;
using System.Data.SqlClient;

namespace SQLServerDataLayer
{
    class SQLServerHistoricalData : SQLServerBase, IHistoricalDataReader
    {
        public SQLServerHistoricalData(SqlConnection connection)
        {
            Connection = connection;
        }

        public IEnumerable<HistoricalData> GetHistoricalAccountData(string account)
        {
            using (var command = new SqlCommand("sp_GetUnitPriceData", Connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@Account", account));
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return new HistoricalData
                        {
                            Date = (DateTime)reader["Valuation_Date"],
                            Price = (double)reader["Unit_Price"]
                        };
                    }
                    reader.Close();
                }
            }
        }
    }
}
