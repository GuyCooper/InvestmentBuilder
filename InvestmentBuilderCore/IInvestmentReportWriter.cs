using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestmentBuilderCore
{
    public interface IInvestmentReportWriter
    {
        void WriteAssetReport(AssetReport report, double startOfYear, string outputPath);
        void WritePerformanceData(IList<IndexedRangeData> data, string outputPath, DateTime dtValuation);

        string GetReportFileName(string outputPath, DateTime dtValuation);
    }
}
