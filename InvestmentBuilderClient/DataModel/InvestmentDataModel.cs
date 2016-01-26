using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using InvestmentBuilderCore;
using System.Data;
using InvestmentBuilder;

namespace InvestmentBuilderClient.DataModel
{
    //ObservableCollection
    //BindingList
    internal class TradeDetails : Stock
    {
        public TradeType Action { get; set; }
        public double? ManualPrice { get; set; }
    }   

    internal class InvestmentDataModel : IDisposable
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private IDataLayer _dataLayer;
        private IClientDataInterface _clientData;
        private IInvestmentRecordInterface _recordData;
        private IAuthorizationManager _authorizationManager;
        private BrokerManager _brokerManager;

        private UserAccountToken _userToken; //cache user token

        public DateTime? LatestValuationDate { get; set; }

        private DateTime LatestRecordValuationDate { get; set; }

        public List<CompanyData> PortfolioItemsList { get; set; }

        //public bool TradeUpdated { get; private set; }

        private string _userName = string.Format(@"{0}\{1}", Environment.UserDomainName, Environment.UserName).ToUpper();

        private Dictionary<string, string> _typeProcedureLookup = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase)
        {
            {"Dividend", "GetActiveCompanies"},
            {"Subscription", "GetAccountMembers"}
        };

        public InvestmentDataModel(IDataLayer dataLayer, IAuthorizationManager authorizationManager, BrokerManager brokerManager) 
        {
            _dataLayer = dataLayer;
            _clientData = dataLayer.ClientData;
            _recordData = dataLayer.InvestmentRecordData;
            _authorizationManager = authorizationManager;
            _brokerManager = brokerManager;
            //var connectstr = @"Data Source=TRAVELPC\SQLEXPRESS;Initial Catalog=InvestmentBuilderTest;Integrated Security=True";
            // _connection = new SqlConnection(dataSource);
            // _connection.Open();
            // logger.Log(LogLevel.Info, "connected to datasource {0}", dataSource);
        }

        public IEnumerable<DateTime> GetValuationDates()
        {
            var dates = _clientData.GetRecentValuationDates(_userToken).ToList();

            if(dates.Count > 0)
            {
                if(dates.First().Month != DateTime.Today.Month ||
                    dates.First().Year != DateTime.Today.Year)
                {
                    //nowon a different month so add the current date to the list
                    dates.Insert(0, DateTime.Today);
                }
            }
            else
            {
                dates.Add(DateTime.Today);
            }
            return dates;
        }

        public IEnumerable<string> GetsTransactionTypes(string side)
        {
            return _clientData.GetTransactionTypes(side);
        }

        public IEnumerable<string> GetParametersForType(string type)
        {
            if(_typeProcedureLookup.ContainsKey(type))
            {
                var methodInfo = _clientData.GetType().GetMethod(_typeProcedureLookup[type]);
                if(methodInfo != null)
                {
                    return methodInfo.Invoke(_clientData, new object[] { _userToken, LatestRecordValuationDate }) as IEnumerable<string>;
                }
            }
            return Enumerable.Empty<string>();
        }

       // public void GetCashAccountData(DateTime dtValuationDate, string side, Action<SqlDataReader> fnAddTransaction)
        public void GetCashAccountData(DateTime dtValuationDate, string side, Action<IDataReader> fnAddTransaction)
        {
            _clientData.GetCashAccountData(_userToken, side, dtValuationDate, fnAddTransaction);
        }

        private void SetLatestValuationDate()
        {
            LatestValuationDate = _clientData.GetLatestValuationDate(_userToken);
            LatestRecordValuationDate = _recordData.GetLatestRecordInvestmentValuationDate(_userToken) ?? DateTime.Today;
        }

        public double GetBalanceInHand(DateTime dtValuation)
        {
            return _clientData.GetBalanceInHand(_userToken, dtValuation);
        }

        public void SaveCashAccountData(DateTime dtValuationDate, DateTime dtTransactionDate,
                                    string type, string parameter, double amount)
        {
            _clientData.AddCashAccountData(_userToken, dtValuationDate, dtTransactionDate, type,
                                                parameter, amount);
        }

        public void ReloadData(string dataSource)
        {
            _dataLayer.ConnectNewDatasource(dataSource);
            logger.Log(LogLevel.Info, "reload from datasource {0}", dataSource);
        }

        public IEnumerable<string> GetAccountNames()
        {
            return _clientData.GetAccountNames(_userName);
        }

        public bool IsExistingValuationDate(DateTime dtValuation)
        {
            return _clientData.IsExistingValuationDate(_userToken, dtValuation);
        }

        public IEnumerable<KeyValuePair<string, AuthorizationLevel>> GetAccountMembers(UserAccountToken token)
        {
            return _clientData.GetAccountMemberDetails(token, DateTime.Today);
        }

        private void _UpdateMemberForAccount(UserAccountToken token, string member, AuthorizationLevel level, bool bAdd)
        {
            _clientData.UpdateMemberForAccount(token, member, level,  bAdd);
        }

        public void UpdateUserAccount(AccountModel account)
        {
            logger.Log(LogLevel.Info, "creating/modifying account {0}", account.Name);
            logger.Log(LogLevel.Info, "Password {0}", account.Password);
            logger.Log(LogLevel.Info, "Description {0}", account.Description);
            logger.Log(LogLevel.Info, "Reporting Currency {0}", account.ReportingCurrency);
            logger.Log(LogLevel.Info, "Account Type {0}", account.Type);
            logger.Log(LogLevel.Info, "Enabled {0}", account.Enabled);
            logger.Log(LogLevel.Info, "Broker {0}", account.Broker);

            var tmpToken = _authorizationManager.GetUserAccountToken(_userName, account.Name);

            _clientData.CreateAccount(tmpToken, account);
            var existingMembers = _clientData.GetAccountMembers(tmpToken, DateTime.Today); 
                //GetAccountMembers(tmpToken).ToList();
            foreach(var member in existingMembers)
            {
                if(account.Members.Where(x => string.Equals(x.Key, member, StringComparison.InvariantCultureIgnoreCase)).Count() == 0)
                {
                    //remove this member
                    logger.Log(LogLevel.Info, "removing member {0} from account {1}", member, account.Name);
                    _UpdateMemberForAccount(tmpToken, member, AuthorizationLevel.NONE, false);
                }
            }

            //now add the members
            foreach (var member in account.Members)
            {
                logger.Log(LogLevel.Info, "adding member {0} to account {1}", member, account.Name);
                _UpdateMemberForAccount(tmpToken, member.Key, member.Value, true);
            }
        }

        public IEnumerable<string> GetAccountTypes()
        {
            return _clientData.GetAccountTypes();
        }

        public AccountModel GetAccountData(string account)
        {
            var tmpToken = _authorizationManager.GetUserAccountToken(_userName, account);
            AccountModel data = _clientData.GetAccount(tmpToken);
            if(data != null)
            {
                data.Members = GetAccountMembers(tmpToken).ToList();
            }

            return data;
        }

        public void UpdateAccountName(string account)
        {
            logger.Log(LogLevel.Info, "updating to account {0} ", account);
            _userToken = _authorizationManager.GetUserAccountToken(_userName, account);
            SetLatestValuationDate();
            TradeUpdated = false;
        }

        public InvestmentInformation GetInvestmentDetails(string name)
        {
            return _recordData.GetInvestmentDetails(name);
        }

        private ManualPrices GetManualPrices()
        {
            ManualPrices manualPrices = new ManualPrices();
            if (PortfolioItemsList != null)
            {
                PortfolioItemsList.ForEach(x =>
                {
                    double dPrice;
                    if ((string.IsNullOrEmpty(x.ManualPrice) == false) && (Double.TryParse(x.ManualPrice, out dPrice) == true))
                    {
                        manualPrices.Add(x.Name, dPrice);
                    }
                });
            }
            return manualPrices;
        }

        public void LoadPortfolioItems()
        {
            ManualPrices manualPrices = GetManualPrices();
            PortfolioItemsList =  ContainerManager.ResolveValue<InvestmentBuilder.InvestmentBuilder>().GetCurrentInvestments(_userToken, manualPrices).ToList();
            logger.Log(LogLevel.Info, "loaded {0} items from database for account {1}", PortfolioItemsList.Count, _userToken.Account);
        }

        public void UpdateTrade(TradeDetails trade)
        {
            if (trade != null)
            {
                logger.Log(LogLevel.Info, "updating trade {0} in database", trade.Name);
                Trades tradesList = new Trades();
                tradesList.Buys = trade.Action == TradeType.BUY ? new[] { trade } : Enumerable.Empty<Stock>().ToArray();
                tradesList.Sells = trade.Action == TradeType.SELL ? new[] { trade } : Enumerable.Empty<Stock>().ToArray();
                tradesList.Changed = trade.Action == TradeType.MODIFY ? new[] { trade } : Enumerable.Empty<Stock>().ToArray();

                var manualPrices = GetManualPrices();
                if(trade.ManualPrice.HasValue)
                {
                    manualPrices.Add(trade.Name, trade.ManualPrice.Value);
                }
                ContainerManager.ResolveValue<InvestmentBuilder.InvestmentBuilder>().UpdateTrades(
                                                                _userToken,
                                                                tradesList,
                                                                manualPrices);

                //PortfolioItemsList =  ContainerManager.ResolveValue<InvestmentBuilder.InvestmentBuilder>().GetCurrentInvestments(_userToken, manualPrices).ToList();
                TradeUpdated = true;
            }
        }

        public AssetReport BuildAssetReport(DateTime dtValuation)
        {
            return ContainerManager.ResolveValue<InvestmentBuilder.InvestmentBuilder>()
                                                           .BuildAssetReport(_userToken, dtValuation, DateTime.Now, true, GetManualPrices());
        }

        public IEnumerable<string> GetAllCompanies()
        {
            return _clientData.GetAllCompanies();
        }

        public UserAccountToken GetUserToken()
        {
            return _userToken;
        }

        public IEnumerable<string> GetBrokers()
        {
            return _brokerManager.GetBrokers();
        }

        public void UndoLastTransaction()
        {
            _clientData.UndoLastTransaction(_userToken);
        }

        public void Dispose()
        {
            //    _connection.Close();
        //    _connection.Dispose();
        }
    }
}
