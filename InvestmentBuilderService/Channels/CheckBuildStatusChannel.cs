using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderService.Utils;

namespace InvestmentBuilderService.Channels
{
    internal class BuildStatusResponseDto : Dto
    {
        public ReportStatus Status { get; set; }
    }

    internal class CheckBuildStatusChannel : EndpointChannel<Dto>
    {
        public CheckBuildStatusChannel(AccountService accountService) : 
            base("CHECK_BUILD_STATUS_REQUEST", "CHECK_BUILD_STATUS_RESPONSE", accountService)
        {
        }

        public override Dto HandleEndpointRequest(UserSession userSession, Dto payload)
        {
            var monitor = userSession.BuildMonitor;
            ReportStatus status = null;
            if (monitor != null)
            {
                status = monitor.GetReportStatus();
            }
            else
            {
                status = new ReportStatus();
            }

            return new BuildStatusResponseDto
            {
                Status = status
            };
        }
    }
}
