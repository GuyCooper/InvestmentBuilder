using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestmentBuilderService.Channels
{
    class BuildReportChannel : EndpointChannel<Dto>
    {
        public BuildReportChannel(AccountService accountService) 
            : base("BUILD_REPORT_REQUEST", "BUILD_REPORT_RESPONSE", accountService)
        {
        }

        public override Dto HandleEndpointRequest(UserSession userSession, Dto payload)
        {
            throw new NotImplementedException();
        }
    }
}
