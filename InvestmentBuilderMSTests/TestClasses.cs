using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderCore;
using MarketDataServices;
using InvestmentBuilderCore.Schedule;

namespace InvestmentBuilderMSTests
{
    //not implemented test instance of IDataLayer interface 
 #region NotImplementedDataLayer classes
    internal class DataLayerTest : IDataLayer
    {
        private IClientDataInterface _clientData;
        private IInvestmentRecordInterface _investmentRecordData;
        private ICashAccountInterface _cashAccountData;
        private IUserAccountInterface _userAccountData;
        private IHistoricalDataReader _historicalData;

        public DataLayerTest() { }

        public DataLayerTest(IClientDataInterface clientData,
                            IInvestmentRecordInterface investmentRecordData,
                            ICashAccountInterface cashAccountData,
                            IUserAccountInterface userAccountData,
                            IHistoricalDataReader historicalData)

        {
            _clientData = clientData;
            _investmentRecordData = investmentRecordData;
            _cashAccountData = cashAccountData;
            _userAccountData = userAccountData;
            _historicalData = historicalData;
        }

        public IClientDataInterface ClientData { get { return _clientData ?? new ClientDataInterfaceTest(); } }
        public IInvestmentRecordInterface InvestmentRecordData { get { return _investmentRecordData ?? new InvestmentRecordInterfaceTest(); } }
        public ICashAccountInterface CashAccountData { get { return _cashAccountData ?? new CashAccountInterfaceTest(); } }
        public IUserAccountInterface UserAccountData { get { return _userAccountData ?? new UserAccountInterfaceTest(); } }
        public IHistoricalDataReader HistoricalData { get { return _historicalData ?? new HistoricalDataReaderTest(); } }

        public void ConnectNewDatasource(string datasource) { }
    }

    internal class ClientDataInterfaceTest : IClientDataInterface
    {
        //client interface
        public virtual IEnumerable<DateTime> GetRecentValuationDates(UserAccountToken userToken, DateTime dtDateFrom) { throw new NotImplementedException(); }
        public virtual IEnumerable<string> GetTransactionTypes(string side) { throw new NotImplementedException(); }
        public virtual DateTime? GetLatestValuationDate(UserAccountToken userToken) { throw new NotImplementedException(); }
        public virtual DateTime? GetPreviousAccountValuationDate(UserAccountToken userToken, DateTime dtValuation) { throw new NotImplementedException(); }
        public virtual bool IsExistingValuationDate(UserAccountToken userToken, DateTime valuationDate) { throw new NotImplementedException(); }
        public virtual IEnumerable<string> GetAccountTypes() { throw new NotImplementedException(); }
        public virtual IEnumerable<string> GetAllCompanies() { throw new NotImplementedException(); }
        public virtual Stock GetTradeItem(UserAccountToken userToken, string name) { throw new NotImplementedException(); }
        public virtual int UndoLastTransaction(UserAccountToken userToken, DateTime fromValuationDate) { throw new NotImplementedException(); }
        public Transaction GetLastTransaction(UserAccountToken userToken, DateTime fromValuationDate) { throw new NotImplementedException(); }
        public IEnumerable<ValuationData> GetAllValuations(UserAccountToken userToken) { throw new NotImplementedException(); }
    }

