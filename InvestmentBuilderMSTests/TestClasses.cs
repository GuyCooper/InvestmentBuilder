using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderCore;
using MarketDataServices;

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
        public virtual IEnumerable<string> GetActiveCompanies(UserAccountToken userToken, DateTime valuationDate) { throw new NotImplementedException(); }
        public virtual IEnumerable<string> GetAccountMembers(UserAccountToken userToken, DateTime valuationDate) { throw new NotImplementedException(); }
        public virtual IEnumerable<KeyValuePair<string, AuthorizationLevel>> GetAccountMemberDetails(UserAccountToken userToken, DateTime valuationDate) { throw new NotImplementedException(); }
        public virtual DateTime? GetLatestValuationDate(UserAccountToken userToken) { throw new NotImplementedException(); }
        public virtual DateTime? GetPreviousAccountValuationDate(UserAccountToken userToken, DateTime dtValuation) { throw new NotImplementedException(); }
        public virtual IEnumerable<string> GetAccountNames(string user) { throw new NotImplementedException(); }
        public virtual bool IsExistingValuationDate(UserAccountToken userToken, DateTime valuationDate) { throw new NotImplementedException(); }
        public virtual void UpdateMemberForAccount(UserAccountToken userToken, string member, AuthorizationLevel level, bool add) { throw new NotImplementedException(); }
        public virtual void CreateAccount(UserAccountToken userToken, AccountModel account) { throw new NotImplementedException(); }
        public virtual AccountModel GetAccount(UserAccountToken userToken) { throw new NotImplementedException(); }
        public virtual IEnumerable<string> GetAccountTypes() { throw new NotImplementedException(); }
        public virtual IEnumerable<string> GetAllCompanies() { throw new NotImplementedException(); }
        public virtual Stock GetTradeItem(UserAccountToken userToken, string name) { throw new NotImplementedException(); }
        public virtual void UndoLastTransaction(UserAccountToken userToken) { throw new NotImplementedException(); }
    }

    internal class InvestmentRecordInterfaceTest : IInvestmentRecordInterface
    {
        //investment record interface
        //roll (copy) an investment from dtPrevious to dtValuation
        public virtual void RollInvestment(UserAccountToken userToken, string investment, DateTime dtValuation, DateTime dtPreviousValaution) { throw new NotImplementedException(); }
        public virtual void UpdateInvestmentQuantity(UserAccountToken userToken, string investment, DateTime dtValuation, int quantity) { throw new NotImplementedException(); }
        public virtual void AddNewShares(UserAccountToken userToken, string investment, int quantity, DateTime dtValaution, double dTotalCost) { throw new NotImplementedException(); }
        public virtual void SellShares(UserAccountToken userToken, string investment, int quantity, DateTime dtValuation) { throw new NotImplementedException(); }
        public virtual void UpdateClosingPrice(UserAccountToken userToken, string investment, DateTime dtValuation, double price) { throw new NotImplementedException(); }
        public virtual void UpdateDividend(UserAccountToken userToken, string investment, DateTime dtValuation, double dividend) { throw new NotImplementedException(); }
        public virtual InvestmentInformation GetInvestmentDetails(string investment) { throw new NotImplementedException(); }
        public virtual IEnumerable<KeyValuePair<string, double>> GetInvestments(UserAccountToken userToken, DateTime dtValuation) { throw new NotImplementedException(); }
        public virtual void CreateNewInvestment(UserAccountToken userToken, string investment, string symbol, string currency,
                                 int quantity, double scalingFactor, double totalCost, double price,
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
        public virtual CashAccountData GetCashAccountData(UserAccountToken userToken, DateTime valuationDate) { throw new NotImplementedException(); }
        public virtual void AddCashAccountTransaction(UserAccountToken userToken, DateTime valuationDate, DateTime transactionDate,
                                string type, string parameter, double amount)
        { throw new NotImplementedException(); }

        public virtual void RemoveCashAccountTransaction(UserAccountToken userToken, DateTime valuationDate, DateTime transactionDate,
                        string type, string parameter)
        { throw new NotImplementedException(); }
        public virtual void GetCashAccountTransactions(UserAccountToken userToken, string side, DateTime valuationDate, Action<System.Data.IDataReader> fnAddTransaction) { throw new NotImplementedException(); }
        public virtual double GetBalanceInHand(UserAccountToken userToken, DateTime valuationDate) { throw new NotImplementedException(); }
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
        public virtual UserAccountData GetUserAccountData(UserAccountToken userToken) { throw new NotImplementedException(); }
        public virtual double GetStartOfYearValuation(UserAccountToken userToken, DateTime valuationDate) { throw new NotImplementedException(); }
        public virtual IEnumerable<Redemption> GetRedemptions(UserAccountToken userToken, DateTime valuationDate) { throw new NotImplementedException(); }
        public virtual void AddRedemption(UserAccountToken userToken, string user, DateTime transactionDate, double amount) { throw new NotImplementedException(); }
        public virtual void UpdateRedemption(UserAccountToken userToken, string user, DateTime transactionDate, double amount, double units) { throw new NotImplementedException(); }
    }

    internal class HistoricalDataReaderTest : IHistoricalDataReader
    {
        public virtual IEnumerable<HistoricalData> GetHistoricalAccountData(UserAccountToken userToken) { throw new NotImplementedException(); }
    }

    internal class ConfigurationSettingsTest : IConfigurationSettings
    {
        public virtual IEnumerable<Index> ComparisonIndexes
        {
            get { return Enumerable.Empty<Index>(); }
        }

        public virtual string DatasourceString
        {
            get { return string.Empty; }
        }

        public virtual string OutputFolder
        {
            get { return string.Empty; }
        }

        public virtual IEnumerable<string> ReportFormats
        {
            get { return Enumerable.Empty<string>(); }
        }

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

        public virtual bool TryGetFxRate(string baseCurrency, string contraCurrency, string source,  out double dFxRate)
        {
            throw new NotImplementedException();
        }

        public virtual bool TryGetMarketData(string symbol, string exchange, string source,  out MarketDataPrice marketData)
        {
            throw new NotImplementedException();
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

        public string GetReportFileName(string outputPath, DateTime ValuationDate)
        {
            return FileName;
        }

        public void WriteAssetReport(AssetReport report, double startOfYear, string outputPath)
        {
        }

        public void WritePerformanceData(IList<IndexedRangeData> data, string path, DateTime dtValuation)
        {
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
        public virtual UserAccountData GetUserAccountData(UserAccountToken userToken) { return null; }
        public virtual double GetStartOfYearValuation(UserAccountToken userToken, DateTime valuationDate) { return 0d; }
        public virtual IEnumerable<Redemption> GetRedemptions(UserAccountToken userToken, DateTime valuationDate) { return null; }
        public virtual void AddRedemption(UserAccountToken userToken, string user, DateTime transactionDate, double amount) { }
        public virtual void UpdateRedemption(UserAccountToken userToken, string user, DateTime transactionDate, double amount, double units) { }
    }

    internal class InvestmentRecordEmptyInterfaceTest : IInvestmentRecordInterface
    {
        //investment record interface
        //roll (copy) an investment from dtPrevious to dtValuation
        public virtual void RollInvestment(UserAccountToken userToken, string investment, DateTime dtValuation, DateTime dtPreviousValaution) { }
        public virtual void UpdateInvestmentQuantity(UserAccountToken userToken, string investment, DateTime dtValuation, int quantity) { }
        public virtual void AddNewShares(UserAccountToken userToken, string investment, int quantity, DateTime dtValaution, double dTotalCost) { }
        public virtual void SellShares(UserAccountToken userToken, string investment, int quantity, DateTime dtValuation) { }
        public virtual void UpdateClosingPrice(UserAccountToken userToken, string investment, DateTime dtValuation, double price) { }
        public virtual void UpdateDividend(UserAccountToken userToken, string investment, DateTime dtValuation, double dividend) { }
        public virtual InvestmentInformation GetInvestmentDetails(string investment) { return null; }
        public virtual IEnumerable<KeyValuePair<string, double>> GetInvestments(UserAccountToken userToken, DateTime dtValuation) { return Enumerable.Empty<KeyValuePair<string, double>>(); }
        public virtual void CreateNewInvestment(UserAccountToken userToken, string investment, string symbol, string currency,
                                 int quantity, double scalingFactor, double totalCost, double price,
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
        public virtual IEnumerable<string> GetActiveCompanies(UserAccountToken userToken, DateTime valuationDate) { return Enumerable.Empty<string>(); }
        public virtual IEnumerable<string> GetAccountMembers(UserAccountToken userToken, DateTime valuationDate) { return Enumerable.Empty<string>(); }
        public virtual IEnumerable<KeyValuePair<string, AuthorizationLevel>> GetAccountMemberDetails(UserAccountToken userToken, DateTime valuationDate) { return null; }
        public virtual DateTime? GetLatestValuationDate(UserAccountToken userToken) { return null; }
        public virtual DateTime? GetPreviousAccountValuationDate(UserAccountToken userToken, DateTime dtValuation) { return null; }
        public virtual IEnumerable<string> GetAccountNames(string user) { return null; }
        public virtual bool IsExistingValuationDate(UserAccountToken userToken, DateTime valuationDate) { return false; }
        public virtual void UpdateMemberForAccount(UserAccountToken userToken, string member, AuthorizationLevel level, bool add) { }
        public virtual void CreateAccount(UserAccountToken userToken, AccountModel account) { }
        public virtual AccountModel GetAccount(UserAccountToken userToken) { return null; }
        public virtual IEnumerable<string> GetAccountTypes() { return Enumerable.Empty<string>(); }
        public virtual IEnumerable<string> GetAllCompanies() { return Enumerable.Empty<string>(); }
        public virtual Stock GetTradeItem(UserAccountToken userToken, string name) { return null; }
        public virtual void UndoLastTransaction(UserAccountToken userToken) { }
    }

    internal class CashAccountEmptyInterfaceTest : ICashAccountInterface
    {
        public virtual CashAccountData GetCashAccountData(UserAccountToken userToken, DateTime valuationDate) { return new CashAccountData(); }
        public virtual void AddCashAccountTransaction(UserAccountToken userToken, DateTime valuationDate, DateTime transactionDate,
                                string type, string parameter, double amount)
        { }
        public virtual void RemoveCashAccountTransaction(UserAccountToken userToken, DateTime valuationDate, DateTime transactionDate,
                        string type, string parameter)
        { }
        public virtual void GetCashAccountTransactions(UserAccountToken userToken, string side, DateTime valuationDate, Action<System.Data.IDataReader> fnAddTransaction) { }
        public virtual double GetBalanceInHand(UserAccountToken userToken, DateTime valuationDate) { return 0d; }
    }

    internal class EmptyHistoricalDataReaderTest : IHistoricalDataReader
    {
        public virtual IEnumerable<HistoricalData> GetHistoricalAccountData(UserAccountToken userToken) { return null; }
    }

    internal class EmptyConfigurationSettingsTest : IConfigurationSettings
    {
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

        public virtual string OutputFolder
        {
            get
            {
                return null;
            }
        }

        public virtual IEnumerable<string> ReportFormats
        {
            get { return null; }
        }

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

        public virtual bool TryGetFxRate(string baseCurrency, string contraCurrency, string source, out double dFxRate)
        {
            dFxRate = 0d;
            return false;
        }

        public virtual bool TryGetMarketData(string symbol, string exchange, string source, out MarketDataPrice marketData)
        {
            marketData = null;
            return false;
        }
    }

    #endregion
}
