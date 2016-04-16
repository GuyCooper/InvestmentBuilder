﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestmentBuilderCore
{
    public class AccountModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Password { get; set; }
        public string ReportingCurrency { get; set; }
        public string Type { get; set; }
        public bool Enabled { get; set; }
        public string Broker { get; set; }
        public IList<KeyValuePair<string, AuthorizationLevel>> Members { get; set; }
    }
}
