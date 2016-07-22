using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderCore;

namespace InvestmentReportGenerator
{
    public class InvestmentReportWriter : IInvestmentReportWriter, IDisposable
    {
        private ExcelInvestmentReportWriter _excelReport;
        private PdfInvestmentReportWriter _pdfReport;
        private IConfigurationSettings _settings;

        public InvestmentReportWriter(IConfigurationSettings settings)
        {
            _settings = settings;
        }

        public void Dispose()
        {
            if(_excelReport != null)
                _excelReport.Dispose();

            if(_pdfReport != null)
                _pdfReport.Dispose();
        }

        public void WriteAssetReport(AssetReport report, double startOfYear, string outputPath)
        {
            var reports = _InitReports().ToList();
           foreach (var reportType in reports)
            {
                reportType.WriteAssetReport(report, startOfYear, outputPath);
            }
        }

        public void WritePerformanceData(IList<IndexedRangeData> data, string path, DateTime dtValuation)
        {
            foreach (var reportType in _InitReports())
            {
                reportType.WritePerformanceData(data, path, dtValuation);
            }
        }

        private IEnumerable<IInvestmentReportWriter> _InitReports()
        {
            if (_settings.ReportFormats != null)
            {
                if (_settings.ReportFormats.Contains("EXCEL") == true)
                {
                    if (_excelReport == null)
                        _excelReport = new ExcelInvestmentReportWriter(_settings.GetTemplatePath());
                    yield return _excelReport;
                }
                if (_settings.ReportFormats.Contains("PDF") == true)
                {
                    if (_pdfReport == null)
                        _pdfReport = new PdfInvestmentReportWriter();
                    yield return _pdfReport;
                }
            }
        }
    }
}
