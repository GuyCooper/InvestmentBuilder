using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvestmentBuilderWeb.Models
{
    public class InvestmentSummaryModel
    {
        public string AccountName { get; set; }
        public string ReportingCurrency { get; set; }
        public DateTime ValuationDate { get; set; }
        public string TotalAssetValue { get; set; }
        public string BankBalance { get; set; }
        public string TotalAssets { get; set; }
        public string NetAssets { get; set; }
        public string ValuePerUnit { get; set; }
        public string MonthlyPnL { get; set; }
    }
}