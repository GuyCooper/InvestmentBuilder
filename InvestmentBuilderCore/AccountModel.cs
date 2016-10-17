using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;

namespace InvestmentBuilderCore
{
    public class AccountModel
    {
        public AccountModel(string name, string description, string password, 
                            string currency, string type, bool enabled, string broker,
                            IList<KeyValuePair<string, AuthorizationLevel>> members)
        {
            Name = name;
            Description = description;
            Password = password;
            ReportingCurrency = currency;
            Type = type;
            Enabled = enabled;
            Broker = broker;
            Members = members;
        }

        public void AddMembers(IList<KeyValuePair<string, AuthorizationLevel>> members)
        {
            Members = members;
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Password { get; private set; }
        public string ReportingCurrency { get; private set; }
        public string Type { get; private set; }
        public bool Enabled { get; private set; }
        public string Broker { get; private set; }
        public IList<KeyValuePair<string, AuthorizationLevel>> Members { get; private set; }

        [ContractInvariantMethod]
        protected void ObjectInvariantMethod()
        {
            Contract.Invariant(string.IsNullOrEmpty(Name) == false);
            Contract.Invariant(string.IsNullOrEmpty(ReportingCurrency) == false);
            Contract.Invariant(string.IsNullOrEmpty(Type) == false);
        }
    }
}
