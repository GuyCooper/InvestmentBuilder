using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestmentBuilder
{
    /// <summary>
    /// investment record interface
    /// </summary>
    interface IInvestment
    {
        string Name {get;}
        void DeactivateInvestment();
        void UpdateRow(DateTime valuationDate);
        void ChangeShareHolding(int holding);
        void AddNewShares(Stock stock);
        void UpdateClosingPrice();
        void UpdateDividend(double dDividend);

    }

  
    //class generates the current investment record for each stock for the current month. sets and sold stocks to inactive
    //and adds any new stocks to a new sheet
    abstract class InvestmentRecordBuilder
    {
        abstract protected IEnumerable<IInvestment> GetInvestments(DateTime dtValuationDate);
        abstract protected void CreateNewInvestment(Stock newTrade, DateTime valuationDate);

        /// <summary>
        /// update all current investments with the latest prices, update / add any new trades. Returns the previous
        /// valuation date
        /// </summary>
        /// <param name="trades"></param>
        /// <param name="cashData"></param>
        /// <param name="valuationDate"></param>
        public void BuildInvestmentRecords(Trades trades, CashAccountData cashData, DateTime valuationDate)
        {
            Console.WriteLine("building investment records...");
            var enInvestments = GetInvestments(valuationDate);
            foreach(var investment in enInvestments)
            {
                var company = investment.Name;
                if(trades.Sells.Any(x => company.Equals(x.Name, StringComparison.CurrentCultureIgnoreCase)))
                {
                    //trade sold, set to inactive
                    Console.WriteLine("company {0} sold", company);
                    investment.DeactivateInvestment();
                }                 
                else
                {
                    Console.WriteLine("updating company {0}", company);
                    //now copy the last row into a new row and update
                    investment.UpdateRow(valuationDate);
                    //update share number if it has changed
                    var trade = trades.Changed.FirstOrDefault(x => company.Equals(x.Name, StringComparison.CurrentCultureIgnoreCase));
                    if(trade != null)
                    {
                        Console.WriteLine("company share number changed {0}", company);
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
                    investment.UpdateClosingPrice();
                }
            }
            foreach(var newTrade in trades.Buys)
            {
                Console.WriteLine("adding new trade {0}", newTrade.Name);
                //new trade to add to investment record
                CreateNewInvestment(newTrade, valuationDate);               
            }
        }
    }

}
