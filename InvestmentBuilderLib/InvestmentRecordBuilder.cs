using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketDataServices;
using NLog;

namespace InvestmentBuilder
{
    /// <summary>
    /// investment record interface
    /// </summary>
    interface IInvestment
    {
        string Name {get;}
        CompanyInformation CompanyData {get;}
        void DeactivateInvestment();
        void UpdateRow(DateTime valuationDate, DateTime? previousDate);
        void ChangeShareHolding(int holding);
        void AddNewShares(Stock stock);
        void UpdateClosingPrice(double dClosing);
        void UpdateDividend(double dDividend);
    }
  
    //class generates the current investment record for each stock for the current month. sets and sold stocks to inactive
    //and adds any new stocks to a new sheet
    abstract class InvestmentRecordBuilder
    {
        protected Logger Log { get; private set; }

        abstract protected IEnumerable<IInvestment> GetInvestments(string account, DateTime dtValuationDate);
        abstract protected void CreateNewInvestment(string account, Stock newTrade, DateTime valuationDate, double dClosing);

        public InvestmentRecordBuilder()
        {
            Log = LogManager.GetLogger(GetType().FullName);
        }

        /// <summary>
        /// update all current investments with the latest prices, update / add any new trades. Returns the previous
        /// valuation date
        /// </summary>
        /// <param name="trades"></param>
        /// <param name="cashData"></param>
        /// <param name="valuationDate"></param>
        public void BuildInvestmentRecords(string account, Trades trades, CashAccountData cashData, DateTime valuationDate, DateTime? previousValuation)
        {
            Log.Log(LogLevel.Info, "building investment records...");
            //Console.WriteLine("building investment records...");
            var enInvestments = GetInvestments(account, valuationDate).ToList();
            foreach(var investment in enInvestments)
            {
                var company = investment.Name;
                if(trades.Sells.Any(x => company.Equals(x.Name, StringComparison.CurrentCultureIgnoreCase)))
                {
                    //trade sold, set to inactive
                    Log.Log(LogLevel.Info, string.Format("company {0} sold", company));
                    //Console.WriteLine("company {0} sold", company);
                    investment.DeactivateInvestment();
                }                 
                else
                {
                    Log.Log(LogLevel.Info, string.Format("updating company {0}", company));
                    //Console.WriteLine("updating company {0}", company);
                    //now copy the last row into a new row and update
                    investment.UpdateRow(valuationDate, previousValuation);
                    //update share number if it has changed
                    var trade = trades.Changed.FirstOrDefault(x => company.Equals(x.Name, StringComparison.CurrentCultureIgnoreCase));
                    if(trade != null)
                    {
                        Log.Log(LogLevel.Info, string.Format("company share number changed {0}", company));
                        //Console.WriteLine("company share number changed {0}", company);
                        investment.ChangeShareHolding(trade.Number);
                    }
                    //update any dividend 
                    double dDividend;
                    if (cashData.Dividends.TryGetValue(company, out dDividend))
                    {
                        investment.UpdateDividend(dDividend);
                    }
                    //now update this stock if more shres have been brought
                    trade = trades.Buys.FirstOrDefault(x => company.Equals(x.Name, StringComparison.CurrentCultureIgnoreCase));
                    if(trade != null)
                    {
                        investment.AddNewShares(trade);
                        //remove the trade from the trade buys
                        trades.Buys = trades.Buys.Where(x => x != trade).ToArray();
                    }

                    //ifwe have the correct comapy data then calculate the closing price forthis company
                    if (investment.CompanyData != null)
                    {
                        var companyData = investment.CompanyData;
                        double dClosing;
                        if (MarketDataService.TryGetClosingPrice(companyData.Symbol, investment.Name, companyData.Currency, companyData.ScalingFactor, out dClosing))
                        {
                            investment.UpdateClosingPrice(dClosing);       
                        }
                    }
                }
            }
            foreach(var newTrade in trades.Buys)
            {
                Log.Log(LogLevel.Info, string.Format("adding new trade {0}", newTrade.Name));
                //Console.WriteLine("adding new trade {0}", newTrade.Name);
                //new trade to add to investment record
                double dClosing;
                MarketDataService.TryGetClosingPrice(newTrade.Name, newTrade.Symbol, newTrade.Currency, newTrade.ScalingFactor, out dClosing);
                CreateNewInvestment(account, newTrade, valuationDate, dClosing);               
            }
        }
    }

}
