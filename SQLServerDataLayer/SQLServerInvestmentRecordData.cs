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
    public class SQLServerInvestmentRecordData : SQLServerBase, IInvestmentRecordInterface
    {
        public SQLServerInvestmentRecordData(string connectionStr)
        {
            ConnectionStr = connectionStr;
        }

        public void RollInvestment(UserAccountToken userToken, string investment, DateTime dtValuation, DateTime dtPreviousValaution)
        {
            userToken.AuthorizeUser(AuthorizationLevel.UPDATE);
            using (var connection = OpenConnection())
            {
                using (var command = new SqlCommand("sp_RollInvestment", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@valuationDate", dtValuation));
                    command.Parameters.Add(new SqlParameter("@previousDate", dtPreviousValaution));
                    command.Parameters.Add(new SqlParameter("@company", investment));
                    command.Parameters.Add(new SqlParameter("@account", userToken.Account));
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateInvestmentQuantity(UserAccountToken userToken, string investment, DateTime dtValuation, int quantity)
        {
            userToken.AuthorizeUser(AuthorizationLevel.UPDATE);
            using (var connection = OpenConnection())
            {
                using (var command = new SqlCommand("sp_UpdateHolding", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@holding", quantity));
                    command.Parameters.Add(new SqlParameter("@valuationDate", dtValuation));
                    command.Parameters.Add(new SqlParameter("@company", investment));
                    command.Parameters.Add(new SqlParameter("@account", userToken.Account));
                    command.ExecuteNonQuery();
                }
            }
        }

        public void AddNewShares(UserAccountToken userToken, string investment, int quantity, DateTime dtValaution, double dTotalCost)
        {
            userToken.AuthorizeUser(AuthorizationLevel.UPDATE);
            using (var connection = OpenConnection())
            {
                using (var command = new SqlCommand("sp_AddNewShares", connection))
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
        }

        public void SellShares(UserAccountToken userToken, string investment, int quantity, DateTime dtValuation)
        {
            userToken.AuthorizeUser(AuthorizationLevel.UPDATE);
            using (var connection = OpenConnection())
            {
                using (var command = new SqlCommand("sp_SellShares", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@valuationDate", dtValuation));
                    command.Parameters.Add(new SqlParameter("@company", investment));
                    command.Parameters.Add(new SqlParameter("@shares", quantity));
                    command.Parameters.Add(new SqlParameter("@account", userToken.Account));
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateClosingPrice(UserAccountToken userToken, string investment, DateTime dtValuation, double price)
        {
            userToken.AuthorizeUser(AuthorizationLevel.UPDATE);
            using (var connection = OpenConnection())
            {
                using (var updateCommand = new SqlCommand("sp_UpdateClosingPrice", connection))
                {
                    updateCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    updateCommand.Parameters.Add(new SqlParameter("@valuationDate", dtValuation));
                    updateCommand.Parameters.Add(new SqlParameter("@investment", investment));
                    updateCommand.Parameters.Add(new SqlParameter("@closingPrice", price));
                    updateCommand.Parameters.Add(new SqlParameter("@account", userToken.Account));
                    updateCommand.ExecuteNonQuery();
                }
            }
        }

        public void UpdateDividend(UserAccountToken userToken, string investment, DateTime dtValuation, double dividend)
        {
            userToken.AuthorizeUser(AuthorizationLevel.UPDATE);
            using (var connection = OpenConnection())
            {
                using (var updateCommand = new SqlCommand("sp_UpdateDividend", connection))
                {
                    updateCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    updateCommand.Parameters.Add(new SqlParameter("@valuationDate", dtValuation));
                    updateCommand.Parameters.Add(new SqlParameter("@company", investment));
                    updateCommand.Parameters.Add(new SqlParameter("@dividend", dividend));
                    updateCommand.Parameters.Add(new SqlParameter("@account", userToken.Account));
                    updateCommand.ExecuteNonQuery();
                }
            }
        }

        public InvestmentInformation GetInvestmentDetails(string investment)
        {
            InvestmentInformation data = null;
            using (var connection = OpenConnection())
            {
                using (var command = new SqlCommand("sp_GetCompanyData", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Name", investment));
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var symbol = GetDBValue<string>("Symbol", reader, string.Empty);
                            var ccy = GetDBValue<string>("Currency", reader, string.Empty);
                            var exchange = GetDBValue<string>("Exchange", reader, string.Empty);
                            data = new InvestmentInformation(
                                symbol.Trim(),
                                exchange.Trim(),
                                ccy.Trim(),
                                GetDBValue<double>("ScalingFactor", reader)
                            );
                        }
                        reader.Close();
                    }
                }
            }
            return data;
        }

        public IEnumerable<KeyValuePair<string, double>> GetInvestments(UserAccountToken userToken, DateTime dtValuation)
        {
            userToken.AuthorizeUser(AuthorizationLevel.UPDATE);
            using (var connection = OpenConnection())
            {
                using (var command = new SqlCommand("sp_GetUserCompanies", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@ValuationDate", dtValuation));
                    command.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        yield return new KeyValuePair<string, double>(
                                                                GetDBValue<string>("Name", reader),
                                                                GetDBValue<double>("Price", reader));
                    }
                    reader.Close();
                }
            }
        }

        public void CreateNewInvestment(UserAccountToken userToken, string investment, string symbol, string currency, int quantity, double scalingFactor, double totalCost, double price, string exchange, DateTime dtValuation)
        {
            userToken.AuthorizeUser(AuthorizationLevel.UPDATE);
            using (var connection = OpenConnection())
            {
                using (var command = new SqlCommand("sp_CreateNewInvestment", connection))
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
        }

        public IEnumerable<CompanyData> GetInvestmentRecordData(UserAccountToken userToken, DateTime dtValuation)
        {
            userToken.AuthorizeUser(AuthorizationLevel.READ);
            using (var connection = OpenConnection())
            {
                using (var command = new SqlCommand("sp_GetLatestInvestmentRecords", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@ValuationDate", dtValuation));
                    command.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        double dTotalCost = GetDBValue<double>("TotalCost", reader);
                        int dSharesHeld = GetDBValue<int>("Bought", reader) + GetDBValue<int>("Bonus", reader) - GetDBValue<int>("Sold", reader);
                        double dAveragePrice = dTotalCost / dSharesHeld;
                        double dSharePrice = GetDBValue<double>("Price", reader);
                        double dDividend = GetDBValue<double>("Dividends", reader);

                        yield return new CompanyData
                        {
                            Name = GetDBValue<string>("Name", reader),
                            ValuationDate = dtValuation,
                            LastBrought = GetDBValue<DateTime>("LastBoughtDate", reader),
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
        }

        public void DeactivateInvestment(UserAccountToken userToken, string investment)
        {
            userToken.AuthorizeUser(AuthorizationLevel.UPDATE);
            using (var connection = OpenConnection())
            {
                using (var command = new SqlCommand("sp_DeactivateCompany", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Name", investment));
                    command.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                    command.ExecuteNonQuery();
                }
            }
        }

        public DateTime? GetLatestRecordInvestmentValuationDate(UserAccountToken userToken)
        {
            userToken.AuthorizeUser(AuthorizationLevel.READ);
            using (var connection = OpenConnection())
            {
                using (var command = new SqlCommand("sp_GetLatestRecordValuationDate", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                    var objResult = command.ExecuteScalar();
                    return objResult is DateTime ? (DateTime?)objResult : null;
                }
            }
        }

        public DateTime? GetPreviousRecordInvestmentValuationDate(UserAccountToken userToken, DateTime dtValuation)
        {
            userToken.AuthorizeUser(AuthorizationLevel.READ);
            using (var connection = OpenConnection())
            {
                using (var command = new SqlCommand("sp_GetPreviousRecordValuationDate", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                    command.Parameters.Add(new SqlParameter("@ValuationDate", dtValuation));
                    var objResult = command.ExecuteScalar();
                    return objResult is DateTime ? (DateTime?)objResult : null;
                }
            }
        }

        public void AddTradeTransactions(IEnumerable<Stock> trades, TradeType action, UserAccountToken userToken, DateTime dtValuation)
        {
            /*
            CREATE PROCEDURE sp_AddTransactionHistory(@transactionDate AS datetime, @company as varchar(50), 
										  @action as varchar(10), @quantity as int,
										  @total_cost as float, @account as varchar(30), @user as varchar(50)) AS

             */
            if (trades == null)
                return;

            userToken.AuthorizeUser(AuthorizationLevel.UPDATE);
            using (var connection = OpenConnection())
            {
                foreach (var trade in trades)
                {
                    var dtTransaction = trade.TransactionDate ?? DateTime.Today;
                    using (var command = new SqlCommand("sp_AddTransactionHistory", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@valuationDate", dtValuation));
                        command.Parameters.Add(new SqlParameter("@transactionDate", dtTransaction));
                        command.Parameters.Add(new SqlParameter("@company", trade.Name));
                        command.Parameters.Add(new SqlParameter("@action", action.ToString()));
                        command.Parameters.Add(new SqlParameter("@quantity", trade.Quantity));
                        command.Parameters.Add(new SqlParameter("@total_cost", trade.TotalCost));
                        command.Parameters.Add(new SqlParameter("@account", userToken.Account));
                        command.Parameters.Add(new SqlParameter("@user", userToken.User));
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public Trades GetHistoricalTransactions(DateTime dtFrom, DateTime dtTo, UserAccountToken userToken)
        {
            //CREATE PROCEDURE sp_GetTransactionHistory(@dateFrom AS datetime, @dateTo AS datetime, @account as varchar(30)) AS
            //BEGIN
            // C.[Name], T.[trade_action], T.[quantity], T.[total_cost]
            List<Stock> buys = new List<Stock>();
            List<Stock> sells = new List<Stock>();
            List<Stock> changed = new List<Stock>();

            userToken.AuthorizeUser(AuthorizationLevel.READ);
            using (var connection = OpenConnection())
            {
                using (var command = new SqlCommand("sp_GetTransactionHistory", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@dateFrom", dtFrom));
                    command.Parameters.Add(new SqlParameter("@dateTo", dtTo));
                    command.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var action = (TradeType)Enum.Parse(typeof(TradeType), (string)reader["trade_action"]);
                        var trade = new Stock
                        {
                            Name = GetDBValue<string>("Name", reader),
                            Quantity = GetDBValue<int>("quantity", reader),
                            TotalCost = GetDBValue<double>("total_cost", reader)
                        };
                        switch (action)
                        {
                            case TradeType.BUY:
                                buys.Add(trade);
                                break;
                            case TradeType.SELL:
                                sells.Add(trade);
                                break;
                            case TradeType.MODIFY:
                                changed.Add(trade);
                                break;
                        }
                    }
                    reader.Close();
                }
            }
            return new Trades
            {
                Buys = buys.ToArray(),
                Sells = sells.ToArray(),
                Changed = changed.ToArray()
            };

        }

        public IEnumerable<CompanyData> GetFullInvestmentRecordData(UserAccountToken userToken)
        {
            userToken.AuthorizeUser(AuthorizationLevel.READ);
            using (var connection = OpenConnection())
            {
                using (var command = new SqlCommand("sp_GetFullInvestmentRecordData", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        double dTotalCost = GetDBValue<double>("TotalCost", reader);
                        int dSharesHeld = GetDBValue<int>("Bought", reader) + GetDBValue<int>("Bonus", reader) - GetDBValue<int>("Sold", reader);
                        double dAveragePrice = dTotalCost / dSharesHeld;
                        double dSharePrice = GetDBValue<double>("Price", reader);
                        double dDividend = GetDBValue<double>("Dividends", reader);

                        yield return new CompanyData
                        {
                            Name = GetDBValue<string>("Name", reader),
                            ValuationDate = GetDBValue<DateTime>("ValuationDate", reader),
                            LastBrought = GetDBValue<DateTime>("LastBoughtDate", reader),
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
        }

        public bool IsExistingRecordValuationDate(UserAccountToken userToken, DateTime dtValuation)
        {
            userToken.AuthorizeUser(AuthorizationLevel.READ);
            using (var connection = OpenConnection())
            {
                using (var sqlCommand = new SqlCommand("sp_IsExistingRecordValuationDate", connection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@ValuationDate", dtValuation));
                    sqlCommand.Parameters.Add(new SqlParameter("@Account", userToken.Account));
                    var result = sqlCommand.ExecuteScalar();
                    return result != null;
                }
            }
        }
    }
}
