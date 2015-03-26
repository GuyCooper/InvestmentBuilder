using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestmentBuilderClient.DataModel
{
    internal class AccountModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Password { get; set; }
        public string ReportingCurrency { get; set; }
        public string Type { get; set; }
        public bool   Enabled { get; set; }
        public IList<string> Members { get; set; }
    }
}
