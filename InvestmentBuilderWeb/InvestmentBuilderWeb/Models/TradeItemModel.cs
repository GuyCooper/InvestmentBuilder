using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InvestmentBuilderCore;

namespace InvestmentBuilderWeb.Models
{
    public class TradeItemModel : Stock
    { 
        public TradeItemModel()
        {
            Action = TransactionType.BUY;
        }

        public TransactionType Action { get; set; }
        public double? ManualPrice { get; set; }
    }
}