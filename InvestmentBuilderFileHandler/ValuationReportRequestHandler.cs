using System;
using System.IO;
using MiddlewareInterfaces;
using System.Reflection;
using NLog;

namespace InvestmentBuilderFileHandler
{
    /// <summary>
    /// Valuation Report parameters.
    /// </summary>
    class ValuationReportParams
    {
        /// <summary>
        /// Name of account to retreive report for
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// Date of report.
        /// </summary>
        public string Date { get; set; }
    }

    /// <summary>
    /// File handler class for managing a valuation report request.
    /// </summary>
    public class ValuationReportRequestHandler : FileRequestHandler
    {
        #region Public Properties

        /// <summary>
        /// Identifier of filehandler.
        /// </summary>
        public string Identifier { get { return "VALUATION_REPORT"; } }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        public ValuationReportRequestHandler()
        {
            var configFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "InvestmentBuilderFileHandlerConfig.xml");
            logger.Info($"Loading configuration file {configFile}");
            var config = InvestmentBuilderFileHandlerConfig.CreateConfig(configFile);
            m_reportFolder = config.ReportFolder;
        }

        /// <summary>
        /// Handle file request. Load the requested valuation report.
        /// </summary>
        public void HandleFileRequest(string query, Stream responseStream, out string contentType)
        {
            var reportParams = MiddlewareUtils.DeserialiseObject<ValuationReportParams>(query);
            var filename = Path.Combine(m_reportFolder, reportParams.Account, $"ValuationReport-{reportParams.Date}.pdf");
            var fileData = File.ReadAllBytes(filename);
            responseStream.Write(fileData, 0, fileData.Length);
            contentType = "application/pdf";
        }

        #endregion

        #region Private Data

        private readonly string m_reportFolder;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        #endregion
    }
}

