using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;

namespace InvestmentBuilderCore
{
    public class AccountMember
    {
        public AccountMember(string name, AuthorizationLevel level)
        {
            Name = name;
            AuthLevel = level;
        }

        public string Name { get; private set; }
        public AuthorizationLevel AuthLevel { get; private set; }
        public string AuthLevelString {
              get { return AuthLevel.ToString(); }
              private set
              {
                AuthLevel = (AuthorizationLevel)Enum.Parse(typeof(AuthorizationLevel), value); 
              }
         }
    }

    public class AccountModel
    {
        private readonly IList<AccountMember> _members =
                    new List<AccountMember> ();

        public AccountModel(string name, string description, string password, 
                            string currency, string type, bool enabled, string broker,
                            IList<AccountMember> members)
        {
            Name = name;
            Description = description ?? Name;
            Password = password ?? "password";
            ReportingCurrency = currency;
            Type = type;
            Enabled = enabled;
            Broker = broker;
            AddMembers(members);
        }

        public void AddMember(string name, AuthorizationLevel authLevel)
        {
            _members.Add(new AccountMember(name, authLevel));
        }

        public void AddMembers(IList<AccountMember> members)
        {
            if (members != null)
            {
                foreach (var member in members)
                {
                    _members.Add(member);
                }
            }
        }

        public void ClearAllMembers()
        {
            _members.Clear();
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Password { get; private set; }
        public string ReportingCurrency { get; private set; }
        public string Type { get; private set; }
        public bool Enabled { get; private set; }
        public string Broker { get; private set; }
        public IList<AccountMember> Members
        {
            get
            {
                return _members;
            }
        }

        public void UpdateAccountDetails(AccountModel account)
        {
            //uopdate the accunt details for this account from the 
            //account parameter. do not updaye accunt members
            Broker = account.Broker;
            Password = account.Password;
            Type = account.Type;
        }

        [ContractInvariantMethod]
        protected void ObjectInvariantMethod()
        {
            Contract.Invariant(string.IsNullOrEmpty(Name) == false);
            Contract.Invariant(string.IsNullOrEmpty(Password) == false);
            Contract.Invariant(string.IsNullOrEmpty(ReportingCurrency) == false);
            Contract.Invariant(string.IsNullOrEmpty(Type) == false);
            Contract.Invariant(Members != null);
        }
    }
}