    internal class InvestmentRecordInterfaceTest : IInvestmentRecordInterface
    {
        //investment record interface
        //roll (copy) an investment from dtPrevious to dtValuation
        public virtual void RollInvestment(UserAccountToken userToken, string investment, DateTime dtValuation, DateTime dtPreviousValaution) { throw new NotImplementedException(); }
        public virtual void UpdateInvestmentQuantity(UserAccountToken userToken, string investment, DateTime dtValuation, double quantity) { throw new NotImplementedException(); }
        public virtual void AddNewShares(UserAccountToken userToken, string investment, double quantity, DateTime dtValaution, double dTotalCost) { throw new NotImplementedException(); }
        public virtual void SellShares(UserAccountToken userToken, string investment, double quantity, DateTime dtValuation) { throw new NotImplementedException(); }
        public virtual void UpdateClosingPrice(UserAccountToken userToken, string investment, DateTime dtValuation, double price) { throw new NotImplementedException(); }
        public virtual void UpdateDividend(UserAccountToken userToken, string investment, DateTime dtValuation, double dividend) { throw new NotImplementedException(); }
        public virtual InvestmentInformation GetInvestmentDetails(string investment) { throw new NotImplementedException(); }
        public virtual IEnumerable<KeyValuePair<string, double>> GetInvestments(UserAccountToken userToken, DateTime dtValuation) { throw new NotImplementedException(); }
        public virtual void CreateNewInvestment(UserAccountToken userToken, string investment, string symbol, string currency,
                                 double quantity, double scalingFactor, double totalCost, double price,
                                 string exchange, DateTime dtValuation)
        { throw new NotImplementedException(); }
        public virtual IEnumerable<CompanyData> GetInvestmentRecordData(UserAccountToken userToken, DateTime dtValuation) { throw new NotImplementedException(); }
        public virtual void DeactivateInvestment(UserAccountToken userToken, string investment) { throw new NotImplementedException(); }
        public virtual DateTime? GetLatestRecordInvestmentValuationDate(UserAccountToken userToken) { throw new NotImplementedException(); }
        public virtual DateTime? GetPreviousRecordInvestmentValuationDate(UserAccountToken userToken, DateTime dtValuation) { throw new NotImplementedException(); }
        public virtual void AddTradeTransactions(IEnumerable<Stock> trades, TradeType action, UserAccountToken userToken, DateTime dtValuation) { throw new NotImplementedException(); }
        public virtual Trades GetHistoricalTransactions(DateTime dtFrom, DateTime dtTo, UserAccountToken userToken) { throw new NotImplementedException(); }
        public virtual IEnumerable<CompanyData> GetFullInvestmentRecordData(UserAccountToken userToken) { throw new NotImplementedException(); }
        public virtual bool IsExistingRecordValuationDate(UserAccountToken userToken, DateTime dtValuation) { throw new NotImplementedException(); }
    }

    internal class CashAccountInterfaceTest : ICashAccountInterface
    {
        public virtual CashAccountData GetCashBalances(UserAccountToken userToken, DateTime valuationDate) { throw new NotImplementedException(); }
        public virtual int AddCashAccountTransaction(UserAccountToken userToken, DateTime valuationDate, DateTime transactionDate,
                                string type, string parameter, double amount)
        { throw new NotImplementedException(); }

        public virtual void RemoveCashAccountTransaction(UserAccountToken userToken, int transactionID)
        { throw new NotImplementedException(); }
        public virtual void GetCashAccountData(UserAccountToken userToken, string side, DateTime valuationDate, Action<System.Data.IDataReader> fnAddTransaction) { throw new NotImplementedException(); }
        public virtual double GetBalanceInHand(UserAccountToken userToken, DateTime valuationDate) { throw new NotImplementedException(); }

        public IEnumerable<Tuple<DateTime, double>> GetCashTransactions(UserAccountToken userToken, string transactionType)
        { throw new NotImplementedException(); }
    }

