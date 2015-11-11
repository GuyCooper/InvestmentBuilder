using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using InvestmentBuilderCore;
using MarketDataServices;

namespace InvestmentBuilder
{
    public class InvestmentBuilder
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private IConfigurationSettings _settings;
        private IDataLayer _dataLayer;
        private IMarketDataService _marketDataService;
        private IUserAccountInterface _userAccountData;
        private ICashAccountInterface _cashAccountData;

        public InvestmentBuilder(IConfigurationSettings settings, IDataLayer dataLayer, IMarketDataService marketDataService)
        {
            _settings = settings;
            _dataLayer = dataLayer;
            _userAccountData = _dataLayer.UserAccountData;
            _marketDataService = marketDataService;
            _cashAccountData = _dataLayer.CashAccountData;
        }
  
        /// <summary>
        /// generate asset report
        /// </summary>
        /// <param name="accountName">club /account name</param>
        /// <param name="valuationDate">valuation date</param>
        /// <param name="bUpdate">flag to save report to db and spreadsheet</param>
        /// <returns></returns>
        public AssetReport BuildAssetReport(UserAccountToken userToken, DateTime valuationDate, bool bUpdate, ManualPrices manualPrices)
        {
            logger.Log(LogLevel.Info, string.Format("Begin BuildAssetSheet"));
            //logger.Log(LogLevel.Info,string.Format("trade file: {0}", _settings.GetTradeFile(accountName)));
            logger.Log(LogLevel.Info,string.Format("path: {0}", _settings.OutputFolder));
            logger.Log(LogLevel.Info,string.Format("datasource: {0}",_settings.DatasourceString));
            logger.Log(LogLevel.Info,string.Format("update: {0}", bUpdate));
            logger.Log(LogLevel.Info,string.Format("valuation date: {0}", valuationDate.ToShortDateString()));

            //var factory = BuildFactory(format, path, connectionstr, valuationDate, bUpdate);
            if(userToken == null)
            {
                throw new ArgumentNullException("invalid user token");
            }

            AssetReport assetReport = null;
            //    var trades = TradeLoader.GetTrades(_settings.GetTradeFile(accountName));

            var recordBuilder = new InvestmentRecordBuilder(_marketDataService, _dataLayer.InvestmentRecordData);
            //var dataReader = new CompanyDataReader(_dataLayer.InvestmentRecordData);

            var accountData = _userAccountData.GetUserAccountData(userToken);

            if(accountData == null)
            {
                logger.Log(LogLevel.Error, "invalid username {0}", userToken);
                return assetReport;
            }

            //rollback any previous updates made for this valuation date
            if (bUpdate)
            {
                _userAccountData.RollbackValuationDate(userToken, valuationDate);
            }

            var dtPreviousValuation = _userAccountData.GetPreviousAccountValuationDate(userToken, valuationDate);
            //first extract the cash account data
            var cashAccountData = _cashAccountData.GetCashAccountData(userToken, valuationDate);
            //parse the trade file for any trades for this month and update the investment record
            //var trades = TradeLoader.GetTrades(tradeFile);
            var dtTradeValuationDate = DateTime.Now;
            if (bUpdate)
            {
                //trades now added seperately
                var emptyTrades = new Trades
                {
                    Buys = Enumerable.Empty<Stock>().ToArray(),
                    Sells = Enumerable.Empty<Stock>().ToArray(),
                    Changed = Enumerable.Empty<Stock>().ToArray()
                };

                if (recordBuilder.UpdateInvestmentRecords(userToken, accountData, emptyTrades/*trades*/, cashAccountData, dtTradeValuationDate, manualPrices) == false)
                {
                    //failed to update investments, return null report
                    return assetReport;
                }
            }
            else
            {
                var currentRecordData = recordBuilder.GetLatestRecordValuationDate(userToken);
                if(currentRecordData.HasValue)
                {
                    dtTradeValuationDate = currentRecordData.Value;
                }
            }

            //now extract the latest data from the investment record
            var lstData = recordBuilder.GetInvestmentRecords(userToken, accountData, dtTradeValuationDate, dtPreviousValuation).ToList();
            foreach (var val in lstData)
            {
                logger.Log(LogLevel.Info, string.Format("{0} : {1} : {2} : {3} : {4}", val.Name, val.SharePrice, val.NetSellingValue, val.MonthChange, val.MonthChangeRatio));
                //Console.WriteLine("{0} : {1} : {2} : {3} : {4}", val.sName, val.dSharePrice, val.dNetSellingValue, val.dMonthChange, val.dMonthChangeRatio);
            }

            assetReport = _BuildAssetReport(
                                            userToken,
                                            valuationDate,
                                            dtPreviousValuation,
                                            accountData,
                                            lstData,
                                            cashAccountData.BankBalance,
                                            bUpdate);
            //finally, build the asset statement
            //assetWriter.WrC:\Projects\InvestmentBuilder\InvestmentBuilderLib\AssetSheetBuilder.csiteAssetStatement(lstData, cashAccountData, dtPreviousValuation, valuationDate);
            //if(bUpdate)
            //{
                using(var assetWriter = new AssetReportWriterExcel(_settings.GetOutputPath(accountData.Name),
                                                                   _settings.GetTemplatePath()))
                {
                    assetWriter.WriteAssetReport(assetReport);
                }
            //}

            logger.Log(LogLevel.Info, "Report Generated, Account Builder Complete");
            return assetReport;
        }

