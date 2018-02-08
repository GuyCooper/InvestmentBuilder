using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderService.Utils;

namespace InvestmentBuilderService.Channels
{
    internal class BuildReportChannel : EndpointChannel<Dto>
    {
        InvestmentBuilder.InvestmentBuilder _builder;
        PerformanceBuilderLib.PerformanceBuilder _chartBuilder;
        public BuildReportChannel(AccountService accountService, 
                                  InvestmentBuilder.InvestmentBuilder builder,
                                  PerformanceBuilderLib.PerformanceBuilder chartBuilder) 
            : base("BUILD_REPORT_REQUEST", "BUILD_REPORT_RESPONSE", accountService)
        {
            _builder = builder;
            _chartBuilder = chartBuilder;
        }

        public override Dto HandleEndpointRequest(UserSession userSession, Dto payload)
        {
            var token = GetCurrentUserToken(userSession);

            IBuildMonitor monitor = new BuildReportMonitor(userSession.UserName);
            monitor.StartBuilding();

            //build the asset report in another thread so it can be built 
            //asynchronously
            Task.Factory.StartNew(() =>
            {
                var report = _builder.BuildAssetReport(token
                                    , userSession.ValuationDate
                                    , true
                                    , userSession.UserPrices
                                    , monitor.GetProgressCounter());

                if (report != null)
                {
                    //now generate the performance charts. by doing this the whole report will be persisted
                    //to a pdf file
                    _chartBuilder.Run(token, userSession.ValuationDate, monitor.GetProgressCounter());
                }
                monitor.StopBuiliding();
            });

            userSession.BuildMonitor = monitor;

            return new ResponseDto { Status = true };
        }
    }
}