    internal class UserAccountInterfaceTest : IUserAccountInterface
    {
        //user account interface
        public virtual void RollbackValuationDate(UserAccountToken userToken, DateTime dtValuation) { throw new NotImplementedException(); }
        public virtual void UpdateMemberAccount(UserAccountToken userToken, DateTime dtValuation, string member, double dAmount) { throw new NotImplementedException(); }
        public virtual double GetMemberSubscription(UserAccountToken userToken, DateTime dtValuation, string member) { throw new NotImplementedException(); }
        public virtual IEnumerable<KeyValuePair<string, double>> GetMemberAccountData(UserAccountToken userToken, DateTime dtValuation) { throw new NotImplementedException(); }
        public virtual double GetPreviousUnitValuation(UserAccountToken userToken, DateTime? previousDate) { throw new NotImplementedException(); }
        public virtual void SaveNewUnitValue(UserAccountToken userToken, DateTime dtValuation, double dUnitValue) { throw new NotImplementedException(); }
        public virtual double GetIssuedUnits(UserAccountToken userToken, DateTime dtValuation) { throw new NotImplementedException(); }
        public virtual AccountModel GetUserAccountData(UserAccountToken userToken) { throw new NotImplementedException(); }
        public virtual double GetStartOfYearValuation(UserAccountToken userToken, DateTime valuationDate) { throw new NotImplementedException(); }
        public virtual IEnumerable<Redemption> GetRedemptions(UserAccountToken userToken, DateTime valuationDate) { throw new NotImplementedException(); }
        public virtual void AddRedemption(UserAccountToken userToken, string user, DateTime transactionDate, double amount) { throw new NotImplementedException(); }
        public virtual RedemptionStatus UpdateRedemption(UserAccountToken userToken, string user, DateTime transactionDate, double amount, double units) { throw new NotImplementedException(); }
        public virtual IEnumerable<string> GetAccountMembers(UserAccountToken userToken, DateTime valuationDate) { throw new NotImplementedException(); }
        public virtual IEnumerable<AccountMember> GetAccountMemberDetails(UserAccountToken userToken, DateTime valuationDate) { throw new NotImplementedException(); }
        public virtual void UpdateMemberForAccount(UserAccountToken userToken, string member, AuthorizationLevel level, bool add) { throw new NotImplementedException(); }
        public virtual void UpdateAccount(UserAccountToken userToken, AccountModel account) { throw new NotImplementedException(); }
        public virtual int CreateAccount(UserAccountToken userToken, AccountModel account) { throw new NotImplementedException(); }
        public virtual AccountModel GetAccount(UserAccountToken userToken) { throw new NotImplementedException(); }
        public virtual IEnumerable<AccountIdentifier> GetAccountNames(string user, bool bCheckAdmin) { throw new NotImplementedException(); }
        public virtual IEnumerable<string> GetActiveCompanies(UserAccountToken userToken, DateTime valuationDate) { throw new NotImplementedException(); }
        public virtual bool InvestmentAccountExists(AccountIdentifier account) { return false; }
        public virtual IEnumerable<double> GetUnitValuationRange(UserAccountToken userToken, DateTime dateFrom, DateTime dateTo) { throw new NotImplementedException(); }
        public virtual int GetUserId(string userName) { throw new NotImplementedException(); }
        public virtual void AddUser(string userName, string description) { }
    }

    internal class HistoricalDataReaderTest : IHistoricalDataReader
    {
        public virtual IEnumerable<HistoricalData> GetHistoricalAccountData(UserAccountToken userToken) { throw new NotImplementedException(); }
        public virtual string GetIndexHistoricalData(UserAccountToken userToken, string symbol) { throw new NotImplementedException(); }
    }

    internal class ConfigurationSettingsTest : IConfigurationSettings
    {
        #region Public Properties

        public virtual IEnumerable<Index> ComparisonIndexes
        {
            get { return Enumerable.Empty<Index>(); }
        }

        public virtual string DatasourceString
        {
            get { return string.Empty; }
        }

        public virtual string AuthDatasourceString
        {
            get { return string.Empty; }
        }

        public virtual string OutputFolder
        {
            get { return string.Empty; }
        }

        public virtual string MarketDatasource { get { return string.Empty; } }

        public virtual string OutputCachedMarketData { get { return string.Empty; } }

        public virtual IEnumerable<string> ReportFormats
        {
            get { return Enumerable.Empty<string>(); }
        }

        public int MaxAccountsPerUser { get { return 5; } }

        public string ScriptFolder { get { return string.Empty; } }

        /// <summary>
        /// List of scheduled tasks.
        /// </summary>
        public IEnumerable<ScheduledTaskDetails> ScheduledTasks { get { return Enumerable.Empty<ScheduledTaskDetails>(); } }

        /// <summary>
        /// Audit file name.
        /// </summary>
        public string AuditFileName { get { return null; } }

        #endregion

        #region Public Methods

        public virtual string GetOutputPath(string account)
        {
            return string.Empty;
        }

        public virtual string GetTemplatePath()
        {
            return string.Empty;
        }

        public virtual string GetTradeFile(string account)
        {
            return string.Empty;
        }

        public virtual bool UpdateComparisonIndexes(IEnumerable<Index> indexes)
        {
            return true;
        }

        public virtual bool UpdateDatasource(string dataSource)
        {
            return true;
        }

        public virtual bool UpdateOutputFolder(string folder)
        {
            return true;
        }

        #endregion

    }

    internal class MarketDataSourceTest : IMarketDataSource
    {
        public int Priority { get { return 0; } }

        public virtual string Name
        {
            get { return string.Empty; }
        }

        public virtual IList<string> GetSources()
        {
            return new List<string> { Name };
        }

        public virtual IEnumerable<HistoricalData> GetHistoricalData(string instrument, string exchange, string source, DateTime dtFrom)
        {
            return Enumerable.Empty<HistoricalData>();
        }

        public virtual bool TryGetFxRate(string baseCurrency, string contraCurrency, string exchange, string source,  out double dFxRate)
        {
            throw new NotImplementedException();
        }

