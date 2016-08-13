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
    public interface IInvestmentRecordDataManager
    {
        bool UpdateInvestmentRecords(UserAccountToken userToken, UserAccountData account, Trades trades, CashAccountData cashData, DateTime valuationDate, ManualPrices manualPrices);
        IEnumerable<CompanyData> GetInvestmentRecords(UserAccountToken userToken, UserAccountData account, DateTime dtValuationDate, DateTime? dtPreviousValuationDate, ManualPrices manualPrices, bool bSnapshot);
        IEnumerable<CompanyData> GetInvestmentRecordSnapshot(UserAccountToken userToken, UserAccountData account, ManualPrices manualPrices);
        DateTime? GetLatestRecordValuationDate(UserAccountToken userToken);
    }
        
    //class generates the current investment record for each stock for the current month. sets and sold stocks to inactive
    //and adds any new stocks to a new sheet
    public class InvestmentRecordBuilder : IInvestmentRecordDataManager
    {        
        protected Logger Log { get; private set; }
        private readonly IMarketDataService _marketDataService;
        private readonly IInvestmentRecordInterface _investmentRecordData;
        private readonly BrokerManager _brokerManager;

        public InvestmentRecordBuilder(IMarketDataService marketDataService, IDataLayer datalayer, BrokerManager brokerManager)
        {
            _investmentRecordData = datalayer.InvestmentRecordData;
            _marketDataService = marketDataService;
            _brokerManager = brokerManager;
            Log = LogManager.GetLogger(GetType().FullName);
        }

        private IEnumerable<IInvestmentRecordData> _GetInvestments(UserAccountToken userToken, DateTime dtValuationDate)
        {
            var companies = _investmentRecordData.GetInvestments(userToken, dtValuationDate).ToList(); 
            return companies.Select(c => new InvestmentData(userToken, c.Key, c.Value, _investmentRecordData));
        }

        private void _CreateNewInvestment(UserAccountToken userToken, Stock newTrade, DateTime valuationDate, double dClosing)
        {
            _investmentRecordData.CreateNewInvestment(userToken, newTrade.Name, newTrade.Symbol, newTrade.Currency,
                                                      newTrade.Quantity, newTrade.ScalingFactor, newTrade.TotalCost, dClosing, newTrade.Exchange,
                                                      valuationDate);
        }

        private IEnumerable<CompanyData> _GetCompanyDataImpl(UserAccountToken userToken, UserAccountData account, DateTime dtValuationDate, bool bUpdatePrice, ManualPrices manualPrices )
        {
            var investments = _investmentRecordData.GetInvestmentRecordData(userToken, dtValuationDate).ToList();
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

                investment.NetSellingValue = _brokerManager.GetNetSellingValue(account.Broker, investment.Quantity, investment.SharePrice);
            }
            return investments;
        }

        private void _updateMonthlyData(CompanyData currentData, CompanyData previousData, Trades trades)
        {
            //difference between current selling value and previous selling value, must also
            //include any transactions that have been completed this month
            double dMonthChange = currentData.NetSellingValue;
            if (trades != null)
            {
                dMonthChange -= trades.Buys.Where(x => x.Name == currentData.Name).Sum(x => x.TotalCost);
                dMonthChange += trades.Sells.Where(x => x.Name == currentData.Name).Sum(x => x.TotalCost);
            }
            dMonthChange -= previousData.NetSellingValue;
            currentData.MonthChange = dMonthChange;
            currentData.MonthChangeRatio = currentData.MonthChange / previousData.NetSellingValue * 100;
        }

        private void _DeactivateInvestment(string investment, UserAccountToken userToken)
        {
            _investmentRecordData.DeactivateInvestment(userToken, investment);
        }

        private bool _tryGetClosingPrice(string symbol, string exchange, string name, string currency, string accountCurrency,double scalingFactor, ManualPrices manualPrices, out double dPrice)
        {
            double? dManualPrice = null;
            if((manualPrices != null)&&( manualPrices.ContainsKey(name) == true))
            {
                dManualPrice = manualPrices[name];
            }
            return _marketDataService.TryGetClosingPrice(symbol, exchange, name, currency, accountCurrency, dManualPrice, out dPrice);
        }

        /// <summary>
        /// refactored method for updating investment record
        /// this method rolls all the investments in the investment record
        /// table and updates the trades
        /// </summary>
        /// <param name="account"></param>
        /// <param name="trades"></param>
        /// <param name="cashData"></param>
        /// <param name="valuationDate"></param>
        /// <param name="previousValuation"></param>
        public bool UpdateInvestmentRecords(UserAccountToken userToken, UserAccountData account, Trades trades, CashAccountData cashData, DateTime valuationDate, ManualPrices manualPrices)
        {
            Log.Log(LogLevel.Info, "building investment records...");
            //Console.WriteLine("building investment records...");

            var aggregatedBuys = trades != null ? trades.Buys.AggregateStocks().ToList() : Enumerable.Empty<Stock>();
            var aggregatedSells = trades != null ? trades.Sells.AggregateStocks().ToList() : Enumerable.Empty<Stock>();

            var previousValuation = _investmentRecordData.GetLatestRecordInvestmentValuationDate(userToken);
            //
            var enInvestments = _GetInvestments(userToken, previousValuation.HasValue ? previousValuation.Value : valuationDate).ToList();

            bool bValidationFailed = false;
            //validate each investment before we proceed with updating
            foreach (var investment in enInvestments)
            {
                double dPrice = 0d;
                var companyInfo = investment.CompanyData;
                if (companyInfo != null && _tryGetClosingPrice(companyInfo.Symbol, companyInfo.Exchange, investment.Name, companyInfo.Currency, account.Currency, companyInfo.ScalingFactor, manualPrices, out dPrice))
                {
                    //validation, compare price with last known price for this investment, if price change> 50%
                    //flag this as an error
                    if(Math.Abs(investment.Price - dPrice) > (investment.Price / 2))
                    {
                        Log.Error("invalid price for {0}. excessive price movement. price = {1}: previous = {2}",
                                    investment.Name, dPrice, investment.Price);
                        bValidationFailed = true;
                        break;
                    }
                }   
            }

            if (bValidationFailed == true)
            {
                return false;
            }

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
                var trade = trades != null ? trades.Changed != null ? trades.Changed.FirstOrDefault(x => company.Equals(x.Name, StringComparison.CurrentCultureIgnoreCase)) : null : null;
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
                _CreateNewInvestment(userToken, newTrade, valuationDate, dClosing);
            }

            //add any transactions to the transaction history table
            if (trades != null)
            {
                _investmentRecordData.AddTradeTransactions(trades.Buys, TradeType.BUY, userToken, valuationDate);
                _investmentRecordData.AddTradeTransactions(trades.Sells, TradeType.SELL, userToken, valuationDate);
                _investmentRecordData.AddTradeTransactions(trades.Changed, TradeType.MODIFY, userToken, valuationDate);
            }

            return true;
        }

        private IEnumerable<CompanyData> GetInvestmentRecordsImpl(UserAccountToken userToken, UserAccountData account, DateTime dtValuationDate, DateTime? dtPreviousValuationDate, bool bSnapshot, ManualPrices manualPrices)
        {
            var lstCurrentData = _GetCompanyDataImpl(userToken, account, dtValuationDate, bSnapshot, manualPrices).ToList();
            var lstPreviousData = dtPreviousValuationDate.HasValue ? _GetCompanyDataImpl(userToken, account, dtPreviousValuationDate.Value, false, null).ToList() : new List<CompanyData>();
            //get the list of all trade transactions between these two dates as this affects the monthly change  
            var trades = dtPreviousValuationDate.HasValue ? _investmentRecordData.GetHistoricalTransactions(dtPreviousValuationDate.Value.AddDays(1), dtValuationDate, userToken) : null;
            
            foreach (var company in lstCurrentData)
            {
                var previousData = lstPreviousData.Find(c => c.Name == company.Name);
                if (previousData != null)
                {
                    _updateMonthlyData(company, previousData, trades);
                }
                company.ProfitLoss = company.NetSellingValue - company.TotalCost;
                if (bSnapshot == false && company.Quantity == 0)
                {
                    _DeactivateInvestment(company.Name, userToken);
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
        public IEnumerable<CompanyData> GetInvestmentRecords(UserAccountToken userToken, UserAccountData account, DateTime dtValuationDate, DateTime? dtPreviousValuationDate, ManualPrices manualPrices, bool bSnapshot)
        {
            //dtPreviousValuationDate parameteris the previous valuation date.we need to extract the previous record
            //valuation date from this to retrieve the correct previous record data from the database
            DateTime? dtNewPreviousRecordValuationDate  = null;
            if (dtPreviousValuationDate.HasValue)
            {
                if(_investmentRecordData.IsExistingRecordValuationDate(userToken, dtPreviousValuationDate.Value))
                {
                    dtNewPreviousRecordValuationDate = dtPreviousValuationDate;
                }
                else
                {
                    dtNewPreviousRecordValuationDate = _investmentRecordData.GetPreviousRecordInvestmentValuationDate(userToken, dtPreviousValuationDate.Value);
                }
            }

            return GetInvestmentRecordsImpl(userToken, account, dtValuationDate, dtNewPreviousRecordValuationDate, bSnapshot, manualPrices);
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
        public IEnumerable<CompanyData> GetInvestmentRecordSnapshot(UserAccountToken userToken, UserAccountData account, ManualPrices manualPrices)
        {
            DateTime? dtPreviousValuationDate = _investmentRecordData.GetLatestRecordInvestmentValuationDate(userToken);
            if (dtPreviousValuationDate.HasValue) //
                return GetInvestmentRecordsImpl(userToken, account, dtPreviousValuationDate.Value, dtPreviousValuationDate, true, manualPrices);

            //if there is no last known valuation date for this account then just return an empty report
            return new List<CompanyData>(); 
        }

        public DateTime? GetLatestRecordValuationDate(UserAccountToken userToken)
        {
            return _investmentRecordData.GetLatestRecordInvestmentValuationDate(userToken);
        }
    }
}
