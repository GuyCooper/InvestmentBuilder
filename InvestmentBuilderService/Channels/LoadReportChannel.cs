using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilder;

namespace InvestmentBuilderService.Channels
{
    internal class LoadReportRequestDto : Dto
    {
        public string ValuationDate { get; set; }
    }

    internal class ReportLocationDto : Dto
    {
        public string Location { get; set; }
    }

    internal class LoadReportChannel : EndpointChannel<LoadReportRequestDto>
    {
        InvestmentBuilder.InvestmentBuilder _builder;

        public LoadReportChannel(AccountService accountService, InvestmentBuilder.InvestmentBuilder builder) : 
            base("LOAD_REPORT_REQUEST", "LOAD_REPORT_RESPONSE", accountService)
        {
            _builder = builder;
        }

        public override Dto HandleEndpointRequest(UserSession userSession, LoadReportRequestDto payload)
        {
            var token = GetCurrentUserToken(userSession);
            var dtValuation = payload.ValuationDate != null ? DateTime.Parse(payload.ValuationDate) : userSession.ValuationDate;
            var reportFile = _builder.GetInvestmentReport(token, dtValuation);

            return new ReportLocationDto
            {
                Location = reportFile
            };
        }
    }
}
