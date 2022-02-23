namespace InvestmentBuilderCore
{
    #region References

    using System;

    #endregion

    public interface IInvestmentReportWriter
    {
        string GetReportFileName(DateTime dtValuation);
    }

}
