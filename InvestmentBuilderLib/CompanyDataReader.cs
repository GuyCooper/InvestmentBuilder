using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using InvestmentBuilderCore;

//
//
//  THIS CLASS IS NOW OBSOLETE!!!!!
//
//

namespace InvestmentBuilder
{
    interface ICompanyDataReader
    {
        IEnumerable<CompanyData> GetCompanyData(UserAccountToken usertoken, DateTime dtValuationDate, DateTime? dtPreviousValuationDate);
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
                investment.NetSellingValue = _GetNetSellingValue(investment.Quantity, investment.SharePrice);
            }
            return investments;
        }

        private void _updateMonthlyData(CompanyData currentData, CompanyData previousData)
        {
            currentData.MonthChange = currentData.NetSellingValue - previousData.NetSellingValue;
            currentData.MonthChangeRatio = currentData.MonthChange / previousData.NetSellingValue * 100;
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
                var previousData = lstPreviousData.Find(c => c.Name == company.Name);
                if (previousData != null)
                {
                    _updateMonthlyData(company, previousData);
                }
                company.ProfitLoss = company.NetSellingValue - company.TotalCost;
                if(company.Quantity == 0)
                {
                    DeactivateInvestment(company.Name, account);
                }
            }
            return lstCurrentData;
        }
    }
}
