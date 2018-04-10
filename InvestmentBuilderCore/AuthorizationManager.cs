using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
namespace InvestmentBuilderCore
{
    public enum AuthorizationLevel
    {
        NONE = 0,
        READ,
        UPDATE,
        ADMINISTRATOR
    }

    /// <summary>
    /// UserAccountToken class defines a user authorization level for an account
    /// </summary>
    public class UserAccountToken
    {
        public string User { get; private set; }
        public string Account { get; private set; }

        private AuthorizationLevel _authorization;

        public UserAccountToken(string user, string account,AuthorizationLevel authorization)
        {
            User = user;
            Account = account;
            _authorization = authorization;
        }

        public void AuthorizeUser(AuthorizationLevel level)
        {
            if(_authorization < level)
            {
                throw new UnauthorizedAccessException("User does not have permission for this action on this account");
            }
        }

        public void UpdateAccount(string account)
        {
            if(string.IsNullOrEmpty(account) == false)
            {
                Account = account;
            }
        }

        [ContractInvariantMethod]
        protected void ObjectInvariantMethod()
        {
            Contract.Invariant(string.IsNullOrEmpty(User) == false);
            Contract.Invariant(string.IsNullOrEmpty(Account) == false);
        }
    }
    /// <summary>
    /// interface defines an authorization manager. user can have any of the levels in
    /// authorization level. lowest is none highest is administrator. each level inherits
    /// the permissions of all the previous ones.i.e. adminstrator inherits all other 
    /// permissions
    /// </summary>
    public interface IAuthorizationManager
    {
        UserAccountToken GetUserAccountToken(string user, string account);
        UserAccountToken SetUserAccountToken(string user, string account);
        UserAccountToken GetCurrentTokenForUser(string user);
    }

    public abstract class AuthorizationManager : IAuthorizationManager
    {
        //private static string GLOBAL_ADMINISTRATOR = "GLOBAL ADMINISTRATOR";
        //private static string GLOBAL_ACCOUNT = "GLOBAL ACCOUNT";

        //private static UserAccountToken GlobalAdministrator =
        //    new UserAccountToken(GLOBAL_ADMINISTRATOR, GLOBAL_ACCOUNT, AuthorizationLevel.ADMINISTRATOR);
        private Dictionary<string, UserAccountToken> _userTokenlookup;

        public AuthorizationManager()
        {
            _userTokenlookup = new Dictionary<string, UserAccountToken>();
        }

        public UserAccountToken GetUserAccountToken(string user, string account)
        {
            if(IsGlobalAdministrator(user) == true)
            {
                return new UserAccountToken(user, account, AuthorizationLevel.ADMINISTRATOR);
            }

            if (string.IsNullOrEmpty(account) == false)
            {
                return new UserAccountToken(user, account,
                    GetUserAuthorizationLevel(user, account));
            }

            return new UserAccountToken(user, account, AuthorizationLevel.NONE);
        }

        /// <summary>
        /// each user is only ever allowed a single useraccount token at any time
        /// </summary>
        /// <param name="user"></param>
        /// <param name="account"></param>
        public UserAccountToken SetUserAccountToken(string user, string account)
        {
            UserAccountToken existingToken;
            if (IsGlobalAdministrator(user) || account == null)
            {
                _userTokenlookup.TryGetValue(user, out existingToken);
                if(existingToken != null)
                {
                    existingToken.UpdateAccount(account);
                }
            }
            else
                existingToken = _userTokenlookup.Values.FirstOrDefault(t => t.Account == account && t.User == user);

            if(existingToken == null)
            {
                existingToken = GetUserAccountToken(user, account);
                _userTokenlookup.Add(user, existingToken);
            }
            return existingToken;
        }

        public UserAccountToken GetCurrentTokenForUser(string user)
        {
            UserAccountToken token;
            if(_userTokenlookup.TryGetValue(user, out token) == true)
            {
                return token;
            }
            return null;
        }

        protected abstract AuthorizationLevel GetUserAuthorizationLevel(string user, string account);

        protected abstract bool IsGlobalAdministrator(string user);
    }
}
