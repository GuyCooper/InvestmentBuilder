﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderCore;
using System.Data.SqlClient;
using System.Globalization;
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
                            date: GetDBValue<DateTime>("Valuation_Date", reader),
                            price: GetDBValue<double>("Unit_Price", reader)
                        );
                    }
                    reader.Close();
                }
            }
        }

        public string GetIndexHistoricalData(UserAccountToken userToken, string symbol)
        {
            userToken.AuthorizeUser(AuthorizationLevel.READ);
            string result = null;
            using (var command = new SqlCommand("sp_GetHistoricalData", Connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@Symbol", symbol));
                using (var reader = command.ExecuteReader())
                {
                    if(reader.Read())
                    {
                        result = GetDBValue<string>("Data", reader);
                    }
                    reader.Close();
                }
            }
            return result;
        }
    }
}
