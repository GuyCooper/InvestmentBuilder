using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InvestmentBuilderClient.ViewModel;
using MarketDataServices;
using InvestmentBuilderClient.DataModel;
using InvestmentBuilderCore;

namespace InvestmentBuilderClient.View
{
    internal partial class AddTradeView : Form
    {
        private IMarketDataSource _marketDataSource;
        private InvestmentDataModel _dataModel;

        public AddTradeView(InvestmentDataModel dataModel, IMarketDataSource marketDataSource, TradeDetails trade)
        {
            _marketDataSource = marketDataSource;
            _dataModel = dataModel;
            InitializeComponent();

            //populate investment names combo
            cmboName.Items.AddRange(_dataModel.GetAllCompanies().ToArray());

            if(trade != null)
            {
                this.Text = "Edit Trade";
                dteTransactionDate.Value = DateTime.Now;
                cmboName.SelectedText = trade.Name;
                cmboName.Text = trade.Name;
                nmrcNumber.Value = trade.Quantity;
                nmrcScaling.Value = (decimal)trade.ScalingFactor;
                txtSymbol.Text = trade.Symbol;
                txtExchange.Text = trade.Exchange;
                txtCcy.Text = trade.Currency;
                txtTotalCost.Text = trade.TotalCost.ToString();
                cmboType.Items.AddRange(Enum.GetNames(typeof(TradeType)));
                cmboType.SelectedItem = trade.Action;
            }
            else
            {
                cmboType.Items.AddRange(Enum.GetNames(typeof(TradeType)));
            }
            btnCheck.Enabled = _marketDataSource != null;
        }

        public TradeDetails GetTrade()
        {
            return new TradeDetails
            {
                TransactionDate = GetTransactionDate(),
                Currency = GetCurrency(),
                Name = GetName(),
                Quantity = GetAmount(),
                Action = GetTradeType(),
                Symbol = GetSymbol(),
                Exchange = GetExchange(),
                ScalingFactor = GetScalingFactor(),
                TotalCost = GetTotalCost(),
                ManualPrice = GetManualPrice()
            };
        }

        public DateTime GetTransactionDate()
        {
            return dteTransactionDate.Value;   
        }

        public string GetName()
        {
            if(string.IsNullOrEmpty(cmboName.SelectedText))
            {
                if (string.IsNullOrEmpty(cmboName.Text))
                {
                    return cmboName.SelectedItem as string;
                }
                return cmboName.Text;
            }
            return cmboName.SelectedText;
        }

        public TradeType GetTradeType()
        {
            return (TradeType)Enum.Parse(typeof(TradeType),(string)cmboType.SelectedItem);
        }

        public int GetAmount()
        {
            return (int)nmrcNumber.Value;
        }

        public string GetSymbol()
        {
            return txtSymbol.Text;
        }

        public string GetExchange()
        {
            return txtExchange.Text;
        }

        public string GetCurrency()
        {
            return txtCcy.Text;
        }

        public int GetScalingFactor()
        {
            return nmrcScaling.Value > 0 ? (int) nmrcScaling.Value : (int)1;
        }

        public double GetTotalCost()
        {
            return Double.Parse(txtTotalCost.Text);
        }

        public double? GetManualPrice()
        {
            double dPrice;
            if(Double.TryParse(txtManualPrice.Text, out dPrice) == true)
            {
                return dPrice;
            }
            return null;
        }
        private void btnCheck_Click(object sender, EventArgs e)
        {
            MarketDataPrice marketData;
            if(_marketDataSource.TryGetMarketData(GetSymbol(), GetExchange(), out marketData))
            {
                lblCheckResult.Text = "Success. Valid Symbol!";
            }
            else
            {
                lblCheckResult.Text = "Fail. Invalid Symbol!";
            }
        }

        private void cmboType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedType = (TradeType)Enum.Parse(typeof(TradeType), cmboType.SelectedItem.ToString());
            if(selectedType == TradeType.SELL)
            {
                chkSellAll.Enabled = true;
            }
            else
            {
                chkSellAll.Enabled = false;
            }
        }

        private void cmboName_SelectedIndexChanged(object sender, EventArgs e)
        {
            var investmentDetails = _dataModel.GetInvestmentDetails(cmboName.SelectedItem as string);
            if(investmentDetails != null)
            {
                nmrcScaling.Value = (decimal)investmentDetails.ScalingFactor;
                txtSymbol.Text = investmentDetails.Symbol;
                txtExchange.Text = investmentDetails.Exchange;
                txtCcy.Text = investmentDetails.Currency;
            }
        }
    }
}
