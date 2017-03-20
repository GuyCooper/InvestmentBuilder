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
        public bool EnableBuildReport { get; set; }
        public IBuildMonitor BuildMonitor { get; set; }
    }

    internal class InvestmentRecordSessionService : IApplicationSessionService
    {
        private Dictionary<string, InvestmentRecordSessionData> _sessionData = new Dictionary<string, InvestmentRecordSessionData>();

        #region IApplicationSessionService
        public void StartSession(string sessionId)
        {
            if(_sessionData.ContainsKey(sessionId) == false)
            {
                _sessionData.Add(sessionId, new InvestmentRecordSessionData());
            }
        }

        public void EndSession(string sessionId)
        {
            _clearSession(sessionId);
        }

        public DateTime GetValuationDate(string sessionId)
        {
            return _GetSessionValue<DateTime?>(sessionId, "ValuationDate", DateTime.Today).Value;
        }

        #endregion

        public void AddManualPrice(string sessionId, string company, double price)
        {
            var manualPrices = _GetSessionValue<ManualPrices>(sessionId, "ManualPrices");
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
            _SetSessionValue<DateTime?>(sessionId, "ValuationDate", dtValuation);
        }

        public void ResetValuationDate(string sessionId)
        {
            _SetSessionValue<DateTime?>(sessionId, "ValuationDate", null);
        }

        public ManualPrices GetManualPrices(string sessionId)
        {
            return _GetSessionValue<ManualPrices>(sessionId, "ManualPrices");
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

        public bool GetEnableBuildReport(string sessionId)
        {
            return _GetSessionValue<bool>(sessionId, "EnableBuildReport");
        }

        public void SetEnableBuildReport(string sessionId, bool enable)
        {
            _SetSessionValue<bool>(sessionId, "EnableBuildReport", enable);
        }

        public IBuildMonitor GetBuildMonitor(string sessionId)
        {
            return _GetSessionValue<IBuildMonitor>(sessionId, "BuildMonitor" );
        }
        
        public void SetBuildMonitor(string sessionId, IBuildMonitor buildMonitor)
        {
            _SetSessionValue<IBuildMonitor>(sessionId, "BuildMonitor", buildMonitor);
        }

        private void _clearSession(string sessionId)
        {
            if (_sessionData.ContainsKey(sessionId) == true)
            {
                _sessionData.Remove(sessionId);
            }
        }

        private T _GetSessionValue<T>(string sessionId, string valueName, T defaultValue = default(T))
        {
            InvestmentRecordSessionData sessionData;
            if (_sessionData.TryGetValue(sessionId, out sessionData) == true)
            {
                var propInfo = sessionData.GetType().GetProperty(valueName);
                if (propInfo != null)
                {
                    var obj = propInfo.GetValue(sessionData);
                    if (obj != null)
                    {
                        return (T)obj;
                    }
                }
            }
            return defaultValue;
        }

        private void _SetSessionValue<T>(string sessionId, string valueName, T value)
        {
            InvestmentRecordSessionData sessionData;
            if (_sessionData.TryGetValue(sessionId, out sessionData) == true)
            {
                var propInfo = sessionData.GetType().GetProperty(valueName);
                if (propInfo != null)
                {
                    propInfo.SetValue(sessionData, value);
                }
            }
        }
    }
}