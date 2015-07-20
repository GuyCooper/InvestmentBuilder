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

namespace InvestmentBuilderClient.View
{
    internal partial class AddTradeView : Form
    {
        private IMarketDataSource _marketDataSource;

        public AddTradeView(IMarketDataSource marketDataSource, TradeDetails trade)
        {
            _marketDataSource = marketDataSource;
            InitializeComponent();
            if(trade != null)
            {
                this.Text = "Edit Trade";
                dteTransactionDate.Value = DateTime.Parse(trade.TransactionDate);
                txtName.Text = trade.Name;
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

        public string GetTransactionDate()
        {
            return dteTransactionDate.Value.ToShortDateString();   
        }

        public string GetName()
        {
            return txtName.Text;
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
            double dPrice;
            if(_marketDataSource.TryGetMarketData(GetSymbol(), GetExchange(), out dPrice))
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
    }
}
