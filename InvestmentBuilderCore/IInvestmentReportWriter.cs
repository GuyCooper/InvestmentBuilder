using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace InvestmentBuilderCore
{
    [ContractClass(typeof(IInvestmentReportWriterContract))]
    public interface IInvestmentReportWriter
    {
        void WriteAssetReport(AssetReport report, double startOfYear, string outputPath);
        void WritePerformanceData(IList<IndexedRangeData> data, string outputPath, DateTime dtValuation);

        string GetReportFileName(string outputPath, DateTime dtValuation);
    }

    [ContractClassFor(typeof(IInvestmentReportWriter))]
    internal abstract class IInvestmentReportWriterContract : IInvestmentReportWriter
    {
        public string GetReportFileName(string outputPath, DateTime dtValuation)
        {
            Contract.Requires(string.IsNullOrEmpty(outputPath) == false);
            Contract.Ensures(string.IsNullOrEmpty(Contract.Result<string>()) == false);
            return null;
        }

        public void WriteAssetReport(AssetReport report, double startOfYear, string outputPath)
        {
            Contract.Requires(report != null);
            Contract.Requires(startOfYear > 0);
            Contract.Requires(string.IsNullOrEmpty(outputPath) == false);
        }

        public void WritePerformanceData(IList<IndexedRangeData> data, string outputPath, DateTime dtValuation)
        {
            Contract.Requires(data != null);
            Contract.Requires(data.Count > 0);
            Contract.Requires(string.IsNullOrEmpty(outputPath) == false);
        }
    }
}
