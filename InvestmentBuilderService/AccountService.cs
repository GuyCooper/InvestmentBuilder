using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderCore;
using InvestmentBuilder;

namespace InvestmentBuilderService
{
    internal class AccountService
    {
        private AccountManager _accountManager;
        IAuthorizationManager _authorizationManager;

        public AccountService(AccountManager accountManager, IAuthorizationManager authorizationManager)
        {
            _accountManager = accountManager;
            _authorizationManager = authorizationManager;
        }

        public UserAccountToken GetUserAccountToken(UserSession userSession, string selectedAccount)
        {
            UserAccountToken token = null;
            var username = userSession.UserName;
            if (username != null)
            {
                var accounts = _accountManager.GetAccountNames(username).ToList();
                token = _authorizationManager.SetUserAccountToken(username, selectedAccount ?? accounts.FirstOrDefault());
            }
            return token;
        }
    }
}
