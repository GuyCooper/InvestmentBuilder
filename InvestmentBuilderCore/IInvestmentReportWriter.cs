using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace InvestmentBuilderCore
{
    [ContractClass(typeof(IInvestmentReportWriterContract))]
    public interface IInvestmentReportWriter
    {
        void WriteAssetReport(AssetReport report, double startOfYear, string outputPath, ProgressCounter progress);
        void WritePerformanceData(IList<IndexedRangeData> data, string outputPath, DateTime dtValuation, ProgressCounter progress);

        string GetReportFileName(DateTime dtValuation);
    }

    [ContractClassFor(typeof(IInvestmentReportWriter))]
    internal abstract class IInvestmentReportWriterContract : IInvestmentReportWriter
    {
        public string GetReportFileName(DateTime dtValuation)
        {
            Contract.Ensures(string.IsNullOrEmpty(Contract.Result<string>()) == false);
            return null;
        }

        public void WriteAssetReport(AssetReport report, double startOfYear, string outputPath, ProgressCounter progress = null)
        {
            Contract.Requires(report != null);
            Contract.Requires(startOfYear > 0);
            Contract.Requires(string.IsNullOrEmpty(outputPath) == false);
        }

        public void WritePerformanceData(IList<IndexedRangeData> data, string outputPath, DateTime dtValuation, ProgressCounter progress)
        {
            Contract.Requires(data != null);
            Contract.Requires(data.Count > 0);
            Contract.Requires(string.IsNullOrEmpty(outputPath) == false);
        }

    }
}