        /// <summary>
        /// This method returns a snapshot of the investment records using today as the current 
        /// valuation date and the most recent valuation date in the database as the previous
        /// valuation date. if they are both the same date then just return previous valuation date 
        /// </summary>
        /// <param name="accountName"></param>
        /// <returns></returns>
        public IEnumerable<CompanyData> GetCurrentInvestments(UserAccountToken userToken, ManualPrices manualPrices)
        {
            if (userToken == null)
            {
                throw new ArgumentNullException("invalid user token");
            }

            var recordBuilder = new InvestmentRecordBuilder(_marketDataService, _dataLayer.InvestmentRecordData);
            var accountData = _userAccountData.GetUserAccountData(userToken);

            //check this is a valid account
            if (accountData == null)
            {
                logger.Log(LogLevel.Error, "invalid account {0}", userToken.Account);
                return Enumerable.Empty<CompanyData>();
            }

            return recordBuilder.GetInvestmentRecordSnapshot(userToken, accountData, manualPrices);

        }

        /// <summary>
        /// This method updates trade information. This is almost the same as build asset report because
        /// we need to update the database with the new date so any subsequent calls to get investment records
        /// will retrieve the new trades
        /// </summary>
        /// <param name="trades"></param>
        public bool UpdateTrades(UserAccountToken userToken, Trades trades, ManualPrices manualPrices)
        {
            if (userToken == null)
            {
                throw new ArgumentNullException("invalid user token");
            }

            var recordBuilder = new InvestmentRecordBuilder(_marketDataService, _dataLayer.InvestmentRecordData);

            var accountData = _userAccountData.GetUserAccountData(userToken);

            //check this is a valid account
            if (accountData == null)
            {
                logger.Log(LogLevel.Error, "invalid account {0}", userToken.Account);
            }

            return recordBuilder.UpdateInvestmentRecords(userToken, accountData, trades, null, DateTime.Now, manualPrices);
        }

        private AssetReport _BuildAssetReport(
                                                UserAccountToken userToken,
                                                DateTime dtValuationDate,
                                                DateTime? dtPreviousValution,
                                                UserAccountData userData,
                                                IEnumerable<CompanyData> companyData,
                                                double dBankBalance,
                                                bool bUpdate)
        {
            logger.Log(LogLevel.Info, "building asset report...");
            AssetReport report = new AssetReport
            {
                AccountName = userData.Name,
                ReportingCurrency = userData.Currency,
                ValuationDate = dtValuationDate,
                Assets = companyData,
                BankBalance = dBankBalance
            };

            report.TotalAssetValue = companyData.Sum(c => c.NetSellingValue);
            report.MonthlyPnL = companyData.Sum(c => c.MonthChange);
            report.TotalAssets = report.BankBalance + report.TotalAssetValue;
            report.TotalLiabilities = default(double); //todo, record liabilities(if any)
            report.NetAssets = report.TotalAssets - report.TotalLiabilities;

            if (bUpdate)
            {
                report.IssuedUnits = _UpdateMembersCapitalAccount(userToken, userData, dtPreviousValution, dtValuationDate, report.NetAssets);
            }
            else
            {
                report.IssuedUnits = _userAccountData.GetIssuedUnits(userToken, dtValuationDate);
            }

            if (report.IssuedUnits > default(double))
            {
                report.ValuePerUnit = report.NetAssets / report.IssuedUnits;
            }
            else
            {
                report.ValuePerUnit = 1d; //default unit value
            }

            //todo total assets
            //unit price
            if (bUpdate)
            {
               _userAccountData.SaveNewUnitValue(userToken, dtValuationDate, report.ValuePerUnit);
            }

            return report;
        }

        private double _UpdateMembersCapitalAccount(
                                                    UserAccountToken userToken,
                                                    UserAccountData userData,
                                                    DateTime? dtPreviousValution,
                                                    DateTime dtValuationDate,
                                                    double dNetAssets)
        {
            logger.Log(LogLevel.Info, "updating members capital account...");
            //get total number of shares allocated for previous month
            //get list of all members who have made a deposit for current month
            double dResult = default(double);
            if (dtPreviousValution.HasValue)
            {
                var dPreviousUnitValue = _userAccountData.GetPreviousUnitValuation(userToken, dtValuationDate, dtPreviousValution);
                var memberAccountData = _userAccountData.GetMemberAccountData(userToken, dtPreviousValution ?? dtValuationDate).ToList();
                foreach (var member in memberAccountData)
                {
                    double dSubscription = _userAccountData.GetMemberSubscription(userToken, dtValuationDate, member.Key);
                    double dNewAmount = member.Value + (dSubscription * (1 / dPreviousUnitValue));
                    dResult += dNewAmount;
                    _userAccountData.UpdateMemberAccount(userToken, dtValuationDate, member.Key, dNewAmount);
                }
            }
            else
            {
                logger.Log(LogLevel.Info, "new account. setting issued units equal to net assets");
                //no previous valaution this is a new account, the total issued units should be the same as
                //the total netassets. this will give a unit valuation of 1.
                var members = _userAccountData.GetAccountMembers(userToken).ToList();
                double memberUnits = dNetAssets / members.Count;
                dResult = dNetAssets;
                foreach(var member in members)
                {
                    _userAccountData.UpdateMemberAccount(userToken, dtValuationDate, member, memberUnits);
                }
            }
            return dResult;
        }
    }
}
