using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderCore;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Globalization;
namespace SQLServerDataLayer
{
    internal class RawHistoricalData
    {
        public string date { get; set; }
        public string price { get; set; }
    }

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
                            date: GetDBValue<DateTime>("Valuation_Date", reader),
                            price: GetDBValue<double>("Unit_Price", reader)
                        );
                    }
                    reader.Close();
                }
            }
        }

        public IEnumerable<HistoricalData> GetIndexHistoricalData(UserAccountToken userToken, string symbol, DateTime? dtFrom)
        {
            userToken.AuthorizeUser(AuthorizationLevel.READ);
            using (var command = new SqlCommand("sp_GetHistoricalData", Connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@Symbol", symbol));
                using (var reader = command.ExecuteReader())
                {
                    if(reader.Read())
                    {
                        string data = GetDBValue<string>("Data", reader);
                        var rawData = JsonConvert.DeserializeObject<IList<RawHistoricalData>>(data);
                        foreach(var item in rawData)
                        {
                            var date = DateTime.ParseExact(item.date, "M/d/yyyy", CultureInfo.InvariantCulture);
                            if (dtFrom.HasValue == false || (date >= dtFrom.Value))
                            {
                                if (string.IsNullOrEmpty(item.price) == false)
                                {
                                    yield return new HistoricalData
                                    (
                                        date: date,
                                        price: double.Parse(item.price)
                                    );
                                }
                            }
                        }
                    }
                    reader.Close();
                }
            }
        }
    }
}
