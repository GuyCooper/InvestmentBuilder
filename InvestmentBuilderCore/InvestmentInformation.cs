using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestmentBuilderCore
{
    public class InvestmentInformation
    {
        public string Symbol { get; set; }
        public string Exchange { get; set; }
        public string Currency { get; set; }
        public double ScalingFactor { get; set; }
    }

    public class CashAccountData
    {
        public CashAccountData()
        {
            Dividends = new Dictionary<string, double>();
        }
        public Dictionary<string, double> Dividends { get; private set; }
        public double BankBalance { get; set; }
    }

    public class CompanyData
    {
        public string sName;
        public DateTime? dtLastBrought;
        public double iNumberOfShares;
        public double dAveragePricePaid;
        public double dTotalCost;
        public double dSharePrice;
        public double dNetSellingValue;
        public double dProfitLoss;
        public double dMonthChange;
        public double dMonthChangeRatio;
        public double dDividend;
    }

    public class UserAccountData
    {
        public string Name { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
    }

    public class HistoricalData
    {
        public DateTime Date { get; set; }
        public double Price { get; set; }
    }

}
