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
    internal interface IInvestment
    {
        //name of investment
        string Name { get; }
        //latest price of investment
        double Price { get; }
        InvestmentInformation CompanyData { get; }
        void UpdateRow(DateTime valuationDate, DateTime previousDate);
        void ChangeShareHolding(DateTime valuationDate, int quantity);
        void AddNewShares(DateTime valuationDate, Stock stock);
        void UpdateClosingPrice(DateTime valuationDate, double dClosing);
        void UpdateDividend(DateTime valuationDate, double dDividend);
        void SellShares(DateTime valuationDate, Stock stock);
    }

    internal class InvestmentData : IInvestment
    {
        private UserAccountToken _userToken;
        private IInvestmentRecordInterface _investmentRecordData;
        public InvestmentData(UserAccountToken userToken, string name, double price, IInvestmentRecordInterface investmentRecordData)
        {
            Name = name;
            Price = price;
            _userToken = userToken;
            _investmentRecordData = investmentRecordData;
            CompanyData = _investmentRecordData.GetInvestmentDetails(Name);
        }

        public string Name { get; private set; }
        public InvestmentInformation CompanyData { get; private set; }
        public double Price { get; private set; }

        public void UpdateRow(DateTime valuationDate, DateTime previousDate)
        {
            _investmentRecordData.RollInvestment(_userToken, Name, valuationDate, previousDate);
        }

        public void ChangeShareHolding(DateTime valuationDate, int quantity)
        {
            _investmentRecordData.UpdateInvestmentQuantity(_userToken, Name, valuationDate, quantity);
        }

        public void AddNewShares(DateTime valuationDate, Stock stock)
        {
            _investmentRecordData.AddNewShares(_userToken, Name, stock.Quantity, valuationDate, stock.TotalCost);
        }

        public void UpdateClosingPrice(DateTime valuationDate, double dClosing)
        {
            _investmentRecordData.UpdateClosingPrice(_userToken, Name, valuationDate, dClosing);
        }

        public void UpdateDividend(DateTime valuationDate, double dDividend)
        {
            _investmentRecordData.UpdateDividend(_userToken, Name, valuationDate, dDividend);
        }

        public void SellShares(DateTime valuationDate, Stock stock)
        {
            _investmentRecordData.SellShares(_userToken, Name, stock.Quantity, valuationDate);
        }
    }


}
