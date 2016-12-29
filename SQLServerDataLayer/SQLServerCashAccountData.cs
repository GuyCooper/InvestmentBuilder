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
                cmdBankBalance.Parameters.Add(new SqlParameter("@ValuationDate", valuationDate.Date));
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
                            var company = GetDBValue<string>("Company", reader);
                            var dividend = GetDBValue<double>("Dividend", reader);
                            if(cashData.Dividends.ContainsKey(company) == true)
                            {
                                cashData.Dividends[company] += dividend; 
                            }
                            else {
                                cashData.Dividends.Add(company, dividend );
                            }
                            
                        }
                    }
                }
            }
            return cashData;
        }

        public void RemoveCashAccountTransaction(UserAccountToken userToken, DateTime valuationDate, DateTime transactionDate,
                    string type, string parameter)
        {
            userToken.AuthorizeUser(AuthorizationLevel.UPDATE);
            using (var sqlCommand = new SqlCommand("sp_RemoveCashAccountData", Connection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@ValuationDate", valuationDate.Date));
                sqlCommand.Parameters.Add(new SqlParameter("@TransactionDate", transactionDate));
                sqlCommand.Parameters.Add(new SqlParameter("@TransactionType", type));
                sqlCommand.Parameters.Add(new SqlParameter("@Parameter", parameter));
                sqlCommand.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                sqlCommand.ExecuteNonQuery();
            }
 
        }

        public void GetCashAccountTransactions(UserAccountToken userToken, string side, DateTime valuationDate, Action<System.Data.IDataReader> fnAddTransaction)
        {
            userToken.AuthorizeUser(AuthorizationLevel.READ);
            if (fnAddTransaction == null)
            {
                return;
            }

            using (var sqlCommand = new SqlCommand("sp_GetCashAccountData", Connection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@ValuationDate", valuationDate.Date));
                sqlCommand.Parameters.Add(new SqlParameter("@Side", side));
                sqlCommand.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        fnAddTransaction(reader);
                    }
                }
            }
        }

        public double GetBalanceInHand(UserAccountToken userToken, DateTime valuationDate)
        {
            userToken.AuthorizeUser(AuthorizationLevel.READ);
            using (var sqlCommand = new SqlCommand("sp_GetBalanceInHand", Connection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@ValuationDate", valuationDate.Date));
                sqlCommand.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                var result = sqlCommand.ExecuteScalar();
                if (result is double)
                {
                    return (double)result;
                }
            }
            return 0d;

        }

        public void AddCashAccountTransaction(UserAccountToken userToken, DateTime valuationDate, DateTime transactionDate, string type, string parameter, double amount)
        {
            userToken.AuthorizeUser(AuthorizationLevel.UPDATE);
            using (var sqlCommand = new SqlCommand("sp_AddCashAccountData", Connection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@ValuationDate", valuationDate.Date));
                sqlCommand.Parameters.Add(new SqlParameter("@TransactionDate", transactionDate));
                sqlCommand.Parameters.Add(new SqlParameter("@TransactionType", type));
                sqlCommand.Parameters.Add(new SqlParameter("@Parameter", parameter));
                sqlCommand.Parameters.Add(new SqlParameter("@Amount", amount));
                sqlCommand.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                sqlCommand.ExecuteNonQuery();
            }
        }

    }
}