        public virtual bool TryGetMarketData(string symbol, string exchange, string source,  out MarketDataPrice marketData)
        {
            throw new NotImplementedException();
        }

        public void Initialise(IConfigurationSettings settings, ScheduledTaskFactory scheduledTaskFactory) { }

        public Task<MarketDataPrice> RequestPrice(string symbol, string exchange, string source)
        {
            return null;
        }

    }

    #endregion

    /// <summary>
    /// empty implementation of datalayer, settings and market data classes
    /// </summary>
    #region EmptyDatalayerData classes

    internal class EmptyDataLayerTest : IDataLayer
    {
        private IClientDataInterface _clientData;
        private IInvestmentRecordInterface _investmentRecordData;
        private ICashAccountInterface _cashAccountData;
        private IUserAccountInterface _userAccountData;
        private IHistoricalDataReader _historicalData;

        public EmptyDataLayerTest() { }

        public EmptyDataLayerTest(IClientDataInterface clientData,
                            IInvestmentRecordInterface investmentRecordData,
                            ICashAccountInterface cashAccountData,
                            IUserAccountInterface userAccountData,
                            IHistoricalDataReader historicalData)

        {
            _clientData = clientData;
            _investmentRecordData = investmentRecordData;
            _cashAccountData = cashAccountData;
            _userAccountData = userAccountData;
            _historicalData = historicalData;
        }

        public IClientDataInterface ClientData { get { return _clientData ?? new ClientDataEmptyInterfaceTest(); } }
        public IInvestmentRecordInterface InvestmentRecordData { get { return _investmentRecordData ?? new InvestmentRecordEmptyInterfaceTest(); } }
        public ICashAccountInterface CashAccountData { get { return _cashAccountData ?? new CashAccountEmptyInterfaceTest(); } }
        public IUserAccountInterface UserAccountData { get { return _userAccountData ?? new UserAccountEmptyInterfaceTest(); } }
        public IHistoricalDataReader HistoricalData { get { return _historicalData ?? new EmptyHistoricalDataReaderTest(); } }

        public void ConnectNewDatasource(string datasource) { }
    }

    internal class InvestmentReportEmptyWriter : IInvestmentReportWriter
    {
        public static readonly string FileName = "testReport";

        public string GetReportFileName(DateTime ValuationDate)
        {
            return FileName;
        }
    }

    internal class UserAccountEmptyInterfaceTest : IUserAccountInterface
    {
        //user account interface
        public virtual void RollbackValuationDate(UserAccountToken userToken, DateTime dtValuation) { }
        public virtual void UpdateMemberAccount(UserAccountToken userToken, DateTime dtValuation, string member, double dAmount) { }
        public virtual double GetMemberSubscription(UserAccountToken userToken, DateTime dtValuation, string member) { return 0d; }
        public virtual IEnumerable<KeyValuePair<string, double>> GetMemberAccountData(UserAccountToken userToken, DateTime dtValuation) { return null; }
        public virtual double GetPreviousUnitValuation(UserAccountToken userToken, DateTime? previousDate) { return 0d; }
        public virtual void SaveNewUnitValue(UserAccountToken userToken, DateTime dtValuation, double dUnitValue) { }
        public virtual double GetIssuedUnits(UserAccountToken userToken, DateTime dtValuation) { return 0d; }
        public virtual AccountModel GetUserAccountData(UserAccountToken userToken) { return null; }
        public virtual double GetStartOfYearValuation(UserAccountToken userToken, DateTime valuationDate) { return 0d; }
        public virtual IEnumerable<Redemption> GetRedemptions(UserAccountToken userToken, DateTime valuationDate) { return null; }
        public virtual void AddRedemption(UserAccountToken userToken, string user, DateTime transactionDate, double amount) { }
        public virtual RedemptionStatus UpdateRedemption(UserAccountToken userToken, string user, DateTime transactionDate, double amount, double units) { return RedemptionStatus.Complete; }
        public void UpdateMemberForAccount(UserAccountToken userToken, string member, AuthorizationLevel level, bool add) { }
        public virtual void UpdateAccount(UserAccountToken userToken, AccountModel account) { }
        public virtual int CreateAccount(UserAccountToken userToken, AccountModel account) { return 0; }
        public virtual AccountModel GetAccount(UserAccountToken userToken) { return null; }
        public virtual IEnumerable<string> GetAccountMembers(UserAccountToken userToken, DateTime valuationDate) { return null; }
        public virtual IEnumerable<AccountMember> GetAccountMemberDetails(UserAccountToken userToken, DateTime valuationDate) { return null; }
        public virtual IEnumerable<AccountIdentifier> GetAccountNames(string user, bool bCheckAdmin) { return null; }
        public virtual IEnumerable<string> GetActiveCompanies(UserAccountToken userToken, DateTime valuationDate) { return Enumerable.Empty<string>(); }
        public virtual bool InvestmentAccountExists(AccountIdentifier accountName) { return false; }
        public IEnumerable<double> GetUnitValuationRange(UserAccountToken userToken, DateTime dateFrom, DateTime dateTo) { return null; }
        public virtual int GetUserId(string userName) { return 0; }
        public virtual void AddUser(string userName, string description) { }

    }

