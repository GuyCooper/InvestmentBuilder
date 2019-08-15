using System.Collections.Generic;
using System.Linq;

namespace InvestmentBuilderServiceTestRunner.Requests
{
    /// <summary>
    /// CreateAccountChannelRequest tests.
    /// </summary>
    class CreateAccountChannelRequest : RequestEndpoint<CreateAccountChannelRequest.Request, CreateAccountChannelRequest.Result>
    {
        #region Internal classes

        public class Request : Dto
        {
            public Dtos.AccountIdentifier AccountName { get; set; }
            public string Description { get; set; }
            public string ReportingCurrency { get; set; }
            public string AccountType { get; set; }
            public bool Enabled { get; set; }
            public string Broker { get; set; }
            public IList<Dtos.AccountMemberDto> Members { get; set; }
        }

        public class Result : Dto
        {
            public bool Status { get; set; }
            public IEnumerable<Dtos.AccountIdentifier> AccountNames { get; set; }
        }

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public CreateAccountChannelRequest()
            : base("CREATE_ACCOUNT_REQUEST", "CREATE_ACCOUNT_RESPONSE")
        {
        }

        #region Test Methods

        /// <summary>
        /// Add a new account.
        /// </summary>
        public void AddNewAccount(ConnectionService connectionService)
        {
            LogMessage("AddNewAccount");
            var newAccount = new CreateAccountChannelRequest.Request
            {
                AccountName = new Dtos.AccountIdentifier { Name = "TestAccount" },
                Enabled = true,
                Members = new List<Dtos.AccountMemberDto>(),
                ReportingCurrency = "GBP",
                AccountType = "Personal"
            };

            newAccount.Members.Add(new Dtos.AccountMemberDto { Name = "user@test.com", Permission = "ADMINISTRATOR" });

            var result = SendRequest(newAccount, connectionService);
            Assert.IsTrue(result.Success, "AddNewAccount", result.Error);
            Assert.AreEqual(1, result.Result.AccountNames.Count(), "User Accounts");
            Assert.AreEqual("TestAccount", result.Result.AccountNames.First().Name, "USer Account is TestAccount");
        }

        #endregion
    }
}
