using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using InvestmentBuilderCore;

namespace InvestmentBuilder
{
    interface ICompanyDataReader
    {
        IEnumerable<CompanyData> GetCompanyData(string account, DateTime dtValuationDate, DateTime? dtPreviousValuationDate);
    }

    /// <summary>
    /// class returns all the investment record data from the investment record
    /// </summary>
    class CompanyDataReader : ICompanyDataReader
    {
        private IInvestmentRecordInterface _investmentRecordData;

        public CompanyDataReader(IInvestmentRecordInterface investmentRecordData)
        {
            _investmentRecordData = investmentRecordData;
        }

        private double _GetNetSellingValue(double dSharesHeld, double dPrice)
        {
            double dGrossValue = dSharesHeld * dPrice;
            if (dGrossValue > 750d)
                return dGrossValue - (dGrossValue * 0.01);
            return dGrossValue - 7.5d;
        }

        private IEnumerable<CompanyData> _GetCompanyDataImpl(string account, DateTime dtValuationDate)
        {
            var investments = _investmentRecordData.GetInvestmentRecordData(account, dtValuationDate).ToList();
            foreach(var investment in investments)
            {
                investment.dNetSellingValue = _GetNetSellingValue(investment.iNumberOfShares, investment.dSharePrice);
            }
            return investments;
        }

        private void _updateMonthlyData(CompanyData currentData, CompanyData previousData)
        {
            currentData.dMonthChange = currentData.dNetSellingValue - previousData.dNetSellingValue;
            currentData.dMonthChangeRatio = currentData.dMonthChange / previousData.dNetSellingValue * 100;
        }

        private void DeactivateInvestment(string investment, string account)
        {
            _investmentRecordData.DeactivateInvestment(account, investment);
        }

        public IEnumerable<CompanyData> GetCompanyData(string account, DateTime dtValuationDate, DateTime? dtPreviousValuationDate)
        {
            var lstCurrentData = _GetCompanyDataImpl(account, dtValuationDate).ToList();
            var lstPreviousData = dtPreviousValuationDate.HasValue ? _GetCompanyDataImpl(account, dtPreviousValuationDate.Value).ToList() : new List<CompanyData>();
            foreach (var company in lstCurrentData)
            {
                var previousData = lstPreviousData.Find(c => c.sName == company.sName);
                if (previousData != null)
                {
                    _updateMonthlyData(company, previousData);
                }
                company.dProfitLoss = company.dNetSellingValue - company.dTotalCost;
                if(company.iNumberOfShares.IsZero() == true)
                {
                    DeactivateInvestment(company.sName, account);
                }
            }
            return lstCurrentData;
        }
    }
}
