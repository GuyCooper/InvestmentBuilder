using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace InvestmentBuilderClient.DataModel
{
    internal interface IInvestmentDataModel
    {
        IEnumerable<DateTime> GetValuationDates();
        IEnumerable<string> GetsTransactionTypes(string side);
        IEnumerable<string> GetParametersForType(string type);
        void GetCashAccountData(DateTime dtValuationDate, string side, Action<IDataReader> fnAddTransaction);
        double GetBalanceInHand(DateTime dtValuation);
        void SaveCashAccountData(DateTime dtValuationDate, DateTime dtTransactionDate,
                                    string type, string parameter, double amount);
        void ReloadData(string dataSource);
        IEnumerable<string> GetAccountNames();
        bool IsExistingValuationDate(DateTime dtValuation);
        void UpdateAccountName(string account);
        DateTime? LatestDate { get; set; }
        IEnumerable<string> GetAccountMembers(string account);
        void UpdateUserAccount(AccountModel account);
        IEnumerable<string> GetAccountTypes();
        AccountModel GetAccountData(string account);
    }
}