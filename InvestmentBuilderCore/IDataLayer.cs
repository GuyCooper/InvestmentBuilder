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

    public interface IClientDataInterface
    {
        //client interface
        IEnumerable<DateTime> GetRecentValuationDates(string account);
        IEnumerable<string> GetTransactionTypes(string side);
        IEnumerable<string> GetActiveCompanies(string account, DateTime valuationDate);
        IEnumerable<string> GetAccountMembers(string account, DateTime valuationDate);
        void GetCashAccountData(string account, string side, DateTime valuationDate, Action<IDataReader> fnAddTransaction);
        DateTime? GetLatestValuationDate(string account);
        double GetBalanceInHand(string account, DateTime valuationDate);
        void AddCashAccountData(string account, DateTime valuationDate, DateTime transactionDate,
                                string type, string parameter, double amount);
        IEnumerable<string> GetAccountNames();
        bool IsExistingValuationDate(string account, DateTime valuationDate);
        void UpdateMemberForAccount(string account, string member, bool add);
        void CreateAccount(AccountModel account);
        AccountModel GetAccount(string account);
        IEnumerable<string> GetAccountTypes();
        IEnumerable<string> GetAllCompanies();
    }

    public interface IInvestmentRecordInterface
    {
        //investment record interface
        void RollInvestment(string account, string investment, DateTime dtValuation, DateTime dtPreviousValaution);
        void UpdateInvestmentQuantity(string account, string investment, DateTime dtValuation, int quantity);
        void AddNewShares(string account, string investment, int quantity, DateTime dtValaution, double dTotalCost);
        void SellShares(string account, string investment, int quantity, DateTime dtValuation);
        void UpdateClosingPrice(string account, string investment, DateTime dtValuation, double price);
        void UpdateDividend(string account, string investment, DateTime dtValuation, double dividend);
        InvestmentInformation GetInvestmentDetails(string investment);
        IEnumerable<string> GetInvestments(string account, DateTime dtValuation);
        void CreateNewInvestment(string account, string investment, string symbol, string currency,
                                 int quantity, double scalingFactor, double totalCost, double price,
                                 string exchange, DateTime dtValuation);
        IEnumerable<CompanyData> GetInvestmentRecordData(string account, DateTime dtValuation);
        void DeactivateInvestment(string account, string investment);
        DateTime? GetLatestRecordInvestmentValuationDate(string account);
        DateTime? GetPreviousRecordInvestmentValuationDate(string account, DateTime dtValuation);
    }

    public interface ICashAccountInterface
    {
        CashAccountData GetCashAccountData(string account, DateTime valuationDate);
    }

    public interface IUserAccountInterface
    {
       //user account interface
        void RollbackValuationDate(string account, DateTime dtValuation);
        void UpdateMemberAccount(string account, DateTime dtValuation, string member, double dAmount);
        double GetMemberSubscription(string account, DateTime dtValuation, string member);
        IEnumerable<KeyValuePair<string, double>> GetMemberAccountData(string account, DateTime dtValuation);
        double GetPreviousUnitValuation(string account, DateTime dtValuation, DateTime? previousDate);
        void SaveNewUnitValue(string account, DateTime dtValuation, double dUnitValue);
        double GetIssuedUnits(string account, DateTime dtValuation);
        DateTime? GetPreviousAccountValuationDate(string account, DateTime dtValuation);
        IEnumerable<string> GetAccountMembers(string account);
        UserAccountData GetUserAccountData(string account);
    }

    public interface IHistoricalDataReader
    {
        IEnumerable<HistoricalData> GetHistoricalAccountData(string account);
    }
}
