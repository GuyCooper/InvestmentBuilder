using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InvestmentBuilderWeb.Interfaces;
using InvestmentBuilderCore;
using InvestmentBuilderWeb.Models;

namespace InvestmentBuilderWeb.Services 
{
    internal class InvestmentRecordSessionData
    {
        public InvestmentRecordSessionData()
        {
            ManualPrices = new ManualPrices();
            SummaryData = new Dictionary<DateTime, InvestmentSummaryModel>();
        }

        public ManualPrices ManualPrices { get; private set; }
        public DateTime? ValuationDate { get; set; }
        public Dictionary<DateTime, InvestmentSummaryModel> SummaryData { get; set; }
    }

    internal class InvestmentRecordSessionService : IApplicationSessionService
    {
        private Dictionary<string, InvestmentRecordSessionData> _sessionData = new Dictionary<string, InvestmentRecordSessionData>();

        #region IApplicationSessionService
        public void StartSession(string sessionId)
        {
            _sessionData.Add(sessionId, new InvestmentRecordSessionData());
        }

        public void EndSession(string sessionId)
        {
            _clearSession(sessionId);
        }

        public DateTime GetValuationDate(string sessionId)
        {
            if (_sessionData.ContainsKey(sessionId) == true)
            {
                var dt = _sessionData[sessionId].ValuationDate;
                if (dt.HasValue)
                {
                    return dt.Value;
                }
            }
            return DateTime.Today;
        }

        #endregion

        public void AddManualPrice(string sessionId, string company, double price)
        {
            var manualPrices = _GetManualPrices(sessionId);
            if (manualPrices != null)
            {
                if (manualPrices.ContainsKey(company) == false)
                {
                    manualPrices.Add(company, price);
                }
                else
                {
                    manualPrices[company] = price;
                }
            }
        }

        public void SetValuationDate(string sessionId, DateTime dtValuation)
        {
            if (_sessionData.ContainsKey(sessionId) == true)
            {
                _sessionData[sessionId].ValuationDate = dtValuation;
            }
        }

        public void ResetValuationDate(string sessionId)
        {
            if (_sessionData.ContainsKey(sessionId) == true)
            {
                _sessionData[sessionId].ValuationDate = null;
            }
        }

        public ManualPrices GetManualPrices(string sessionId)
        {
            return _GetManualPrices(sessionId);
        }

        public InvestmentSummaryModel GetSummaryData(string sessionId, DateTime dtValuation)
        {
            if(_sessionData.ContainsKey(sessionId) == true)
            {
                if(_sessionData[sessionId].SummaryData.ContainsKey(dtValuation) == true)
                {
                    return _sessionData[sessionId].SummaryData[dtValuation];
                }
            }
            return null;
        }

        public void SetSummaryData(string sessionId, DateTime dtValuation, InvestmentSummaryModel summaryData)
        {
            if (_sessionData.ContainsKey(sessionId) == true)
            {
                if(_sessionData[sessionId].SummaryData.ContainsKey(dtValuation) == false)
                {
                    _sessionData[sessionId].SummaryData.Add(dtValuation, summaryData);
                }
            }
        }

        private void _clearSession(string sessionId)
        {
            if (_sessionData.ContainsKey(sessionId) == true)
            {
                _sessionData.Remove(sessionId);
            }
        }

        private ManualPrices _GetManualPrices(string sessionId)
        {
            if (_sessionData.ContainsKey(sessionId) == true)
            {
                return _sessionData[sessionId].ManualPrices;
            }
            return null;
        }
    }
}