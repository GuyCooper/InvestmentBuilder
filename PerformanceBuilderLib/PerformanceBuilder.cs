﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketDataServices;
using NLog;
using InvestmentBuilderCore;
using System.Diagnostics.Contracts;

namespace PerformanceBuilderLib
{
    public sealed class PerformanceBuilder 
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private IConfigurationSettings _settings;
        private IDataLayer _dataLayer;
        private IInvestmentReportWriter _reportWriter;

        public PerformanceBuilder(IConfigurationSettings settings, IDataLayer dataLayer,
                                IInvestmentReportWriter reportWriter)
        {
            _dataLayer = dataLayer;
            _settings = settings;
            _reportWriter = reportWriter;
        }

        public IList<IndexedRangeData> Run(UserAccountToken userToken, DateTime dtValuation, ProgressCounter progress)
        {
            Contract.Requires(userToken != null);
            Contract.Ensures(Contract.Result<IList<IndexedRangeData>>() != null);

            logger.Log(LogLevel.Info, "starting performance builder...");
            logger.Log(LogLevel.Info, "output path: {0}", _settings.GetOutputPath(userToken.Account.GetPathName()));
            logger.Log(LogLevel.Info, "valuation date {0}", dtValuation);

            var ladderBuilder = new PerformanceLaddersBuilder(_settings, _dataLayer);
            var allLadders = ladderBuilder.BuildPerformanceLadders(userToken, dtValuation, progress).ToList();

            //now insert the individual company company
            allLadders.InsertRange(0, ladderBuilder.BuildCompanyPerformanceLadders(userToken, progress));

            //now build the company dividend ladders
            allLadders.Insert(0, ladderBuilder.BuildAccountDividendPerformanceLadder(userToken, progress));
           
            //and finally the company dividend yield ladders
            allLadders.Insert(0, ladderBuilder.BuildAccountDividendYieldPerformanceLadder(userToken, progress));

            logger.Log(LogLevel.Info, "data ladders building complete...");
            //now persist it to the spreadsheet, TODO, make configurable, allow persist to pdf
            _reportWriter.WritePerformanceData(allLadders, _settings.GetOutputPath(userToken.Account.GetPathName()), dtValuation, progress);

            logger.Log(LogLevel.Info, "performance chartbuilder complete");
            return allLadders;
        }
    }

 }

  