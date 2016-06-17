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
        public const string TestUser = "TestAuthUser1";

        protected override AuthorizationLevel GetUserAuthorizationLevel(string user, string account)
        {
            if(user == TestUser)
                return AuthorizationLevel.ADMINISTRATOR;
            return AuthorizationLevel.NONE;
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
            //var user = string.Format(@"{0}\{1}", Environment.UserDomainName, Environment.UserName);
            manager.SetUserAccountToken(TestAuthorizationManager.TestUser, _TestAccount);
            var token = manager.GetCurrentTokenForUser(TestAuthorizationManager.TestUser);
            token.AuthorizeUser(AuthorizationLevel.ADMINISTRATOR);
        }

        [TestMethod]
        public void When_Authorizing_Invalid_User()
        {
            var manager = new TestAuthorizationManager();
            var user = "dodgy geezer";
            var token = manager.GetCurrentTokenForUser(user);
            Assert.IsNull(token);
        }

    }
}
