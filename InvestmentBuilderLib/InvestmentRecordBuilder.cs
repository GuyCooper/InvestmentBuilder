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
        void UpdateRow(DateTime valuationDate, DateTime? previousDate);
        void ChangeShareHolding(int holding);
        void AddNewShares(Stock stock);
        void UpdateClosingPrice(double dClosing);
        void UpdateDividend(double dDividend);
        void SellShares(Stock stock);
    }
  
    //class generates the current investment record for each stock for the current month. sets and sold stocks to inactive
    //and adds any new stocks to a new sheet
    abstract class InvestmentRecordBuilder
    {
        protected Logger Log { get; private set; }

        abstract protected IEnumerable<IInvestment> GetInvestments(UserData account, DateTime dtValuationDate);
        abstract protected void CreateNewInvestment(UserData account, Stock newTrade, DateTime valuationDate, double dClosing);

        private IMarketDataService _marketDataService;

        public InvestmentRecordBuilder()
        {
            Log = LogManager.GetLogger(GetType().FullName);
            _marketDataService = ContainerManager.ResolveValue<IMarketDataService>();
        }

        /// <summary>
        /// update all current investments with the latest prices, update / add any new trades. Returns the previous
        /// valuation date
        /// </summary>
        /// <param name="trades"></param>
        /// <param name="cashData"></param>
        /// <param name="valuationDate"></param>
        public void BuildInvestmentRecords(UserData account, Trades trades, CashAccountData cashData, DateTime valuationDate, DateTime? previousValuation)
        {
            Log.Log(LogLevel.Info, "building investment records...");
            //Console.WriteLine("building investment records...");

            var aggregatedBuys = trades.Buys.AggregateStocks().ToList();
            var aggregatedSells = trades.Sells.AggregateStocks().ToList();

            var enInvestments = GetInvestments(account, previousValuation.HasValue ? previousValuation.Value : valuationDate).ToList();
            foreach(var investment in enInvestments)
            {
                var company = investment.Name;
                var sellTrade = aggregatedSells.FirstOrDefault(x => company.Equals(x.Name, StringComparison.CurrentCultureIgnoreCase));
                if(sellTrade != null)
                {
                    //trade sold, set to inactive. todo do this properly
                    Log.Log(LogLevel.Info, string.Format("company {0} sold {1} shares", company, sellTrade.Number));
                    //Console.WriteLine("company {0} sold", company);
                    //investment.DeactivateInvestment();
                    investment.SellShares(sellTrade);
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
                    trade = aggregatedBuys.FirstOrDefault(x => company.Equals(x.Name, StringComparison.CurrentCultureIgnoreCase));
                    if(trade != null)
                    {
                        investment.AddNewShares(trade);
                        //remove the trade from the trade buys
                        aggregatedBuys = aggregatedBuys.Where(x => x != trade).ToList();
                    }

                    //ifwe have the correct comapy data then calculate the closing price forthis company
                    if (investment.CompanyData != null)
                    {
                        var companyData = investment.CompanyData;
                        double dClosing;
                        if (_marketDataService.TryGetClosingPrice(companyData.Symbol, companyData.Exchange, investment.Name, companyData.Currency, account.Currency, companyData.ScalingFactor, out dClosing))
                        {
                            investment.UpdateClosingPrice(dClosing);       
                        }
                    }
                }
            }

            foreach(var newTrade in aggregatedBuys)
            {
                Log.Log(LogLevel.Info, string.Format("adding new trade {0}", newTrade.Name));
                //Console.WriteLine("adding new trade {0}", newTrade.Name);
                //new trade to add to investment record
                double dClosing;
                _marketDataService.TryGetClosingPrice(newTrade.Symbol, newTrade.Exchange, newTrade.Name, newTrade.Currency, account.Currency, newTrade.ScalingFactor, out dClosing);
                CreateNewInvestment(account, newTrade, valuationDate, dClosing);               
            }
        }
    }

}
