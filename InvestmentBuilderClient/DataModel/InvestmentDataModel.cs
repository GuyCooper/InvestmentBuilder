﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using InvestmentBuilderCore;
using System.Data;
using InvestmentBuilder;
using PerformanceBuilderLib;

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
        private CashAccountTransactionManager _cashAccountManager;
        private InvestmentBuilder.InvestmentBuilder _investmentBuilder;
        private PerformanceBuilder _performanceBuilder;
        private AccountManager _accountManager;

        private UserAccountToken _userToken; //cache user token
        private int _TradeUpdateCount;

        public DateTime? LatestValuationDate { get; set; }

        private DateTime LatestRecordValuationDate { get; set; }

        public List<CompanyData> PortfolioItemsList { get; set; }

        public event Func<bool, bool> TradeUpdateEvent; 

        private string _userName = string.Format(@"{0}\{1}", Environment.UserDomainName, Environment.UserName).ToUpper();

        public InvestmentDataModel(IDataLayer dataLayer, 
                                   InvestmentBuilder.InvestmentBuilder investmentBuilder,
                                   IAuthorizationManager authorizationManager,
                                   BrokerManager brokerManager,
                                   CashAccountTransactionManager cashAccountManager,
                                   PerformanceBuilder performanceBuilder,
                                   AccountManager accountManager ) 
        {
            _dataLayer = dataLayer;
            _clientData = dataLayer.ClientData;
            _recordData = dataLayer.InvestmentRecordData;
            _authorizationManager = authorizationManager;
            _brokerManager = brokerManager;
            _cashAccountManager = cashAccountManager;
            _investmentBuilder = investmentBuilder;
            _performanceBuilder = performanceBuilder;
            _accountManager = accountManager;
        }

        private void UpdateTradeUpdateEvent()
        {
            if(TradeUpdateEvent != null)
            {
                TradeUpdateEvent(_TradeUpdateCount > 0);
            }
        }

        public IEnumerable<DateTime> GetValuationDates()
        {
            var dates = _clientData.GetRecentValuationDates(_userToken, DateTime.Now).ToList();

            //always have the current date time as the first entry in the combo
            dates.Insert(0, DateTime.Now);

            return dates;
        }

        public IEnumerable<string> GetsTransactionTypes(string side)
        {
            return _cashAccountManager.GetTransactionTypes(side).Where(x => string.Equals(x, "Redemption") == false);
        }

        public IEnumerable<string> GetParametersForType(string type)    
        {
            return _investmentBuilder.GetParametersForTransactionType(_userToken, LatestRecordValuationDate, type);
        }

        private void SetLatestValuationDate()
        {
            LatestValuationDate = _clientData.GetLatestValuationDate(_userToken);
            LatestRecordValuationDate = _recordData.GetLatestRecordInvestmentValuationDate(_userToken) ?? DateTime.Today;
        }

        public void ReloadData(string dataSource)
        {
            _dataLayer.ConnectNewDatasource(dataSource);
            logger.Log(LogLevel.Info, "reload from datasource {0}", dataSource);
        }

        public IEnumerable<string> GetAccountNames()
        {
            return _accountManager.GetAccountNames(_userName);
        }

        public bool IsExistingValuationDate(DateTime dtValuation)
        {
            return _clientData.IsExistingValuationDate(_userToken, dtValuation);
        }

        public IEnumerable<AccountMember> GetAccountMembers(UserAccountToken token)
        {
            return _accountManager.GetAccountMembers(token, LatestValuationDate ?? DateTime.Today);
        }

        public void UpdateUserAccount(AccountModel account)
        {
            _accountManager.UpdateUserAccount(_userName, account, LatestValuationDate ?? DateTime.Today);
        }

        public IEnumerable<string> GetAccountTypes()
        {
            return _clientData.GetAccountTypes();
        }

        public AccountModel GetAccountData(string account)
        {
          
            var tmpToken = _authorizationManager.GetUserAccountToken(_userName, account);
            return _accountManager.GetAccountData(tmpToken, LatestValuationDate ?? DateTime.Today);
        }

        public void UpdateAccountName(string account)
        {
            logger.Log(LogLevel.Info, "updating to account {0} ", account);
            _userToken = _authorizationManager.GetUserAccountToken(_userName, account);
            SetLatestValuationDate();
            _TradeUpdateCount = 0;
            UpdateTradeUpdateEvent();
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
            PortfolioItemsList = _investmentBuilder.GetCurrentInvestments(_userToken, manualPrices).ToList();
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
                if(trade.ManualPrice.HasValue && (manualPrices.ContainsKey(trade.Name) == false))
                {
                    manualPrices.Add(trade.Name, trade.ManualPrice.Value);
                }
                _investmentBuilder.UpdateTrades(
                                                _userToken,
                                                tradesList,
                                                manualPrices,
                                                null);

                //PortfolioItemsList =  ContainerManager.ResolveValue<InvestmentBuilder.InvestmentBuilder>().GetCurrentInvestments(_userToken, manualPrices).ToList();
                _TradeUpdateCount++;
                UpdateTradeUpdateEvent();
            }
        }

        public AssetReport BuildAssetReport(DateTime dtValuation, bool update)
        {
            var  report = _investmentBuilder.BuildAssetReport(_userToken, dtValuation, update, GetManualPrices(),null);
            if(report != null)
            {
                _performanceBuilder.Run(_userToken, dtValuation, null);
            }
            return report;
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
            if (_TradeUpdateCount > 0)
            {
                --_TradeUpdateCount;
                _clientData.UndoLastTransaction(_userToken);
                UpdateTradeUpdateEvent();
            }
        }

        public IEnumerable<Redemption> GetRedemptions(DateTime dtValuation)
        {
            return _investmentBuilder.GetRedemptions(_userToken, dtValuation);
        }

        public bool RequestRedemption(string user, double amount, DateTime dtValuation )
        {
            return _investmentBuilder.RequestRedemption(_userToken, user, amount, dtValuation);
        }

        public IList<PaymentTransaction> GetPaymentTransactions(DateTime dtValuationDate, out double dTotal)
        {
            return _cashAccountManager.GetPaymentTransactions(_userToken, dtValuationDate, out dTotal);
        }

        public IList<ReceiptTransaction> GetReceiptTransactions(DateTime dtValuationDate, out double dTotal)
        {
            var dtPrevious = _clientData.GetPreviousAccountValuationDate(_userToken, dtValuationDate);
            return _cashAccountManager.GetReceiptTransactions(_userToken, dtValuationDate, dtPrevious , out dTotal);
        }

        public void AddCashTransaction(DateTime dtValuation, DateTime dtTransactionDate,
                                   string type, string parameter, double amount)
        {
            _cashAccountManager.AddTransaction(_userToken, dtValuation, dtTransactionDate,
                                                type, parameter, amount);
        }

        public void RemoveCashTransaction(DateTime dtValuationDate, DateTime dtTransactionDate,
                                        string type, string parameter)
        {
            _cashAccountManager.RemoveTransaction(_userToken, dtValuationDate, dtTransactionDate, type, parameter);
        }

        public string GetPaymentMnenomic()
        {
            return _cashAccountManager.PaymentMnemomic;
        }

        public string GetReceiptMnenomic()
        {
            return _cashAccountManager.ReceiptMnemomic;
        }

        public IList<IndexedRangeData> GetPerformanceCharts(DateTime dtValaution)
        {
            return _performanceBuilder.Run(_userToken, dtValaution, null);
        }

        public void Dispose()
        {
            //    _connection.Close();
        //    _connection.Dispose();
        }
    }
}
