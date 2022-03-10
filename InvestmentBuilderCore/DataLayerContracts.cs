using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;
using System.Data;

namespace InvestmentBuilderCore
{
    [ContractClassFor(typeof(IClientDataInterface))]
    internal abstract class IClientDataContract : IClientDataInterface
    {
        public IEnumerable<DateTime> GetRecentValuationDates(UserAccountToken userToken, DateTime dtDateFrom)
        {
            Contract.Requires(userToken != null);
            Contract.Ensures(Contract.Result< IEnumerable<DateTime>>() != null);
            return null;
        }

        public IEnumerable<string> GetTransactionTypes(string side)
        {
            Contract.Ensures(Contract.Result<IEnumerable<string>>() != null);
            return null;
        }

        public DateTime? GetLatestValuationDate(UserAccountToken userToken)
        {
            Contract.Requires(userToken != null);
            return null;
        }

        public DateTime? GetPreviousAccountValuationDate(UserAccountToken userToken, DateTime dtValuation)
        {
            Contract.Requires(userToken != null);
            return null;
        }

        public bool IsExistingValuationDate(UserAccountToken userToken, DateTime valuationDate)
        {
            Contract.Requires(userToken != null);
            return false;
        }

        public IEnumerable<string> GetAccountTypes()
        {
            Contract.Requires(Contract.Result<IEnumerable<string>>() != null);
            return null;
        }

        public IEnumerable<string> GetAllCompanies()
        {
            Contract.Requires(Contract.Result<IEnumerable<string>>() != null);
            return null;
        }

        public Stock GetTradeItem(UserAccountToken userToken, string name)
        {
            Contract.Requires(userToken != null);
            Contract.Requires(string.IsNullOrEmpty(name) == false);
            Contract.Ensures(Contract.Result<Stock>() != null);
            return null;
        }

        public int UndoLastTransaction(UserAccountToken userToken, DateTime fromValuationDate)
        {
            Contract.Requires(userToken != null);
            return 0;
        }

        public Transaction GetLastTransaction(UserAccountToken userToken, DateTime fromValuationDate)
        {
            Contract.Requires(userToken != null);
            return null;
        }

        public IEnumerable<ValuationData> GetAllValuations(UserAccountToken userToken)
        {
            Contract.Requires(userToken != null);
            return null;
        }
    }

    [ContractClassFor(typeof(IInvestmentRecordInterface))]
    internal abstract class IInvestmentRecordContract : IInvestmentRecordInterface
    {
        public void AddNewShares(UserAccountToken userToken, string investment, double quantity, DateTime dtValaution, double dTotalCost)
        {
            Contract.Requires(userToken != null);
            Contract.Requires(string.IsNullOrEmpty(investment) == false);
            Contract.Requires(quantity > 0);
            Contract.Requires(dTotalCost > 0 && dTotalCost < 100000.0);
        }

        public void AddTradeTransactions(IEnumerable<Stock> trades, TradeType action, UserAccountToken userToken, DateTime dtValuation)
        {
            Contract.Requires(userToken != null);
            Contract.Requires(trades != null);
        }

        public void CreateNewInvestment(UserAccountToken userToken, string investment, string symbol, string currency, double quantity, double scalingFactor, double totalCost, double price, string exchange, DateTime dtValuation)
        {
            Contract.Requires(userToken != null);
            Contract.Requires(string.IsNullOrEmpty(investment) == false);
            Contract.Requires(string.IsNullOrEmpty(symbol) == false);
            Contract.Requires(string.IsNullOrEmpty(currency) == false);
            Contract.Requires(quantity > 0);
            Contract.Requires(totalCost > 0 && totalCost < 100000);
            Contract.Requires(price > 0 && price < 100000);
        }

        public void DeactivateInvestment(UserAccountToken userToken, string investment)
        {
            Contract.Requires(userToken != null);
            Contract.Requires(string.IsNullOrEmpty(investment) == false);
        }

        public IEnumerable<CompanyData> GetFullInvestmentRecordData(UserAccountToken userToken)
        {
            Contract.Requires(userToken != null);
            Contract.Ensures(Contract.Result<IEnumerable<CompanyData>>() != null);
            return null;
        }

        public Trades GetHistoricalTransactions(DateTime dtFrom, DateTime dtTo, UserAccountToken userToken)
        {
            Contract.Requires(userToken != null);
            Contract.Requires(dtTo > dtFrom);
            return null;
        }

        public InvestmentInformation GetInvestmentDetails(string investment)
        {
            Contract.Requires(string.IsNullOrEmpty(investment) == false);
            Contract.Ensures(Contract.Result<InvestmentInformation>() != null);
            return null;
        }

        public IEnumerable<CompanyData> GetInvestmentRecordData(UserAccountToken userToken, DateTime dtValuation)
        {
            Contract.Requires(userToken != null);
            Contract.Ensures(Contract.Result<IEnumerable<CompanyData>>() != null);
            return null;
        }

