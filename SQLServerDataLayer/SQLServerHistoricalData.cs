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

        public IEnumerable<HistoricalData> GetHistoricalAccountData(UserAccountToken userToken)
        {
            userToken.AuthorizeUser(AuthorizationLevel.READ);
            using (var command = new SqlCommand("sp_GetUnitPriceData", Connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return new HistoricalData
                        (
                            date: (DateTime)reader["Valuation_Date"],
                            price: (double)reader["Unit_Price"]
                        );
                    }
                    reader.Close();
                }
            }
        }
    }
}
