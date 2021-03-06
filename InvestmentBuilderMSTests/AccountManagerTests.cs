﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InvestmentBuilder;
using InvestmentBuilderCore;

namespace InvestmentBuilderMSTests
{
    [TestClass]
    public class AccountManagerTests
    {
        private static readonly UserAccountToken _userToken = new UserAccountToken(
                                            TestDataCache._testUser,
                                            TestDataCache._TestAccount,
                                            AuthorizationLevel.READ);

        [TestMethod]
        public void When_Getting_Empty_User_Accounts()
        {
            var UoT = new AccountManager(new DataLayerTest(new ClientDataEmptyInterfaceTest(),
                                                           null, null, new UserAccountEmptyInterfaceTest(), null), new TestAuthorizationManager(),
                                                           new ConfigurationSettingsTest());

            var result = UoT.GetAccountData(_userToken, TestDataCache._currentValuationDate);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void When_Getting_User_Accounts()
        {
            var UoT = new AccountManager(new DataLayerTest(new CurrentInvestmentsClientData(),
                                                           null, null, new UserAccountDataTest2(), null), new TestAuthorizationManager(),
                                                           new ConfigurationSettingsTest());

            var result = UoT.GetAccountData(_userToken, TestDataCache._currentValuationDate);

            Assert.IsNotNull(result);
            Assert.AreEqual(TestDataCache._TestAccount.Name, result.Identifier.Name);
            Assert.AreEqual(TestDataCache._TestAccount.AccountId, result.Identifier.AccountId);
            Assert.AreEqual(TestDataCache._Currency, result.ReportingCurrency);
            Assert.AreEqual(TestDataCache._TestAccountType, result.Type);
            Assert.AreEqual(1, result.Members.Count);
            Assert.AreEqual(TestDataCache._testUser, result.Members[0].Name);
            Assert.AreEqual(AuthorizationLevel.ADMINISTRATOR, result.Members[0].AuthLevel);
        }

        [TestMethod]
        public void When_creating_user_account()
        {
            var testClientInterface = new UserAccountDataTest();
            var UoT = new AccountManager(new DataLayerTest(null,
                                                           null, null, testClientInterface, null), new TestAuthorizationManager(),
                                                           new ConfigurationSettingsTest());

            var account = new AccountModel(TestDataCache._TestAccount, TestDataCache._Description,
                                         TestDataCache._Currency, TestDataCache._TestAccountType, true,
                                         TestDataCache._Broker, new List<AccountMember>{
                                             new AccountMember(TestDataCache._testUser, AuthorizationLevel.ADMINISTRATOR)
                                         });

            bool bUpdatedOk = UoT.CreateUserAccount(TestDataCache._testUser, account, TestDataCache._currentValuationDate);

            Assert.IsTrue(bUpdatedOk);

            var result = testClientInterface.GetAccountMemberDetails(_userToken, TestDataCache._currentValuationDate).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(TestDataCache._testUser, result[0].Name);
        }

        [TestMethod]
        public void When_updating_user_account()
        {
            var testClientInterface = new UserAccountDataTest();
            var UoT = new AccountManager(new DataLayerTest(null,
                                                           null, null, testClientInterface, null), new TestAuthorizationManager(),
                                                           new ConfigurationSettingsTest());

            var account = new AccountModel(TestDataCache._TestAccount, TestDataCache._Description,
                                          TestDataCache._Currency, TestDataCache._TestAccountType, true,
                                         TestDataCache._Broker, new List<AccountMember>{
                                             new AccountMember(TestDataCache._testUser, AuthorizationLevel.ADMINISTRATOR)
                                         });

            bool bUpdatedOk = UoT.CreateUserAccount(TestAuthorizationManager.TestUser, account, TestDataCache._currentValuationDate);

            Assert.IsTrue(bUpdatedOk);

            var result = testClientInterface.GetAccountMemberDetails(_userToken, TestDataCache._currentValuationDate).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(TestDataCache._testUser, result[0].Name);
        }

        [TestMethod]
        public void When_updating_user_account_members()
        {
            var testClientInterface = new UserAccountDataTest();
            var UoT = new AccountManager(new DataLayerTest(null,
                                                           null, null, testClientInterface, null), new TestAuthorizationManager(),
                                                           new ConfigurationSettingsTest());

            var account = new AccountModel(TestDataCache._TestAccount, TestDataCache._Description,
                                         TestDataCache._Currency, TestDataCache._TestAccountType, true,
                                         TestDataCache._Broker, new List<AccountMember>{
                                             new AccountMember(TestDataCache._testUser, AuthorizationLevel.ADMINISTRATOR)
                                         });

            UoT.UpdateUserAccount(TestAuthorizationManager.TestUser, account, TestDataCache._currentValuationDate);

            //now update the same account but with a different member
            account.ClearAllMembers();
            account.AddMember(TestDataCache._testUser1, AuthorizationLevel.ADMINISTRATOR);

            var bUpdatedOk = UoT.CreateUserAccount(TestAuthorizationManager.TestUser, account, TestDataCache._currentValuationDate);

            Assert.IsTrue(bUpdatedOk);

            var result = testClientInterface.GetAccountMemberDetails(_userToken, TestDataCache._currentValuationDate).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(TestDataCache._testUser1, result[0].Name);
        }

        [TestMethod]
        public void When_updating_account_with_invalid_user()
        {
            var testClientInterface = new UserAccountDataTest();
            var UoT = new AccountManager(new DataLayerTest(null,
                                                           null, null, testClientInterface, null), new TestAuthorizationManager(),
                                                           new ConfigurationSettingsTest());

            var account = new AccountModel(TestDataCache._TestAccount, TestDataCache._Description,
                                          TestDataCache._Currency, TestDataCache._TestAccountType, true,
                                         TestDataCache._Broker, new List<AccountMember>{
                                             new AccountMember(TestDataCache._dodgyUser, AuthorizationLevel.ADMINISTRATOR)
                                         });

            bool result = UoT.CreateUserAccount(TestDataCache._testUser, account, TestDataCache._currentValuationDate);
            Assert.IsFalse(result); 
        }

        [TestMethod]
        public void When_creating_invalid_user_account()
        {
            var testClientInterface = new UserAccountDataTest();
            var UoT = new AccountManager(new DataLayerTest(null,
                                                           null, null, testClientInterface, null), new TestAuthorizationManager(),
                                                           new ConfigurationSettingsTest());

            var account = new AccountModel(TestDataCache._TestAccount, TestDataCache._Description,
                                         TestDataCache._Currency, TestDataCache._TestAccountType, true,
                                         TestDataCache._Broker, new List<AccountMember>{
                                             new AccountMember(TestDataCache._testUser, AuthorizationLevel.READ)
                                         });

            bool bUpdatedOk = UoT.CreateUserAccount(TestDataCache._testUser, account, TestDataCache._currentValuationDate);

            Assert.IsFalse(bUpdatedOk);
        }

        [TestMethod]
        public void When_creating_invalid_user_account1()
        {
            var testClientInterface = new UserAccountDataTest1();
            var UoT = new AccountManager(new DataLayerTest(null,
                                                           null, null, testClientInterface, null), new TestAuthorizationManager(),
                                                           new ConfigurationSettingsTest());

            var account = new AccountModel(TestDataCache._TestAccount, TestDataCache._Description,
                                         TestDataCache._Currency, TestDataCache._TestAccountType, true,
                                         TestDataCache._Broker, new List<AccountMember>{
                                             new AccountMember(TestDataCache._testUser, AuthorizationLevel.READ)
                                         });

            bool bUpdatedOk = UoT.UpdateUserAccount(TestDataCache._testUser, account, TestDataCache._currentValuationDate);

            Assert.IsFalse(bUpdatedOk);
        }

        [TestMethod]
        public void When_creating_an_account_that_already_exists()
        {
            var testClientInterface = new UserAccountDataTest3();
            var UoT = new AccountManager(new DataLayerTest(null,
                                                           null, null, testClientInterface, null), new TestAuthorizationManager(),
                                                           new ConfigurationSettingsTest());

            var account = new AccountModel(TestDataCache._TestAccount, TestDataCache._Description,
                                         TestDataCache._Currency, TestDataCache._TestAccountType, true,
                                         TestDataCache._Broker, new List<AccountMember>{
                                             new AccountMember(TestDataCache._testUser, AuthorizationLevel.ADMINISTRATOR)
                                         });

            bool bUpdatedOk = UoT.CreateUserAccount(TestDataCache._testUser, account, TestDataCache._currentValuationDate);

            Assert.IsTrue(bUpdatedOk);
        }
    }
}
