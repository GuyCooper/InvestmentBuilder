using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketDataServices;
using NLog;
using InvestmentBuilderCore;

namespace InvestmentBuilder
{
    //class generates the current investment record for each stock for the current month. sets and sold stocks to inactive
    //and adds any new stocks to a new sheet
    internal class InvestmentRecordBuilder
    {
        protected Logger Log { get; private set; }
        private IMarketDataService _marketDataService;
        private IInvestmentRecordInterface _investmentRecordData;

        public InvestmentRecordBuilder(IMarketDataService marketDataService, IInvestmentRecordInterface investmentRecordData)
        {
            _investmentRecordData = investmentRecordData;
            _marketDataService = marketDataService;
            Log = LogManager.GetLogger(GetType().FullName);
        }

        protected IEnumerable<IInvestment> GetInvestments(string account, DateTime dtValuationDate)
        {
            var companies = _investmentRecordData.GetInvestments(account, dtValuationDate).ToList(); 
            return companies.Select(c => new InvestmentData(account, c, _investmentRecordData));
        }

        protected void CreateNewInvestment(string account, Stock newTrade, DateTime valuationDate, double dClosing)
        {
            _investmentRecordData.CreateNewInvestment(account, newTrade.Name, newTrade.Symbol, newTrade.Currency,
                                                      newTrade.Quantity, newTrade.ScalingFactor, newTrade.TotalCost, dClosing, newTrade.Exchange,
                                                      valuationDate);
        }

        /// <summary>
        /// update all current investments with the latest prices, update / add any new trades. Returns the previous
        /// valuation date
        /// </summary>
        /// <param name="trades"></param>
        /// <param name="cashData"></param>
        /// <param name="valuationDate"></param>
        public void BuildInvestmentRecords(UserAccountData account, Trades trades, CashAccountData cashData, DateTime valuationDate, DateTime? previousValuation)
        {
            Log.Log(LogLevel.Info, "building investment records...");
            //Console.WriteLine("building investment records...");

            var aggregatedBuys = trades.Buys.AggregateStocks().ToList();
            var aggregatedSells = trades.Sells.AggregateStocks().ToList();

            var enInvestments = GetInvestments(account.Name, previousValuation.HasValue ? previousValuation.Value : valuationDate).ToList();
            foreach(var investment in enInvestments)
            {
                if(previousValuation.HasValue == false)
                {
                    throw new ApplicationException(string.Format("BuildInvestmentRecords: no previous valuation date for {0}. please investigate!!!", account));
                }

                var company = investment.Name;
                //Console.WriteLine("updating company {0}", company);
                //now copy the last row into a new row and update
                investment.UpdateRow(valuationDate, previousValuation.Value);
                var sellTrade = aggregatedSells.FirstOrDefault(x => company.Equals(x.Name, StringComparison.CurrentCultureIgnoreCase));
                if(sellTrade != null)
                {
                    //trade sold, set to inactive. todo do this properly
                    Log.Log(LogLevel.Info, string.Format("company {0} sold {1} shares", company, sellTrade.Quantity));
                    //Console.WriteLine("company {0} sold", company);
                    //investment.DeactivateInvestment();
                    investment.SellShares(valuationDate, sellTrade);
                }                 
  
                //update share number if it has changed
                var trade = trades.Changed != null ? trades.Changed.FirstOrDefault(x => company.Equals(x.Name, StringComparison.CurrentCultureIgnoreCase)) : null;
                if(trade != null)
                {
                    Log.Log(LogLevel.Info, string.Format("company share number changed {0}", company));
                    //Console.WriteLine("company share number changed {0}", company);
                    investment.ChangeShareHolding(valuationDate, trade.Quantity);
                }

                //now update this stock if more shres have been brought
                trade = aggregatedBuys.FirstOrDefault(x => company.Equals(x.Name, StringComparison.CurrentCultureIgnoreCase));
                if(trade != null)
                {
                    investment.AddNewShares(valuationDate, trade);
                    //remove the trade from the trade buys
                    aggregatedBuys = aggregatedBuys.Where(x => x != trade).ToList();
                }

                //update any dividend 
                double dDividend;
                if (cashData.Dividends.TryGetValue(company, out dDividend))
                {
                    investment.UpdateDividend(valuationDate, dDividend);
                }
                //ifwe have the correct comapy data then calculate the closing price forthis company
                if (investment.CompanyData != null)
                {
                    var companyData = investment.CompanyData;
                    double dClosing;
                    if (_marketDataService.TryGetClosingPrice(companyData.Symbol, companyData.Exchange, investment.Name, companyData.Currency, account.Currency, companyData.ScalingFactor, out dClosing))
                    {
                        investment.UpdateClosingPrice(valuationDate, dClosing);       
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
                CreateNewInvestment(account.Name, newTrade, valuationDate, dClosing);               
            }
        }
    }

}
