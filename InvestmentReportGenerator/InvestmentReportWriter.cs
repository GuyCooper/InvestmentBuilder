namespace InvestmentReportGenerator
{
    #region References

    using System;
    using InvestmentBuilderCore;

    #endregion
    public class InvestmentReportWriter : IInvestmentReportWriter
    {
        /// <summary>
        /// get the name of the report file used to generate the report
        /// if pdf available use that one otherwise use the excel one
        /// </summary>
        public string GetReportFileName(DateTime ValuationDate)
        {
            return PdfInvestmentReportWriter.GetPdfReportFile(ValuationDate);
        }
    }
}
