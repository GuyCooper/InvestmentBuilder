using InvestmentBuilderCore;
using InvestmentBuilderService.Utils;
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
        public string Link { get; set; }
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
            IConnectionSettings settings) 
            : base("GET_RECENT_REPORTS_REQUEST", "GET_RECENT_REPORTS_RESPONSE", accountService)
        {
            m_clientData = dataLayer.ClientData;
            m_builder = builder;
            m_connectionSettings = settings;
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
                RecentReports = m_clientData.GetRecentValuationDates(userToken, DateTime.Now).Select(x =>
                    new RecentReportFile
                    {
                        Link = CreateReportLink(m_connectionSettings, userToken.Account, x),
                        ValuationDate = x.ToShortDateString()
                    }).ToList()
            };
        }

        #endregion

        #region Private Data Members

        private readonly IClientDataInterface m_clientData;
        private readonly InvestmentBuilder.InvestmentBuilder m_builder;
        private readonly IConnectionSettings m_connectionSettings;

        #endregion
    }
}
