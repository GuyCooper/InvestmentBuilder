using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InvestmentBuilderCore;

namespace InvestmentBuilderMSTests
{
    internal class TestAuthorizationManager : AuthorizationManager
    {
        protected override AuthorizationLevel GetUserAuthorizationLevel(string user, string account)
        {
            return AuthorizationLevel.ADMINISTRATOR;
        }

        protected override bool IsGlobalAdministrator(string user)
        {
            return false;
        }
    }

    [TestClass]
    public class AuthorisationTests
    {
        private static string _TestAccount = "Guy SIPP";

        [TestMethod]
        public void When_Authorizing_Windows_User()
        {
            var manager = new TestAuthorizationManager();
            var user = string.Format(@"{0}\{1}", Environment.UserDomainName, Environment.UserName);
            manager.SetUserAccountToken(user, _TestAccount);
            var token = manager.GetCurrentTokenForUser(user);
            token.AuthorizeUser(AuthorizationLevel.ADMINISTRATOR);
        }
    }
}
