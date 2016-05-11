using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketDataServices;
using NLog;
using InvestmentBuilderCore;

namespace PerformanceBuilderLib
{
 
    public class PerformanceBuilder 
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private IConfigurationSettings _settings;
        private IDataLayer _dataLayer;
        private IMarketDataSource _marketDataSource;

        public PerformanceBuilder(IConfigurationSettings settings, IDataLayer dataLayer, IMarketDataSource marketDataSource)
        {
            _dataLayer = dataLayer;
            _marketDataSource = marketDataSource;
            _settings = settings;
        }

        public IList<IndexedRangeData> Run(UserAccountToken userToken, DateTime dtValuation)
        {
            logger.Log(LogLevel.Info, "starting performance builder...");
            logger.Log(LogLevel.Info, "output path: {0}", _settings.GetOutputPath(userToken.Account));
            logger.Log(LogLevel.Info, "valuation date {0}", dtValuation);


            var ladderBuilder = new PerformanceLaddersBuilder(_settings, _dataLayer, _marketDataSource);
            var allLadders = ladderBuilder.BuildPerformanceLadders(userToken, dtValuation);

            logger.Log(LogLevel.Info, "data ladders building complete...");

            //now persist it to the spreadsheet, TODO, make configurable, allow persist to pdf
            using(var dataWriter = new PerformanceExcelSheetWriter(_settings.GetOutputPath(userToken.Account), dtValuation))
            {
                dataWriter.WritePerformanceData(allLadders);
            }

            logger.Log(LogLevel.Info, "performance chartbuilder complete");
            return allLadders;
        }
    }
}

  