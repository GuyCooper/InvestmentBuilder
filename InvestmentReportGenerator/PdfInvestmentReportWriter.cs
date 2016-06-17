using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderCore;

namespace InvestmentReportGenerator
{
    public class PdfInvestmentReportWriter : IInvestmentReportWriter, IDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void WriteAssetReport(AssetReport report, double startOfYear, string outputPath)
        {
            throw new NotImplementedException();
        }

        public void WritePerformanceData(IList<IndexedRangeData> data, string path, DateTime dtValuation)
        {
            throw new NotImplementedException();
        }
    }
}