        public IEnumerable<KeyValuePair<string, double>> GetInvestments(UserAccountToken userToken, DateTime dtValuation)
        {
            Contract.Requires(userToken != null);
            return null;
        }

        public DateTime? GetLatestRecordInvestmentValuationDate(UserAccountToken userToken)
        {
            Contract.Requires(userToken != null);
            return null;
        }

        public DateTime? GetPreviousRecordInvestmentValuationDate(UserAccountToken userToken, DateTime dtValuation)
        {
            Contract.Requires(userToken != null);
            return null;
        }

        public bool IsExistingRecordValuationDate(UserAccountToken userToken, DateTime dtValuation)
        {
            Contract.Requires(userToken != null);
            return false;
        }

        public void RollInvestment(UserAccountToken userToken, string investment, DateTime dtValuation, DateTime dtPreviousValaution)
        {
            Contract.Requires(userToken != null);
            Contract.Requires(dtValuation > dtPreviousValaution);    
        }

        public void SellShares(UserAccountToken userToken, string investment, double quantity, DateTime dtValuation)
        {
            Contract.Requires(userToken != null);
            Contract.Requires(string.IsNullOrEmpty(investment) == false);
            Contract.Requires(quantity > 0);
        }

        public void UpdateClosingPrice(UserAccountToken userToken, string investment, DateTime dtValuation, double price)
        {
            Contract.Requires(userToken != null);
            Contract.Requires(string.IsNullOrEmpty(investment) == false);
            Contract.Requires(price > 0);
        }

        public void UpdateDividend(UserAccountToken userToken, string investment, DateTime dtValuation, double dividend)
        {
            Contract.Requires(userToken != null);
            Contract.Requires(string.IsNullOrEmpty(investment) == false);
            Contract.Requires(dividend > 0);
        }

        public void UpdateInvestmentQuantity(UserAccountToken userToken, string investment, DateTime dtValuation, double quantity)
        {
            Contract.Requires(userToken != null);
            Contract.Requires(string.IsNullOrEmpty(investment) == false);
            Contract.Requires(quantity > 0);
        }
    }

    [ContractClassFor(typeof(ICashAccountInterface))]
    internal abstract class ICashAccountContract : ICashAccountInterface
    {
        public int AddCashAccountTransaction(UserAccountToken userToken, DateTime valuationDate, DateTime transactionDate, string type, string parameter, double amount)
        {
            Contract.Requires(userToken != null);
            Contract.Requires(string.IsNullOrEmpty(type) == false);
            Contract.Requires(string.IsNullOrEmpty(parameter) == false);
            Contract.Requires(amount > 0);
            return 0;
        }

        public double GetBalanceInHand(UserAccountToken userToken, DateTime valuationDate)
        {
            Contract.Requires(userToken != null);
            Contract.Ensures(Contract.Result<double>() >= 0);
            return 0;
        }

        public CashAccountData GetCashBalances(UserAccountToken userToken, DateTime valuationDate)
        {
            Contract.Requires(userToken != null);
            Contract.Ensures(Contract.Result<CashAccountData>() != null);
            return null;
        }

        public void GetCashAccountData(UserAccountToken userToken, string side, DateTime valuationDate, Action<IDataReader> fnAddTransaction)
        {
            Contract.Requires(userToken != null);
            Contract.Requires(string.IsNullOrEmpty(side) == false);
        }

        public void RemoveCashAccountTransaction(UserAccountToken userToken, int transactionID)
        {
            Contract.Requires(userToken != null);
        }

        public IEnumerable<Tuple<DateTime, double>> GetCashTransactions(UserAccountToken userToken, string transactionType)
        {
            Contract.Requires(userToken != null);
            Contract.Requires(string.IsNullOrEmpty(transactionType) == false);
            return null;
        }
    }

    [ContractClassFor(typeof(IUserAccountInterface))]
    internal abstract class IUserAccountContract : IUserAccountInterface
    {
        public void AddRedemption(UserAccountToken userToken, string user, DateTime transactionDate, double amount)
        {
            Contract.Requires(userToken != null);
            Contract.Requires(string.IsNullOrEmpty(user) == false);
            Contract.Requires(amount > 0);
        }

        public double GetIssuedUnits(UserAccountToken userToken, DateTime dtValuation)
        {
            Contract.Requires(userToken != null);
            Contract.Ensures(Contract.Result<double>() >= 0);
            return 0;
        }

        public IEnumerable<KeyValuePair<string, double>> GetMemberAccountData(UserAccountToken userToken, DateTime dtValuation)
        {
            Contract.Requires(userToken != null);
            return null;
        }

        public double GetMemberSubscription(UserAccountToken userToken, DateTime dtValuation, string member)
        {
            Contract.Requires(userToken != null);
            Contract.Requires(string.IsNullOrEmpty(member) == false);
            Contract.Requires(Contract.Result<double>() >= 0);
            return 0;
        }

        public double GetPreviousUnitValuation(UserAccountToken userToken, DateTime? previousDate)
        {
            Contract.Requires(userToken != null);
            Contract.Requires(Contract.Result<double>() >= 0);
            return 0;
        }

