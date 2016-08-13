using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace InvestmentBuilderCore
{
    public interface IDataLayer
    {
        IClientDataInterface ClientData { get; }
        IInvestmentRecordInterface InvestmentRecordData { get; }
        ICashAccountInterface CashAccountData { get; }
        IUserAccountInterface UserAccountData { get; }
        IHistoricalDataReader HistoricalData { get; }

        void ConnectNewDatasource(string datasource);
    }

    /// <summary>
    /// client interface to datalayer contains client specific methods
    /// </summary>
    public interface IClientDataInterface
    {
        //client interface
        IEnumerable<DateTime> GetRecentValuationDates(UserAccountToken userToken, DateTime dtDateFrom);
        IEnumerable<string> GetTransactionTypes(string side);
        IEnumerable<string> GetActiveCompanies(UserAccountToken userToken, DateTime valuationDate);
        IEnumerable<string> GetAccountMembers(UserAccountToken userToken, DateTime valuationDate);
        IEnumerable<KeyValuePair<string, AuthorizationLevel>> GetAccountMemberDetails(UserAccountToken userToken, DateTime valuationDate);
        DateTime? GetLatestValuationDate(UserAccountToken userToken);
        DateTime? GetPreviousAccountValuationDate(UserAccountToken userToken, DateTime dtValuation);
        IEnumerable<string> GetAccountNames(string user);
        bool IsExistingValuationDate(UserAccountToken userToken, DateTime valuationDate);
        void UpdateMemberForAccount(UserAccountToken userToken, string member, AuthorizationLevel level, bool add);
        void CreateAccount(UserAccountToken userToken, AccountModel account);
        AccountModel GetAccount(UserAccountToken userToken);
        IEnumerable<string> GetAccountTypes();
        IEnumerable<string> GetAllCompanies();
        Stock GetTradeItem(UserAccountToken userToken, string name);
        void UndoLastTransaction(UserAccountToken userToken);
    }

    public interface IInvestmentRecordInterface
    {
        //investment record interface
        //roll (copy) an investment from dtPrevious to dtValuation
        void RollInvestment(UserAccountToken userToken, string investment, DateTime dtValuation, DateTime dtPreviousValaution);
        void UpdateInvestmentQuantity(UserAccountToken userToken, string investment, DateTime dtValuation, int quantity);
        void AddNewShares(UserAccountToken userToken, string investment, int quantity, DateTime dtValaution, double dTotalCost);
        void SellShares(UserAccountToken userToken, string investment, int quantity, DateTime dtValuation);
        void UpdateClosingPrice(UserAccountToken userToken, string investment, DateTime dtValuation, double price);
        void UpdateDividend(UserAccountToken userToken, string investment, DateTime dtValuation, double dividend);
        InvestmentInformation GetInvestmentDetails(string investment);
        IEnumerable<KeyValuePair<string, double>> GetInvestments(UserAccountToken userToken, DateTime dtValuation);
        void CreateNewInvestment(UserAccountToken userToken, string investment, string symbol, string currency,
                                 int quantity, double scalingFactor, double totalCost, double price,
                                 string exchange, DateTime dtValuation);
        IEnumerable<CompanyData> GetInvestmentRecordData(UserAccountToken userToken, DateTime dtValuation);
        void DeactivateInvestment(UserAccountToken userToken, string investment);
        DateTime? GetLatestRecordInvestmentValuationDate(UserAccountToken userToken);
        DateTime? GetPreviousRecordInvestmentValuationDate(UserAccountToken userToken, DateTime dtValuation);
        void AddTradeTransactions(IEnumerable<Stock> trades, TradeType action, UserAccountToken userToken, DateTime dtValuation);
        Trades GetHistoricalTransactions(DateTime dtFrom, DateTime dtTo, UserAccountToken userToken);
        //IEnumerable<CompanyData> GetCompanyRecordData(UserAccountToken userToken, string company);
        //returns the full investment record data set for this account
        IEnumerable<CompanyData> GetFullInvestmentRecordData(UserAccountToken userToken);
        bool IsExistingRecordValuationDate(UserAccountToken userToken, DateTime dtValuation);
    }

    public interface ICashAccountInterface
    {
        CashAccountData GetCashAccountData(UserAccountToken userToken, DateTime valuationDate);
        void AddCashAccountTransaction(UserAccountToken userToken, DateTime valuationDate, DateTime transactionDate,
                                string type, string parameter, double amount);

        void RemoveCashAccountTransaction(UserAccountToken userToken, DateTime valuationDate, DateTime transactionDate,
                        string type, string parameter);
        void GetCashAccountTransactions(UserAccountToken userToken, string side, DateTime valuationDate, Action<System.Data.IDataReader> fnAddTransaction);
        double GetBalanceInHand(UserAccountToken userToken, DateTime valuationDate);
    }

    public interface IUserAccountInterface
    {
       //user account interface
        void RollbackValuationDate(UserAccountToken userToken, DateTime dtValuation);
        void UpdateMemberAccount(UserAccountToken userToken, DateTime dtValuation, string member, double dAmount);
        double GetMemberSubscription(UserAccountToken userToken, DateTime dtValuation, string member);
        IEnumerable<KeyValuePair<string, double>> GetMemberAccountData(UserAccountToken userToken, DateTime dtValuation);
        double GetPreviousUnitValuation(UserAccountToken userToken, DateTime? previousDate);
        void SaveNewUnitValue(UserAccountToken userToken, DateTime dtValuation, double dUnitValue);
        double GetIssuedUnits(UserAccountToken userToken, DateTime dtValuation);
        UserAccountData GetUserAccountData(UserAccountToken userToken);
        double GetStartOfYearValuation(UserAccountToken userToken, DateTime valuationDate);
        IEnumerable<Redemption> GetRedemptions(UserAccountToken userToken, DateTime valuationDate);
        void AddRedemption(UserAccountToken userToken, string user, DateTime transactionDate, double amount);
        void UpdateRedemption(UserAccountToken userToken, string user, DateTime transactionDate, double amount, double units);
    }

    public interface IHistoricalDataReader
    {
        IEnumerable<HistoricalData> GetHistoricalAccountData(UserAccountToken userToken);
    }
}
