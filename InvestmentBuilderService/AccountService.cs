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
        private readonly IAuthorizationManager _authorizationManager;
        private readonly AccountManager _accountManager;

        public AccountService(AccountManager accountManager, IAuthorizationManager authorizationManager)
        {
            _authorizationManager = authorizationManager;
            _accountManager = accountManager;
        }

        public UserAccountToken GetUserAccountToken(UserSession userSession, string selectedAccount)
        {
            UserAccountToken token = null;
            var username = userSession.UserName;
            if (username != null)
            {
                token = _authorizationManager.SetUserAccountToken(username, selectedAccount ?? userSession.AccountName);
            }
            return token;
        }

        public IEnumerable<string> GetAccountsForUser(UserSession session)
        {
            return _accountManager.GetAccountNames(session.UserName);
        }
    }
}
