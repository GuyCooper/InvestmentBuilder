using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderCore;

namespace InvestmentBuilderWeb.Interfaces
{
    public class ReportStatus
    {
        public bool IsBuilding { get; set; }
        public int Progress { get; set; }
        public string BuildSection { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }

    public interface IBuildMonitor
    {
        void StartBuilding();
        void StopBuiliding();
        ReportStatus GetReportStatus();
        ProgressCounter GetProgressCounter();
    }
}
