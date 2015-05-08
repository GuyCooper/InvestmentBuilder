using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using InvestmentBuilderCore;
using System.Data;

namespace InvestmentBuilderClient.DataModel
{
    //ObservableCollection
    //BindingList

    internal class InvestmentDataModel : IDisposable
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private string _Account;
        private IDataLayer _dataLayer;
        private IClientDataInterface _clientData;

        public DateTime? LatestDate { get; set; }

        private Dictionary<string, string> _typeProcedureLookup = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase)
        {
            {"Dividend", "sp_GetActiveCompanies"},
            {"Subscription", "sp_GetAccountMembers"}
        };

        public InvestmentDataModel(IDataLayer dataLayer) 
        {
            _dataLayer = dataLayer;
            _clientData = dataLayer.ClientData;
            //var connectstr = @"Data Source=TRAVELPC\SQLEXPRESS;Initial Catalog=InvestmentBuilderTest;Integrated Security=True";
            // _connection = new SqlConnection(dataSource);
            // _connection.Open();
            // logger.Log(LogLevel.Info, "connected to datasource {0}", dataSource);
        }

        public IEnumerable<DateTime> GetValuationDates()
        {
            var dates = _clientData.GetRecentValuationDates(_Account).ToList();

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
                    return methodInfo.Invoke(_clientData, new object[] {_Account, LatestDate}) as IEnumerable<string>;
                }
            }
            return Enumerable.Empty<string>();
        }

       // public void GetCashAccountData(DateTime dtValuationDate, string side, Action<SqlDataReader> fnAddTransaction)
        public void GetCashAccountData(DateTime dtValuationDate, string side, Action<IDataReader> fnAddTransaction)
        {
            _clientData.GetCashAccountData(_Account, side, dtValuationDate, fnAddTransaction);
        }

        private void GetLatestValuationDate()
        {
            LatestDate = _clientData.GetLatestValuationDate(_Account);
        }

        public double GetBalanceInHand(DateTime dtValuation)
        {
            return _clientData.GetBalanceInHand(_Account, dtValuation);
        }

        public void SaveCashAccountData(DateTime dtValuationDate, DateTime dtTransactionDate,
                                    string type, string parameter, double amount)
        {
            _clientData.AddCashAccountData(_Account, dtValuationDate, dtTransactionDate, type,
                                                parameter, amount);
        }

        public void ReloadData(string dataSource)
        {
            _dataLayer.ConnectNewDatasource(dataSource);
            logger.Log(LogLevel.Info, "reload from datasource {0}", dataSource);
        }

        public IEnumerable<string> GetAccountNames()
        {
            return _clientData.GetAccountNames();
        }

        public bool IsExistingValuationDate(DateTime dtValuation)
        {
            return _clientData.IsExistingValuationDate(_Account, dtValuation);
        }

        public IEnumerable<string> GetAccountMembers(string account)
        {
            return _clientData.GetAccountMembers(_Account, DateTime.Today);
        }

        private void _UpdateMemberForAccount(string account, string member, bool bAdd)
        {
            _clientData.UpdateMemberForAccount(account, member, bAdd);
        }

        public void UpdateUserAccount(AccountModel account)
        {
            logger.Log(LogLevel.Info, "creating/modifying account {0}", account.Name);
            logger.Log(LogLevel.Info, "Password {0}", account.Password);
            logger.Log(LogLevel.Info, "Description {0}", account.Description);
            logger.Log(LogLevel.Info, "Reporting Currency {0}", account.ReportingCurrency);
            logger.Log(LogLevel.Info, "Account Type {0}", account.Type);
            logger.Log(LogLevel.Info, "Enabled {0}", account.Enabled);

            _clientData.CreateAccount(account);
            var existingMembers = GetAccountMembers(account.Name);
            foreach(var member in existingMembers)
            {
                if(account.Members.Contains(member, StringComparer.CurrentCultureIgnoreCase) == false)
                {
                    //remove this member
                    logger.Log(LogLevel.Info, "removing member {0} from account {1}", member, account.Name);
                    _UpdateMemberForAccount(account.Name, member, false);
                }
            }

            //now add the members
            foreach (var member in account.Members)
            {
                logger.Log(LogLevel.Info, "adding member {0} to account {1}", member, account.Name);
                _UpdateMemberForAccount(account.Name, member, true);
            }
        }

        public IEnumerable<string> GetAccountTypes()
        {
            return _clientData.GetAccountTypes();
        }

        public AccountModel GetAccountData(string account)
        {
            AccountModel data = _clientData.GetAccount(account);
            if(data != null)
            {
                data.Members = GetAccountMembers(data.Name).ToList();
            }

            return data;
        }

        public void UpdateAccountName(string account)
        {
            logger.Log(LogLevel.Info, "updating to account {0} ", account);
            _Account = account;
            GetLatestValuationDate();
        }

        public void Dispose()
        {
            //    _connection.Close();
        //    _connection.Dispose();
        }
    }
}
