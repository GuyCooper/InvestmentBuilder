using System;
using InvestmentBuilderWeb.Models;
using InvestmentBuilderCore;

namespace InvestmentBuilderWeb.Interfaces
{
    public interface IApplicationSessionService
    {
        void StartSession(string sessionId);
        void EndSession(string sessionId);
        DateTime GetValuationDate(string sessionId);
        void AddManualPrice(string sessionId, string company, double price);
        ManualPrices GetManualPrices(string sessionId);
        void SetValuationDate(string sessionId, DateTime dtValuation);
        void ResetValuationDate(string sessionId);
        InvestmentSummaryModel GetSummaryData(string sessionId, DateTime dtValuation);
        void SetSummaryData(string sessionId, DateTime dtValuation, InvestmentSummaryModel summaryData);
        bool GetEnableBuildReport(string sessionId);
        void SetEnableBuildReport(string sessionId, bool enable);
    }
}