        public IEnumerable<Redemption> GetRedemptions(UserAccountToken userToken, DateTime valuationDate)
        {
            Contract.Requires(userToken != null);
            return null;
        }

        public double GetStartOfYearValuation(UserAccountToken userToken, DateTime valuationDate)
        {
            Contract.Requires(userToken != null);
            Contract.Requires(Contract.Result<double>() >= 0);
            return 0;
        }

        public AccountModel GetUserAccountData(UserAccountToken userToken)
        {
            Contract.Requires(userToken != null);
            Contract.Ensures(Contract.Result<AccountModel>() != null);
            return null;
        }

        public void RollbackValuationDate(UserAccountToken userToken, DateTime dtValuation)
        {
            Contract.Requires(userToken != null);
        }

        public void SaveNewUnitValue(UserAccountToken userToken, DateTime dtValuation, double dUnitValue)
        {
            Contract.Requires(userToken != null);
            Contract.Requires(dUnitValue >= 0);
        }

        public void UpdateMemberAccount(UserAccountToken userToken, DateTime dtValuation, string member, double dAmount)
        {
            Contract.Requires(userToken != null);
            Contract.Requires(string.IsNullOrEmpty(member) == false);
            Contract.Requires(dAmount > 0);
        }

        public RedemptionStatus UpdateRedemption(UserAccountToken userToken, string user, DateTime transactionDate, double amount, double units)
        {
            Contract.Requires(userToken != null);
            Contract.Requires(string.IsNullOrEmpty(user) == false);
            Contract.Requires(amount > 0);
            Contract.Requires(units> 0);
            return RedemptionStatus.Complete;
        }

        public IEnumerable<string> GetAccountMembers(UserAccountToken userToken, DateTime valuationDate)
        {
            Contract.Requires(userToken != null);
            Contract.Ensures(Contract.Result<IEnumerable<string>>() != null);
            return null;
        }

        public IEnumerable<AccountMember> GetAccountMemberDetails(UserAccountToken userToken, DateTime valuationDate)
        {
            Contract.Requires(userToken != null);
            Contract.Ensures(Contract.Result<IEnumerable<AccountMember>>() != null);
            return null;
        }

        public void UpdateMemberForAccount(UserAccountToken userToken, string member, AuthorizationLevel level, bool add)
        {
            Contract.Requires(userToken != null);
            Contract.Requires(string.IsNullOrEmpty(member) == false);
        }

        public void UpdateAccount(UserAccountToken userToken, AccountModel account)
        {
            Contract.Requires(userToken != null);
        }

        public int CreateAccount(UserAccountToken userToken, AccountModel account)
        {
            Contract.Requires(userToken != null);
            return 0;
        }

        public AccountModel GetAccount(UserAccountToken userToken)
        {
            Contract.Requires(userToken != null);
            Contract.Ensures(Contract.Result<AccountModel>() != null);
            return null;
        }

        public IEnumerable<AccountIdentifier> GetAccountNames(string user, bool bCheckAdmin)
        {
            Contract.Requires(string.IsNullOrEmpty(user) == false);
            Contract.Ensures(Contract.Result<IEnumerable<AccountIdentifier>>() != null);
            return null;
        }

        public IEnumerable<string> GetActiveCompanies(UserAccountToken userToken, DateTime valuationDate)
        {
            Contract.Requires(userToken != null);
            Contract.Ensures(Contract.Result<IEnumerable<string>>() != null);
            return null;
        }

        public bool InvestmentAccountExists(AccountIdentifier account)
        {
            Contract.Requires(account != null);
            Contract.Requires(string.IsNullOrEmpty(account.Name) == false);
            return false;
        }

        public IEnumerable<double> GetUnitValuationRange(UserAccountToken userToken, DateTime dateFrom, DateTime dateTo)
        {
            Contract.Requires(userToken != null);
            Contract.Ensures(Contract.Result<IEnumerable<string>>().Count() > 0);
            return null;
        }

        public int GetUserId(string userName)
        {
            Contract.Requires(string.IsNullOrEmpty(userName) == false);
            return 0;
        }
        public void AddUser(string userName, string description)
        {
            Contract.Requires(string.IsNullOrEmpty(userName) == false);
        }

    }

    internal abstract class IHistoricalDataContract : IHistoricalDataReader
    {
        public IEnumerable<HistoricalData> GetHistoricalAccountData(UserAccountToken userToken)
        {
            Contract.Requires(userToken != null);
            Contract.Ensures(Contract.Result<IEnumerable<HistoricalData>>() != null);
            return null;
        }

        public string GetIndexHistoricalData(UserAccountToken userToken, string symbol)
        {
            Contract.Requires(userToken != null);
            Contract.Requires(string.IsNullOrEmpty(symbol) == false);
            Contract.Ensures(Contract.Result<string>() != null);
            return null;
        }

        public Dictionary<string, List<Tuple<int, double>>> GetHistoricalYieldData(UserAccountToken userToken)
        {
            return null;
        }
    }
}
