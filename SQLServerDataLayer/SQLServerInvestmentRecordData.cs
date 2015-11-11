﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderCore;
using System.Data.SqlClient;
using System.Data;

namespace SQLServerDataLayer
{
    public class SQLServerInvestmentRecordData : SQLServerBase, IInvestmentRecordInterface
    {
        public SQLServerInvestmentRecordData(SqlConnection connection)
        {
            Connection = connection;
        }

        public void RollInvestment(UserAccountToken userToken, string investment, DateTime dtValuation, DateTime dtPreviousValaution)
        {
            userToken.AuthorizeUser(AuthorizationLevel.UPDATE);
            using (var command = new SqlCommand("sp_RollInvestment", Connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@valuationDate", dtValuation));
                command.Parameters.Add(new SqlParameter("@previousDate", dtPreviousValaution));
                command.Parameters.Add(new SqlParameter("@company", investment));
                command.Parameters.Add(new SqlParameter("@account", userToken.Account));
                command.ExecuteNonQuery();
            }
        }

        public void UpdateInvestmentQuantity(UserAccountToken userToken, string investment, DateTime dtValuation, int quantity)
        {
            userToken.AuthorizeUser(AuthorizationLevel.UPDATE);
            using (var command = new SqlCommand("sp_UpdateHolding", Connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@holding", quantity));
                command.Parameters.Add(new SqlParameter("@valuationDate", dtValuation));
                command.Parameters.Add(new SqlParameter("@company", investment));
                command.Parameters.Add(new SqlParameter("@account", userToken.Account));
                command.ExecuteNonQuery();
            }
        }

        public void AddNewShares(UserAccountToken userToken, string investment, int quantity, DateTime dtValaution, double dTotalCost)
        {
            userToken.AuthorizeUser(AuthorizationLevel.UPDATE);
            using (var command = new SqlCommand("sp_AddNewShares", Connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@valuationDate", dtValaution));
                command.Parameters.Add(new SqlParameter("@company", investment));
                command.Parameters.Add(new SqlParameter("@shares", quantity));
                command.Parameters.Add(new SqlParameter("@totalCost", dTotalCost));
                command.Parameters.Add(new SqlParameter("@account", userToken.Account));
                command.ExecuteNonQuery();
            }
 
        }

        public void SellShares(UserAccountToken userToken, string investment, int quantity, DateTime dtValuation)
        {
            userToken.AuthorizeUser(AuthorizationLevel.UPDATE);
            using (var command = new SqlCommand("sp_SellShares", Connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@valuationDate", dtValuation));
                command.Parameters.Add(new SqlParameter("@company", investment));
                command.Parameters.Add(new SqlParameter("@shares", quantity));
                command.Parameters.Add(new SqlParameter("@account", userToken.Account));
                command.ExecuteNonQuery();
            }
        }

        public void UpdateClosingPrice(UserAccountToken userToken, string investment, DateTime dtValuation, double price)
        {
            userToken.AuthorizeUser(AuthorizationLevel.UPDATE);
            using (var updateCommand = new SqlCommand("sp_UpdateClosingPrice", Connection))
            {
                updateCommand.CommandType = System.Data.CommandType.StoredProcedure;
                updateCommand.Parameters.Add(new SqlParameter("@valuationDate", dtValuation));
                updateCommand.Parameters.Add(new SqlParameter("@investment", investment));
                updateCommand.Parameters.Add(new SqlParameter("@closingPrice", price));
                updateCommand.Parameters.Add(new SqlParameter("@account", userToken.Account));
                updateCommand.ExecuteNonQuery();
            }
        }

        public void UpdateDividend(UserAccountToken userToken, string investment, DateTime dtValuation, double dividend)
        {
            userToken.AuthorizeUser(AuthorizationLevel.UPDATE);
            using (var updateCommand = new SqlCommand("sp_UpdateDividend", Connection))
            {
                updateCommand.CommandType = System.Data.CommandType.StoredProcedure;
                updateCommand.Parameters.Add(new SqlParameter("@valuationDate", dtValuation));
                updateCommand.Parameters.Add(new SqlParameter("@company", investment));
                updateCommand.Parameters.Add(new SqlParameter("@dividend", dividend));
                updateCommand.Parameters.Add(new SqlParameter("@account", userToken.Account));
                updateCommand.ExecuteNonQuery();
            }
        }

        public InvestmentInformation GetInvestmentDetails(string investment)
        {
            InvestmentInformation data = null;
            using (var command = new SqlCommand("sp_GetCompanyData", Connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@Name", investment));
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var symbol = (string)reader["Symbol"];
                        var ccy = (string)reader["Currency"];
                        var exchange = reader["Exchange"] as string;
                        data = new InvestmentInformation
                        {
                            Symbol = symbol.Trim(),
                            Exchange = exchange != null ? exchange.Trim() : string.Empty,
                            Currency = ccy.Trim(),
                            ScalingFactor = (double)reader["ScalingFactor"]
                        };
                    }
                    reader.Close();
                }
            }
            return data;
        }

        public IEnumerable<KeyValuePair<string, double>> GetInvestments(UserAccountToken userToken, DateTime dtValuation)
        {
            userToken.AuthorizeUser(AuthorizationLevel.UPDATE);
            using (var command = new SqlCommand("sp_GetUserCompanies", Connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@ValuationDate", dtValuation));
                command.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    yield return new KeyValuePair<string,double>((string)reader["Name"], (double)reader["Price"]);
                }
                reader.Close();
            }
        }

        public void CreateNewInvestment(UserAccountToken userToken, string investment, string symbol, string currency, int quantity, double scalingFactor, double totalCost, double price, string exchange, DateTime dtValuation)
        {
            userToken.AuthorizeUser(AuthorizationLevel.UPDATE);
            using (var command = new SqlCommand("sp_CreateNewInvestment", Connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@valuationDate", dtValuation));
                command.Parameters.Add(new SqlParameter("@investment", investment));
                command.Parameters.Add(new SqlParameter("@symbol", symbol));
                command.Parameters.Add(new SqlParameter("@currency", currency));
                command.Parameters.Add(new SqlParameter("@scalingFactor", scalingFactor));
                command.Parameters.Add(new SqlParameter("@shares", quantity));
                command.Parameters.Add(new SqlParameter("@totalCost", totalCost));
                command.Parameters.Add(new SqlParameter("@closingPrice", price));
                command.Parameters.Add(new SqlParameter("@account", userToken.Account));
                command.Parameters.Add(new SqlParameter("@exchange", exchange ?? string.Empty));

                command.ExecuteNonQuery();
            }
        }

        public IEnumerable<CompanyData> GetInvestmentRecordData(UserAccountToken userToken, DateTime dtValuation)
        {
            userToken.AuthorizeUser(AuthorizationLevel.READ);
            using (var command = new SqlCommand("sp_GetLatestInvestmentRecords", Connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@ValuationDate", dtValuation));
                command.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    double dTotalCost = (double)reader["TotalCost"];
                    int dSharesHeld = (int)reader["Bought"] + (int)reader["Bonus"] - (int)reader["Sold"];
                    double dAveragePrice = dTotalCost / dSharesHeld;
                    double dSharePrice = (double)reader["Price"];
                    double dDividend = (double)reader["Dividends"];

                    yield return new CompanyData
                    {
                        Name = (string)reader["Name"],
                        LastBrought = (DateTime)reader["LastBoughtDate"],
                        Quantity = dSharesHeld,
                        AveragePricePaid = dAveragePrice,
                        TotalCost = dTotalCost,
                        SharePrice = dSharePrice,
                        //dNetSellingValue = _GetNetSellingValue(dSharesHeld, dSharePrice),
                        Dividend = dDividend
                    };
                }
                reader.Close();
            }
        }

        public void DeactivateInvestment(UserAccountToken userToken, string investment)
        {
            userToken.AuthorizeUser(AuthorizationLevel.UPDATE);
            using (var command = new SqlCommand("sp_DeactivateCompany", Connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@Name", investment));
                command.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                command.ExecuteNonQuery();
            }
        }

        public DateTime? GetLatestRecordInvestmentValuationDate(UserAccountToken userToken)
        {
            userToken.AuthorizeUser(AuthorizationLevel.READ);
            using (var command = new SqlCommand("sp_GetLatestRecordValuationDate", Connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                var objResult = command.ExecuteScalar();
                return objResult is DateTime ? (DateTime?)objResult : null;
            }
        }

        public DateTime? GetPreviousRecordInvestmentValuationDate(UserAccountToken userToken, DateTime dtValuation)
        {
            userToken.AuthorizeUser(AuthorizationLevel.READ);
            using (var command = new SqlCommand("sp_GetPreviousRecordValuationDate", Connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                command.Parameters.Add(new SqlParameter("@ValuationDate", dtValuation));
                var objResult = command.ExecuteScalar();
                return objResult is DateTime ? (DateTime?)objResult : null;
            }
        }

    }
}
