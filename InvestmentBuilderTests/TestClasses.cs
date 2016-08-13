using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderCore;
using MarketDataServices;

namespace InvestmentBuilderTests
{
    //empty test instance of IDataLayer interface
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

    /// <summary>
    /// empty test implementation client interface to datalayer contains client specific methods
    /// </summary>
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
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual string DatasourceString
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual string OutputFolder
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<string> ReportFormats
        {
            get { throw new NotImplementedException(); }
        }       

        public virtual string GetOutputPath(string account)
        {
            throw new NotImplementedException();
        }

        public virtual string GetTemplatePath()
        {
            throw new NotImplementedException();
        }

        public virtual string GetTradeFile(string account)
        {
            throw new NotImplementedException();
        }

        public virtual bool UpdateComparisonIndexes(IEnumerable<Index> indexes)
        {
            throw new NotImplementedException();
        }

        public virtual bool UpdateDatasource(string dataSource)
        {
            throw new NotImplementedException();
        }

        public virtual bool UpdateOutputFolder(string folder)
        {
            throw new NotImplementedException();
        }
    }

    internal class MarketDataSourceTest : IMarketDataSource
    {
        public int Priority { get { return 0; } }

        public virtual string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual IEnumerable<HistoricalData> GetHistoricalData(string instrument, DateTime dtFrom)
        {
            throw new NotImplementedException();
        }

        public virtual bool TryGetFxRate(string baseCurrency, string contraCurrency, out double dFxRate)
        {
            throw new NotImplementedException();
        }

        public virtual bool TryGetMarketData(string symbol, string exchange, out MarketDataPrice marketData)
        {
            throw new NotImplementedException();
        }
    }
}
