using InvestmentBuilderCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InvestmentBuilderService.Channels
{
    /// <summary>
    /// RecentReportFile class. Contains File information about a report
    /// </summary>
    internal class RecentReportFile
    {
        public string ValuationDate { get; set; }
        public string FileName { get; set; }
    }

    /// <summary>
    /// Dto returned from channel handler. Contains a list of RecentReportFile objects.
    /// </summary>
    internal class RecentReportListDto : Dto
    {
        public List<RecentReportFile> RecentReports { get; set; } 
    }

    /// <summary>
    /// Channel Handler for returning a list of recent report file locations
    /// </summary>
    internal class GetRecentReportFiles : EndpointChannel<Dto, ChannelUpdater>
    {
        #region Constructor
        /// <summary>
        /// Constructor.
        /// </summary>
        public GetRecentReportFiles(AccountService accountService, IDataLayer dataLayer, InvestmentBuilder.InvestmentBuilder builder,
            IConfigurationSettings settings) 
            : base("GET_RECENT_REPORTS_REQUEST", "GET_RECENT_REPORTS_RESPONSE", accountService)
        {
            _clientData = dataLayer.ClientData;
            _builder = builder;
            _m_settings = settings;
        }

        #endregion

        #region Protected Methods
        /// <summary>
        /// Handle request for GetRecentReportFiles.
        /// </summary>
        protected override Dto HandleEndpointRequest(UserSession userSession, Dto payload, ChannelUpdater updater)
        {
            var userToken = GetCurrentUserToken(userSession);
            return new RecentReportListDto
            {
                RecentReports = _clientData.GetRecentValuationDates(userToken, DateTime.Now).Select(x =>
                    new RecentReportFile
                    {
                        FileName = $"{_m_settings.GetOutputLinkPath(userToken.Account)}/{_builder.GetInvestmentReport(userToken, x)}",
                        ValuationDate = x.ToShortDateString()
                    }).ToList()
            };
        }

        #endregion

        #region Private Data Members

        private readonly IClientDataInterface _clientData;
        private readonly InvestmentBuilder.InvestmentBuilder _builder;
        private readonly IConfigurationSettings _m_settings;

        #endregion
    }
}
