﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderCore;
using InvestmentBuilderService.Utils;

namespace InvestmentBuilderService
{
    internal class UserSession
    {
        public string UserName { get; private set; }
        public string SessionId { get; private set; }
        public DateTime ValuationDate { get; set; }
        public AccountIdentifier AccountName { get; set; }
        public ManualPrices UserPrices { get; set; }

        public UserSession(string username, string usersessionid)
        {
            UserName = username;
            SessionId = usersessionid;
            ValuationDate = DateTime.Now;
            UserPrices = new ManualPrices();
        }             
    }
}

