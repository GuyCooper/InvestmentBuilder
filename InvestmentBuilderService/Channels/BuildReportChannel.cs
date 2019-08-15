using System.Threading.Tasks;
using InvestmentBuilderService.Utils;
using System.Timers;
using InvestmentBuilderService.Session;
using InvestmentBuilderCore;
using System;
using System.IO;

namespace InvestmentBuilderService.Channels
{
    internal class BuildStatusResponseDto : Dto
    {
        public ReportStatus Status { get; set; }
    }

    /// <summary>
    /// updater class for build reporter.
    /// </summary>
    internal class BuildReportUpdater : TimerUpdater
    {
        #region Public Methods
        public BuildReportUpdater(IConnectionSession session, UserSession userSession, string channel, string sourceId,string requestId, int interval) : base(session, channel, sourceId, requestId, interval)
        {
            _monitor = new BuildReportMonitor(userSession.UserName);
        }

        public IBuildMonitor GetBuildMonitor()
        {
            return _monitor;
        }
        #endregion
        #region Protected Methods
        /// <summary>
        /// method invoked on timer callback. if the report is still building, send an update to the client
        /// and return true otherwise return false. (this will stop the timer)
        /// </summary>
        protected override bool OnUpdate()
        {
            var status = _monitor.GetReportStatus();
            SendUpdate(new BuildStatusResponseDto { Status = status });
            return status.IsBuilding;
        }
        #endregion

        #region Private Data Members
        private readonly IBuildMonitor _monitor;
        #endregion
    }

    /// <summary>
    /// handler class for building investment report.
    /// </summary>
    internal class BuildReportChannel : EndpointChannel<Dto, BuildReportUpdater>
    {
        #region Constructor
        public BuildReportChannel(ServiceAggregator aggregator)
            : base("BUILD_REPORT_REQUEST", "BUILD_REPORT_RESPONSE", aggregator)
        {
            _builder = aggregator.Builder;
            _chartBuilder = aggregator.ChartBuilder;
            m_settings = aggregator.Settings;
            m_connectionSettings = aggregator.ConnectionSettings;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Handle BuildReport request.
        /// </summary>
        protected override Dto HandleEndpointRequest(UserSession userSession, Dto payload, BuildReportUpdater updater)
        {
            var token = GetCurrentUserToken(userSession);
            
            var monitor = updater.GetBuildMonitor();
            monitor.StartBuilding();
            updater.Start();

            //build the asset report in another thread so it can be built 
            //asynchronously
            Task.Factory.StartNew(() =>
            {
                //DummyBuildRun(monitor);

                var report = _builder.BuildAssetReport(token
                                    , userSession.ValuationDate
                                    , true
                                    , userSession.UserPrices
                                    , monitor.GetProgressCounter());

                if (report != null)
                {
                    //now generate the performance charts. by doing this the whole report will be persisted
                    //to a pdf filen
                    _chartBuilder.Run(token, userSession.ValuationDate, monitor.GetProgressCounter());
                }

                //this command creates a new valuation snapshot. reset the valuation date to allow
                //any subsequent updates.
                userSession.ValuationDate = DateTime.Now;

                var reportFile = CreateReportLink(m_connectionSettings, token.Account, userSession.ValuationDate);

                monitor.StopBuiliding(reportFile);
            });

            return new BuildStatusResponseDto { Status = monitor.GetReportStatus() };
        }

        /// <summary>
        /// Return a buildreport updater that will update on the response channel every second
        /// </summary>
        public override BuildReportUpdater GetUpdater(IConnectionSession session, UserSession userSession, string sourceId, string requestId)
        {
            return new BuildReportUpdater(session, userSession, ResponseName, sourceId, requestId, 1000);
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Method invoked on timer callback.
        /// </summary>
        private void onUpdateBuildStatus(object sender, ElapsedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Tester method for running a dummy run. Test the monitoring functionality
        /// is working ok.
        /// </summary>
        private void DummyBuildRun(IBuildMonitor monitor)
        {
            var waitEvent = new System.Threading.ManualResetEvent(false);
            var progress = monitor.GetProgressCounter();
            for (var outer = 0; outer < 3; outer++)
            {
                progress.ResetCounter($"section_{outer}", 100);
                for(var inner = 0; inner < 100; inner++)
                {
                    System.Threading.Thread.Sleep(100);
                    progress.IncrementCounter();
                }
            }
        }
        #endregion

        #region Private Data Members

        private readonly InvestmentBuilder.InvestmentBuilder _builder;
        private readonly PerformanceBuilderLib.PerformanceBuilder _chartBuilder;
        private readonly IConfigurationSettings m_settings;
        private readonly IConnectionSettings m_connectionSettings;

        #endregion
    }
}