    internal class InvestmentRecordEmptyInterfaceTest : IInvestmentRecordInterface
    {
        //investment record interface
        //roll (copy) an investment from dtPrevious to dtValuation
        public virtual void RollInvestment(UserAccountToken userToken, string investment, DateTime dtValuation, DateTime dtPreviousValaution) { }
        public virtual void UpdateInvestmentQuantity(UserAccountToken userToken, string investment, DateTime dtValuation, double quantity) { }
        public virtual void AddNewShares(UserAccountToken userToken, string investment, double quantity, DateTime dtValaution, double dTotalCost) { }
        public virtual void SellShares(UserAccountToken userToken, string investment, double quantity, DateTime dtValuation) { }
        public virtual void UpdateClosingPrice(UserAccountToken userToken, string investment, DateTime dtValuation, double price) { }
        public virtual void UpdateDividend(UserAccountToken userToken, string investment, DateTime dtValuation, double dividend) { }
        public virtual InvestmentInformation GetInvestmentDetails(string investment) { return null; }
        public virtual IEnumerable<KeyValuePair<string, double>> GetInvestments(UserAccountToken userToken, DateTime dtValuation) { return Enumerable.Empty<KeyValuePair<string, double>>(); }
        public virtual void CreateNewInvestment(UserAccountToken userToken, string investment, string symbol, string currency,
                                 double quantity, double scalingFactor, double totalCost, double price,
                                 string exchange, DateTime dtValuation)
        { }
        public virtual IEnumerable<CompanyData> GetInvestmentRecordData(UserAccountToken userToken, DateTime dtValuation) { return Enumerable.Empty<CompanyData>(); }
        public virtual void DeactivateInvestment(UserAccountToken userToken, string investment) { }
        public virtual DateTime? GetLatestRecordInvestmentValuationDate(UserAccountToken userToken) { return null; }
        public virtual DateTime? GetPreviousRecordInvestmentValuationDate(UserAccountToken userToken, DateTime dtValuation) { return null; }
        public virtual void AddTradeTransactions(IEnumerable<Stock> trades, TradeType action, UserAccountToken userToken, DateTime dtValuation) { }
        public virtual Trades GetHistoricalTransactions(DateTime dtFrom, DateTime dtTo, UserAccountToken userToken) { return null; }
        public virtual IEnumerable<CompanyData> GetFullInvestmentRecordData(UserAccountToken userToken) { return Enumerable.Empty<CompanyData>(); }
        public virtual bool IsExistingRecordValuationDate(UserAccountToken userToken, DateTime dtValuation) { return true; }
    }

    internal class ClientDataEmptyInterfaceTest : IClientDataInterface
    {
        //client interface
        public virtual IEnumerable<DateTime> GetRecentValuationDates(UserAccountToken userToken, DateTime dtDateFrom) { return null; }
        public virtual IEnumerable<string> GetTransactionTypes(string side) { return Enumerable.Empty<string>(); }
        public virtual DateTime? GetLatestValuationDate(UserAccountToken userToken) { return null; }
        public virtual DateTime? GetPreviousAccountValuationDate(UserAccountToken userToken, DateTime dtValuation) { return null; }
        public virtual bool IsExistingValuationDate(UserAccountToken userToken, DateTime valuationDate) { return false; }
        public virtual IEnumerable<string> GetAccountTypes() { return Enumerable.Empty<string>(); }
        public virtual IEnumerable<string> GetAllCompanies() { return Enumerable.Empty<string>(); }
        public virtual Stock GetTradeItem(UserAccountToken userToken, string name) { return null; }
        public virtual int UndoLastTransaction(UserAccountToken userToken, DateTime fromValuationDate) { return 0; }
        public Transaction GetLastTransaction(UserAccountToken userToken, DateTime fromValuationDate) { return null; }
        public virtual IEnumerable<ValuationData> GetAllValuations(UserAccountToken userToken) { return null; }
    }

