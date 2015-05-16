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

namespace InvestmentBuilderClient.View
{
    internal partial class AddTradeView : Form
    {
        private IMarketDataSource _marketDataSource;

        public AddTradeView(IMarketDataSource marketDataSource)
        {
            _marketDataSource = marketDataSource;
            InitializeComponent();
            cmboType.Items.AddRange(Enum.GetNames(typeof(TradeType)));
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
    }
}
