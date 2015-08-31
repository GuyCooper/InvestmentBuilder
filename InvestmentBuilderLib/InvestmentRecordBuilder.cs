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

        private IEnumerable<IInvestment> _GetInvestments(string account, DateTime dtValuationDate)
        {
            var companies = _investmentRecordData.GetInvestments(account, dtValuationDate).ToList(); 
            return companies.Select(c => new InvestmentData(account, c, _investmentRecordData));
        }

        private void _CreateNewInvestment(string account, Stock newTrade, DateTime valuationDate, double dClosing)
        {
            _investmentRecordData.CreateNewInvestment(account, newTrade.Name, newTrade.Symbol, newTrade.Currency,
                                                      newTrade.Quantity, newTrade.ScalingFactor, newTrade.TotalCost, dClosing, newTrade.Exchange,
                                                      valuationDate);
        }

        private double _GetNetSellingValue(double dSharesHeld, double dPrice)
        {
            double dGrossValue = dSharesHeld * dPrice;
            if (dGrossValue > 750d)
                return dGrossValue - (dGrossValue * 0.01);
            return dGrossValue - 7.5d;
        }

        private IEnumerable<CompanyData> _GetCompanyDataImpl(UserAccountData account, DateTime dtValuationDate, bool bUpdatePrice, ManualPrices manualPrices )
        {
            var investments = _investmentRecordData.GetInvestmentRecordData(account.Name, dtValuationDate).ToList();
            foreach (var investment in investments)
            {
                if (bUpdatePrice == true)
                {
                    double dClosing;
                    var companyData = _investmentRecordData.GetInvestmentDetails(investment.Name);
                    if(_tryGetClosingPrice(companyData.Symbol, 
                                           companyData.Exchange, 
                                           investment.Name, 
                                           companyData.Currency, 
                                           account.Currency, 
                                           companyData.ScalingFactor, 
                                           manualPrices, 
                                           out dClosing) == true)
                    {
                        investment.SharePrice = dClosing;
                    }
                }
                investment.NetSellingValue = _GetNetSellingValue(investment.Quantity, investment.SharePrice);
            }
            return investments;
        }

        private void _updateMonthlyData(CompanyData currentData, CompanyData previousData)
        {
            currentData.MonthChange = currentData.NetSellingValue - previousData.NetSellingValue;
            currentData.MonthChangeRatio = currentData.MonthChange / previousData.NetSellingValue * 100;
        }

        private void _DeactivateInvestment(string investment, string account)
        {
            _investmentRecordData.DeactivateInvestment(account, investment);
        }

        /// <summary>
        /// update all current investments with the latest prices, update / add any new trades. Returns the previous
        /// valuation date. updates to the persistence layer.
        /// </summary>
        /// <param name="trades"></param>
        /// <param name="cashData"></param>
        /// <param name="valuationDate"></param>
        public void UpdateInvestmentRecords(UserAccountData account, Trades trades, CashAccountData cashData, DateTime valuationDate, DateTime? previousValuation)
        {
            Log.Log(LogLevel.Info, "building investment records...");
            //Console.WriteLine("building investment records...");

            var aggregatedBuys = trades.Buys.AggregateStocks().ToList();
            var aggregatedSells = trades.Sells.AggregateStocks().ToList();

            var enInvestments = _GetInvestments(account.Name, previousValuation.HasValue ? previousValuation.Value : valuationDate).ToList();
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
                _CreateNewInvestment(account.Name, newTrade, valuationDate, dClosing);               
            }
        }

        private bool _tryGetClosingPrice(string symbol, string exchange, string name, string currency, string accountCurrency,double scalingFactor, ManualPrices manualPrices, out double dPrice)
        {
            bool bOk = false;
            if((manualPrices != null)&&( manualPrices.ContainsKey(name) == true))
            {
                dPrice = manualPrices[name];
                bOk = true;
            }
            else
            {
                bOk = _marketDataService.TryGetClosingPrice(symbol, exchange, name, currency, accountCurrency, scalingFactor, out dPrice);
            }
           
            return bOk;
        }

        /// <summary>
        /// refactored method for updating investment record
        /// </summary>
        /// <param name="account"></param>
        /// <param name="trades"></param>
        /// <param name="cashData"></param>
        /// <param name="valuationDate"></param>
        /// <param name="previousValuation"></param>
        public void UpdateInvestmentRecordsNew(UserAccountData account, Trades trades, CashAccountData cashData, DateTime valuationDate, ManualPrices manualPrices)
        {
            Log.Log(LogLevel.Info, "building investment records...");
            //Console.WriteLine("building investment records...");

            var aggregatedBuys = trades.Buys.AggregateStocks().ToList();
            var aggregatedSells = trades.Sells.AggregateStocks().ToList();

            var previousValuation = _investmentRecordData.GetLatestRecordInvestmentValuationDate(account.Name);
            //
            var enInvestments = _GetInvestments(account.Name, previousValuation.HasValue ? previousValuation.Value : valuationDate).ToList();
            foreach (var investment in enInvestments)
            {
                if (previousValuation.HasValue == false)
                {
                    throw new ApplicationException(string.Format("BuildInvestmentRecords: no previous valuation date for {0}. please investigate!!!", account));
                }

                var company = investment.Name;
                //Console.WriteLine("updating company {0}", company);
                //now copy the last row into a new row and update
                investment.UpdateRow(valuationDate, previousValuation.Value);
                var sellTrade = aggregatedSells.FirstOrDefault(x => company.Equals(x.Name, StringComparison.CurrentCultureIgnoreCase));
                if (sellTrade != null)
                {
                    //trade sold, set to inactive. todo do this properly
                    Log.Log(LogLevel.Info, string.Format("company {0} sold {1} shares", company, sellTrade.Quantity));
                    //Console.WriteLine("company {0} sold", company);
                    //investment.DeactivateInvestment();
                    investment.SellShares(valuationDate, sellTrade);
                }

                //update share number if it has changed
                var trade = trades.Changed != null ? trades.Changed.FirstOrDefault(x => company.Equals(x.Name, StringComparison.CurrentCultureIgnoreCase)) : null;
                if (trade != null)
                {
                    Log.Log(LogLevel.Info, string.Format("company share number changed {0}", company));
                    //Console.WriteLine("company share number changed {0}", company);
                    investment.ChangeShareHolding(valuationDate, trade.Quantity);
                }

                //now update this stock if more shres have been brought
                trade = aggregatedBuys.FirstOrDefault(x => company.Equals(x.Name, StringComparison.CurrentCultureIgnoreCase));
                if (trade != null)
                {
                    investment.AddNewShares(valuationDate, trade);
                    //remove the trade from the trade buys
                    aggregatedBuys = aggregatedBuys.Where(x => x != trade).ToList();
                }

                //update any dividend 
                double dDividend;
                if (cashData!= null && cashData.Dividends.TryGetValue(company, out dDividend))
                {
                    investment.UpdateDividend(valuationDate, dDividend);
                }
                //if we have the correct comapy data then calculate the closing price forthis company
                double dPrice = 0d;
                var companyInfo = investment.CompanyData;
                if (companyInfo != null && _tryGetClosingPrice(companyInfo.Symbol, companyInfo.Exchange,investment.Name, companyInfo.Currency, account.Currency, companyInfo.ScalingFactor, manualPrices, out dPrice))
                {
                    investment.UpdateClosingPrice(valuationDate, dPrice);
                }
            }

            foreach (var newTrade in aggregatedBuys)
            {
                Log.Log(LogLevel.Info, string.Format("adding new trade {0}", newTrade.Name));
                //Console.WriteLine("adding new trade {0}", newTrade.Name);
                //new trade to add to investment record
                double dClosing;
                _tryGetClosingPrice(newTrade.Symbol,
                                    newTrade.Exchange,
                                    newTrade.Name,
                                    newTrade.Currency,
                                    account.Currency,
                                    newTrade.ScalingFactor,
                                    manualPrices, 
                                    out dClosing);

                //_marketDataService.TryGetClosingPrice(newTrade.Symbol, newTrade.Exchange, newTrade.Name, newTrade.Currency, account.Currency, newTrade.ScalingFactor, out dClosing);
                _CreateNewInvestment(account.Name, newTrade, valuationDate, dClosing);
            }
        }

        private IEnumerable<CompanyData> GetInvestmentRecordsImpl(UserAccountData account, DateTime dtValuationDate, DateTime? dtPreviousValuationDate, bool bSnapshot, ManualPrices manualPrices)
        {
            var lstCurrentData = _GetCompanyDataImpl(account, dtValuationDate, bSnapshot, manualPrices).ToList();
            var lstPreviousData = dtPreviousValuationDate.HasValue ? _GetCompanyDataImpl(account, dtPreviousValuationDate.Value, false, null).ToList() : new List<CompanyData>();
            foreach (var company in lstCurrentData)
            {
                var previousData = lstPreviousData.Find(c => c.Name == company.Name);
                if (previousData != null)
                {
                    _updateMonthlyData(company, previousData);
                }
                company.ProfitLoss = company.NetSellingValue - company.TotalCost;
                if (bSnapshot == false && company.Quantity == 0)
                {
                    _DeactivateInvestment(company.Name, account.Name);
                }
            }
            return lstCurrentData;
        }
        /// <summary>
        /// this method returns the current investment records from persistence layer
        /// </summary>
        /// <param name="account"></param>
        /// <param name="dtValuationDate"></param>
        /// <param name="dtPreviousValuationDate"></param>
        /// <returns></returns>
        public IEnumerable<CompanyData> GetInvestmentRecords(UserAccountData account, DateTime dtValuationDate, DateTime? dtPreviousValuationDate)
        {
            //dtPreviousValuationDate parameteris the previous valuation date.we need to extract the previous record
            //valuation date from this to retrieve the correct previous record data from the database
            DateTime? dtPreviousRecordValuationDate = dtPreviousValuationDate.HasValue ? _investmentRecordData.GetPreviousRecordInvestmentValuationDate(account.Name, dtPreviousValuationDate.Value) : null;
            return GetInvestmentRecordsImpl(account, dtValuationDate, dtPreviousRecordValuationDate, false, null);
        }

        /// <summary>
        /// This method builds a snapshot of the current investment records updating the current prices
        /// but NOT persisting to the database. the report is generated from the last known valuation date
        /// as this snapshot does notyet exist in the database
        /// </summary>
        /// <param name="account"></param>
        /// <param name="dtValuationDate"></param>
        /// <param name="dtPreviousValuationDate"></param>
        /// <returns></returns>
        public IEnumerable<CompanyData> GetInvestmentRecordSnapshot(UserAccountData account, ManualPrices manualPrices)
        {
            DateTime? dtPreviousValuationDate = _investmentRecordData.GetLatestRecordInvestmentValuationDate(account.Name);
            if (dtPreviousValuationDate.HasValue) //
                return GetInvestmentRecordsImpl(account, dtPreviousValuationDate.Value, dtPreviousValuationDate, true, manualPrices);

            //if there is no last known valuation date for this account then just return an empty report
            return new List<CompanyData>(); 
        }
    
        public DateTime? GetLatestRecordValuationDate(string account)
        {
            return _investmentRecordData.GetLatestRecordInvestmentValuationDate(account);
        }
    }
}
