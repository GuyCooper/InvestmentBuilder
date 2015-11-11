using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderCore;
using System.Data.SqlClient;
using System.Data;

namespace SQLServerDataLayer
{
    public class SQLServerCashAccountData : SQLServerBase, ICashAccountInterface
    {
        public SQLServerCashAccountData(SqlConnection connection)
        {
            Connection = connection;
        }

        public CashAccountData GetCashAccountData(UserAccountToken userToken, DateTime valuationDate)
        {
            userToken.AuthorizeUser(AuthorizationLevel.READ);

            var cashData = new CashAccountData();

            //retrieve the current bank balance
            using (SqlCommand cmdBankBalance = new SqlCommand("sp_GetBankBalance", Connection))
            {
                cmdBankBalance.CommandType = System.Data.CommandType.StoredProcedure;
                cmdBankBalance.Parameters.Add(new SqlParameter("@ValuationDate", valuationDate));
                cmdBankBalance.Parameters.Add(new SqlParameter("@Account", userToken.Account));

                //var balanceParam = new SqlParameter("@balance", System.Data.SqlDbType.Float);
                //balanceParam.Direction = System.Data.ParameterDirection.Output;
                //cmdBankBalance.Parameters.Add(balanceParam);
                //cmdBankBalance.ExecuteNonQuery();

                //cashData.BankBalance = balanceParam.Value is double ? (double)balanceParam.Value : 0d;
                var oBalance = cmdBankBalance.ExecuteScalar();
                if(oBalance is double)
                {
                    cashData.BankBalance = (double)cmdBankBalance.ExecuteScalar();
                }

                using (SqlCommand cmdDividends = new SqlCommand("sp_GetDividends", Connection))
                {
                    cmdDividends.CommandType = System.Data.CommandType.StoredProcedure;
                    cmdDividends.Parameters.Add(new SqlParameter("@ValuationDate", valuationDate));
                    cmdDividends.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                    using (SqlDataReader reader = cmdDividends.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cashData.Dividends.Add((string)reader["Company"], (double)reader["Dividend"]);
                        }
                    }
                }
            }
            return cashData;
        }
    }
}
