using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderClient.DataModel;
using InvestmentBuilderCore;
using NLog;

namespace InvestmentBuilderClient.ViewModel
{
    internal class PortfolioViewModel
    {
        private InvestmentDataModel _dataModel;

        public List<CompanyData> ItemsList { get { return _dataModel.PortfolioItemsList; } }

        private string _account;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public PortfolioViewModel(InvestmentDataModel dataModel)
        {
            _dataModel = dataModel;
        }

        public string Account
        {
            get { return _account; }
            set
            {
                _account = value;
                _dataModel.LoadPortfolioItems();
            }
        }

        /// <summary>
        /// convert the comapnydata item specified by this index as a
        /// TradeDetails item
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TradeDetails GetTradeItem(int index)
        {
            if (index > ItemsList.Count)
                return null;

            var item = ItemsList[index];
            var itemDetails = _dataModel.GetInvestmentDetails(item.Name);
            if(itemDetails != null)
            {
                return new TradeDetails
                {
                    Action = TradeType.SELL,
                    Currency = itemDetails.Currency,
                    Exchange = itemDetails.Exchange,
                    Name = item.Name,
                    Quantity = item.Quantity,
                    ScalingFactor = itemDetails.ScalingFactor,
                    Symbol = itemDetails.Symbol,
                    TotalCost = item.TotalCost,
                    //TransactionDate = item.LastBrought.HasValue ? item.LastBrought.Value.ToShortDateString() : DateTime.Now.ToShortDateString()
                    TransactionDate = item.LastBrought.ToShortDateString()
                };
            }
            return null;
        }
    }
}
