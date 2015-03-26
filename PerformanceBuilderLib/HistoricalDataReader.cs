using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketDataServices;
using System.Data.SqlClient;

namespace PerformanceBuilderLib
{
    class HistoricalDataReader : IDisposable
    {
        private SqlConnection _connection;
        public HistoricalDataReader(string connectionStr)
        {
            _connection = new SqlConnection(connectionStr);
            _connection.Open();
        }

        public IEnumerable<HistoricalData> GetClubData(string account)
        {
            using (var command = new SqlCommand("sp_GetUnitPriceData", _connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@Account", account));
                using (var reader = command.ExecuteReader())
                {
                    while(reader.Read())
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

        public void Dispose()
        {
            _connection.Close();
        }
    }
}
