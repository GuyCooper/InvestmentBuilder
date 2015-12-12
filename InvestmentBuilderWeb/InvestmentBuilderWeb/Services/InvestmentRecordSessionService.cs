using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InvestmentBuilderWeb.Interfaces;
using InvestmentBuilderCore;

namespace InvestmentBuilderWeb.Services 
{
    internal class InvestmentRecordSessionService : IApplicationSessionService
    {
        private Dictionary<string, ManualPrices> _sessionData = new Dictionary<string, ManualPrices>();

        #region IApplicationSessionService
        public void StartSession(string sessionId)
        {
            _sessionData.Add(sessionId, new ManualPrices());
        }

        public void EndSession(string sessionId)
        {
            _clearSession(sessionId);
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

        public ManualPrices GetManualPrices(string sessionId)
        {
            return _GetManualPrices(sessionId);
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
                return _sessionData[sessionId];
            }
            return null;
        }
    }
}