    internal class CashAccountEmptyInterfaceTest : ICashAccountInterface
    {
        public virtual CashAccountData GetCashBalances(UserAccountToken userToken, DateTime valuationDate) { return new CashAccountData(); }
        public virtual int AddCashAccountTransaction(UserAccountToken userToken, DateTime valuationDate, DateTime transactionDate,
                                string type, string parameter, double amount)
        { return 0; }
        public virtual void RemoveCashAccountTransaction(UserAccountToken userToken, int transactionID)
        { }
        public virtual void GetCashAccountData(UserAccountToken userToken, string side, DateTime valuationDate, Action<System.Data.IDataReader> fnAddTransaction) { }
        public virtual double GetBalanceInHand(UserAccountToken userToken, DateTime valuationDate) { return 0d; }

        public IEnumerable<Tuple<DateTime, double>> GetCashTransactions(UserAccountToken userToken, string transactionType) { return null; }
    }

    internal class EmptyHistoricalDataReaderTest : IHistoricalDataReader
    {
        public virtual IEnumerable<HistoricalData> GetHistoricalAccountData(UserAccountToken userToken) { return null; }
        public virtual string GetIndexHistoricalData(UserAccountToken userToken, string symbol) { return null; }
    }

    internal class EmptyConfigurationSettingsTest : IConfigurationSettings
    {
        #region Public Properties

        public virtual IEnumerable<Index> ComparisonIndexes
        {
            get
            {
                return null;
            }
        }

        public virtual string DatasourceString
        {
            get
            {
                return null;
            }
        }

        public virtual string AuthDatasourceString
        {
            get
            {
                return null;
            }
        }

        public virtual string OutputFolder
        {
            get
            {
                return null;
            }
        }

        public virtual string MarketDatasource { get { return null; } }

        public virtual string OutputCachedMarketData { get { return null; } }

        public virtual IEnumerable<string> ReportFormats
        {
            get { return null; }
        }

        public virtual int MaxAccountsPerUser {get { return 0; } }

        public virtual string ScriptFolder { get { return null; } }

        /// <summary>
        /// List of scheduled tasks.
        /// </summary>
        public virtual IEnumerable<ScheduledTaskDetails> ScheduledTasks { get { return Enumerable.Empty<ScheduledTaskDetails>(); } }

        /// <summary>
        /// Audit file name.
        /// </summary>
        public string AuditFileName { get { return null; } }

        #endregion

        #region Public Methods

        public virtual string GetOutputPath(string account)
        {
            return null;
        }

        public virtual string GetTemplatePath()
        {
            return null;
        }

        public virtual string GetTradeFile(string account)
        {
            return null;
        }

        public virtual bool UpdateComparisonIndexes(IEnumerable<Index> indexes)
        {
            return true;
        }

        public virtual bool UpdateDatasource(string dataSource)
        {
            return true;
        }

        public virtual bool UpdateOutputFolder(string folder)
        {
            return true;
        }

        #endregion

    }

    internal class EmptyMarketDataSourceTest : IMarketDataSource
    {
        public int Priority { get { return 0; } }

        public virtual string Name
        {
            get
            {
                return null;
            }
        }

        public virtual IList<string> GetSources()
        {
            return new List<string> { Name };
        }

        public virtual IEnumerable<HistoricalData> GetHistoricalData(string instrument, string exchange, string source, DateTime dtFrom)
        {
            return null;
        }

        public virtual bool TryGetFxRate(string baseCurrency, string contraCurrency, string exchange, string source, out double dFxRate)
        {
            dFxRate = 0d;
            return false;
        }

        public virtual bool TryGetMarketData(string symbol, string exchange, string source, out MarketDataPrice marketData)
        {
            marketData = null;
            return false;
        }

        public void Initialise(IConfigurationSettings settings, ScheduledTaskFactory scheduledTaskFactory) { }

        public Task<MarketDataPrice> RequestPrice(string symbol, string exchange, string source)
        {
            return null;
        }

    }

    #endregion
}
