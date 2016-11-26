using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderCore;
using NLog;

namespace InvestmentBuilder
{
    public sealed class AccountManager
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private IUserAccountInterface _accountData;
        private IAuthorizationManager _authorizationManager;

        public AccountManager(IDataLayer dataLayer, IAuthorizationManager authorizationManager)
        {
            _accountData = dataLayer.UserAccountData;
            _authorizationManager = authorizationManager;
        }

        public AccountModel GetAccountData(UserAccountToken userToken, DateTime dtValuationDate)
        {
            AccountModel data = _accountData.GetAccount(userToken);
            if (data != null)
            {
                data.AddMembers(GetAccountMembers(userToken, dtValuationDate).ToList());
            }
            return data;
        }

        public bool UpdateUserAccount(string user, AccountModel account, DateTime dtValuationDate)
        {
            logger.Log(LogLevel.Info, "creating/modifying account {0}", account.Name);
            logger.Log(LogLevel.Info, "Password {0}", account.Password);
            logger.Log(LogLevel.Info, "Description {0}", account.Description);
            logger.Log(LogLevel.Info, "Reporting Currency {0}", account.ReportingCurrency);
            logger.Log(LogLevel.Info, "Account Type {0}", account.Type);
            logger.Log(LogLevel.Info, "Enabled {0}", account.Enabled);
            logger.Log(LogLevel.Info, "Broker {0}", account.Broker);

            //there must be at least 1 member with administrator rights in the account
            if(_ValidateAccount(account) == false)
            {
                return false;
            }

            var tmpToken = _authorizationManager.GetUserAccountToken(user, account.Name);

            _accountData.CreateAccount(tmpToken, account);
            var existingMembers = _accountData.GetAccountMembers(tmpToken, dtValuationDate);
            //GetAccountMembers(tmpToken).ToList();
            foreach (var member in existingMembers)
            {
                if (account.Members.Where(x => string.Equals(x.Name, member, StringComparison.InvariantCultureIgnoreCase)).Count() == 0)
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
                _UpdateMemberForAccount(tmpToken, member.Name, member.AuthLevel, true);
            }

            return true;
        }

        public IEnumerable<AccountMember> GetAccountMembers(UserAccountToken token, DateTime dtValuationDate)
        {
            return _accountData.GetAccountMemberDetails(token, dtValuationDate);
        }

        public IEnumerable<string> GetAccountNames(string user)
        {
            return _accountData.GetAccountNames(user, true);
        }

        private void _UpdateMemberForAccount(UserAccountToken token, string member, AuthorizationLevel level, bool bAdd)
        {
            _accountData.UpdateMemberForAccount(token, member, level, bAdd);
        }

        private bool _ValidateAccount(AccountModel account)
        {
            //account must have at least one administrator and no user can be a 
            //member of 5 accounts
            bool hasAdmin = false;
            foreach (var member in account.Members)
            {
                var userAccounts = _accountData.GetAccountNames(member.Name, false).ToList();
                if(userAccounts.Count >= 5)
                {
                    logger.Log(LogLevel.Error, "user {0} is already a member of 5 groups!!");
                    return false;
                }
                hasAdmin |= member.AuthLevel == AuthorizationLevel.ADMINISTRATOR;
            }
            if(hasAdmin == false)
            {
                logger.Log(LogLevel.Error, "account {0} must have at least one administrator");
            }

            return hasAdmin;
        }
    }
}
