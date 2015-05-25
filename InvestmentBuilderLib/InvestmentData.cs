using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderCore;

namespace InvestmentBuilder
{
    /// <summary>
    /// investment record interface
    /// </summary>
    interface IInvestment
    {
        string Name { get; }
        InvestmentInformation CompanyData { get; }
        void UpdateRow(DateTime valuationDate, DateTime previousDate);
        void ChangeShareHolding(DateTime valuationDate, int quantity);
        void AddNewShares(DateTime valuationDate, Stock stock);
        void UpdateClosingPrice(DateTime valuationDate, double dClosing);
        void UpdateDividend(DateTime valuationDate, double dDividend);
        void SellShares(DateTime valuationDate, Stock stock);
    }

    class InvestmentData : IInvestment
    {
        private string _account;
        private IInvestmentRecordInterface _investmentRecordData;
        public InvestmentData(string account, string name, IInvestmentRecordInterface investmentRecordData)
        {
            Name = name;
            _account = account;
            _investmentRecordData = investmentRecordData;
            CompanyData = _investmentRecordData.GetInvestmentDetails(Name);
        }

        public string Name
        {
            get;
            private set;
        }

        public InvestmentInformation CompanyData
        {
            get;
            private set;
        }

        public void UpdateRow(DateTime valuationDate, DateTime previousDate)
        {
            _investmentRecordData.RollInvestment(_account, Name, valuationDate, previousDate);
        }

        public void ChangeShareHolding(DateTime valuationDate, int quantity)
        {
            _investmentRecordData.UpdateInvestmentQuantity(_account, Name, valuationDate, quantity);
        }

        public void AddNewShares(DateTime valuationDate, Stock stock)
        {
            _investmentRecordData.AddNewShares(_account, Name, stock.Quantity, valuationDate, stock.TotalCost);
        }

        public void UpdateClosingPrice(DateTime valuationDate, double dClosing)
        {
            _investmentRecordData.UpdateClosingPrice(_account, Name, valuationDate, dClosing);
        }

        public void UpdateDividend(DateTime valuationDate, double dDividend)
        {
            _investmentRecordData.UpdateDividend(_account, Name, valuationDate, dDividend);
        }

        public void SellShares(DateTime valuationDate, Stock stock)
        {
            _investmentRecordData.SellShares(_account, Name, stock.Quantity, valuationDate);
        }
    }
}
