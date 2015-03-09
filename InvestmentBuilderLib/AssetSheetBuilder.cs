﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace InvestmentBuilder
{
    public enum DataFormat
    {
        EXCEL,
        DATABASE
    }

    public static class AssetSheetBuilder
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// factory method to create the correct factory class
        /// </summary>
        /// <param name="format"></param>
        /// <param name="path"></param>
        /// <param name="connectionstr"></param>
        /// <param name="bTest"></param>
        /// <returns></returns>
        private static IInvestmentFactory BuildFactory(DataFormat format, string path, string connectionstr, DateTime dtValuation, bool bUpdate)
        {
            switch(format)
            {
                case DataFormat.EXCEL:
                    return new ExcelInvestmentFactory(path, dtValuation, bUpdate);
                case DataFormat.DATABASE:
                    return new DatabaseInvestmentFactory(path, connectionstr, dtValuation, bUpdate);
            }

            throw new ArgumentException("invalid dataformat");
             
        }

        /// <summary>
        /// generate asset report
        /// </summary>
        /// <param name="userName">club /account name</param>
        /// <param name="tradeFile">trade file</param>
        /// <param name="path">output path</param>
        /// <param name="connectionstr">db connectionstring</param>
        /// <param name="valuationDate">valuation date</param>
        /// <param name="format">format EXCEL / DATABASE</param>
        /// <param name="bUpdate">flag to save report to db and spreadsheet</param>
        /// <returns></returns>
        public static AssetReport BuildAssetSheet(string userName, string tradeFile, string path, string connectionstr, DateTime valuationDate, DataFormat format, bool bUpdate)
        {
            logger.Log(LogLevel.Info, string.Format("Begin BuildAssetSheet"));
            logger.Log(LogLevel.Info,string.Format("trade file: {0}", tradeFile));
            logger.Log(LogLevel.Info,string.Format("path: {0}", path));
            logger.Log(LogLevel.Info,string.Format("datasource: {0}",connectionstr));
            logger.Log(LogLevel.Info,string.Format("update: {0}", bUpdate));
            logger.Log(LogLevel.Info,string.Format("valuation date: {0}", valuationDate.ToShortDateString()));
            logger.Log(LogLevel.Info, string.Format("data format: {0}", format));

            var factory = BuildFactory(format, path, connectionstr, valuationDate, bUpdate);

            AssetReport assetReport = null;
            try
            {
                var trades = TradeLoader.GetTrades(tradeFile);

                var recordBuilder = factory.CreateInvestmentRecordBuilder();
                var dataReader = factory.CreateCompanyDataReader();
                var assetWriter = factory.CreateAssetStatementWriter();
                var cashAccountReader = factory.CreateCashAccountReader();
                var userDataReader = factory.CreateUserDataReader();

                var userData = userDataReader.GetUserData(userName);

                if(userData == null)
                {
                    logger.Log(LogLevel.Error, "invalid username {0}", userName);
                    return assetReport;
                }

                //rollback any previous updates made for this valuation date
                if (bUpdate)
                {
                    userData.RollbackValuationDate(valuationDate);
                }

                var dtPreviousValuation = userData.GetPreviousValuationDate(valuationDate);
                //first extract the cash account data
                var cashAccountData = cashAccountReader.GetCashAccountData(valuationDate);
                //parse the trade file for any trades for this month and update the investment record
                //var trades = TradeLoader.GetTrades(tradeFile);
                if (bUpdate)
                {
                    recordBuilder.BuildInvestmentRecords(trades, cashAccountData, valuationDate, dtPreviousValuation);
                }

                //now extract the latest data from the investment record
                var lstData = dataReader.GetCompanyData(valuationDate, dtPreviousValuation).ToList();
                foreach (var val in lstData)
                {
                    logger.Log(LogLevel.Info, string.Format("{0} : {1} : {2} : {3} : {4}", val.sName, val.dSharePrice, val.dNetSellingValue, val.dMonthChange, val.dMonthChangeRatio));
                    //Console.WriteLine("{0} : {1} : {2} : {3} : {4}", val.sName, val.dSharePrice, val.dNetSellingValue, val.dMonthChange, val.dMonthChangeRatio);
                }

                assetReport = _BuildAssetReport(valuationDate,
                                                dtPreviousValuation,
                                                userData,
                                                lstData,
                                                cashAccountData.BankBalance,
                                                bUpdate);
                //finally, build the asset statement
                //assetWriter.WriteAssetStatement(lstData, cashAccountData, dtPreviousValuation, valuationDate);

                logger.Log(LogLevel.Info, "commiting changes...");
                factory.CommitData();
                logger.Log(LogLevel.Info, "Account Builder Complete, Asset statement sheet generated: {0}", factory.AssetSheetLocation);
            }
            finally
            {
                //app.Workbooks.Close();
                factory.Close();
            }

            return assetReport;
        }

        private static AssetReport _BuildAssetReport(DateTime dtValuationDate,
                                                     DateTime? dtPreviousValution,
                                                     UserData userData,
                                                     IEnumerable<CompanyData> companyData,
                                                     double dBankBalance,
                                                     bool bUpdate)
        {
            logger.Log(LogLevel.Info, "building asset report...");
            AssetReport report = new AssetReport
            {
                ClubName = userData.Name,
                ReportingCurrency = userData.Currency,
                ValuationDate = dtValuationDate,
                Assets = companyData,
                BankBalance = dBankBalance
            };

            if (bUpdate)
            {
                report.IssuedUnits = _UpdateMembersCapitalAccount(userData, dtPreviousValution, dtValuationDate);
            }
            else
            {
                report.IssuedUnits = userData.GetIssuedUnits(dtValuationDate);
            }

            report.TotalAssetValue = companyData.Sum(c => c.dNetSellingValue);
            report.TotalAssets = report.BankBalance + report.TotalAssetValue;
            report.TotalLiabilities = 0d; //todo, record liabilities(if any)
            report.NetAssets = report.TotalAssets - report.TotalLiabilities;
            report.ValuePerUnit = report.NetAssets / report.IssuedUnits;
            //todo total assets
            //unit price
            return report;
        }

        private static double _UpdateMembersCapitalAccount(UserData userData, DateTime? dtPreviousValution, DateTime dtValuationDate)
        {
            logger.Log(LogLevel.Info, "updating members capital account...");
            //get total number of shares allocated for previous month
            //get list of all members who have made a deposit for current month
            double dResult = 0d;
            var dPreviousUnitValue = userData.GetPreviousUnitValuation(dtValuationDate, dtPreviousValution);
            var memberAccountData = userData.GetMemberAccountData(dtPreviousValution ?? dtValuationDate).ToList();
            foreach (var member in memberAccountData)
            {
                double dSubscription = userData.GetMemberSubscription(dtValuationDate, member.Key);
                double dNewAmount = member.Value + (dSubscription * (1 / dPreviousUnitValue));
                dResult += dNewAmount;
                userData.UpdateMemberAccount(dtValuationDate, member.Key, dNewAmount);
            }
            return dResult;
        }
    }
}
