using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MarketDataServices;

namespace InvestmentBuilderClient.View
{
    public partial class AddIndexView : Form
    {
        private IMarketDataSource _marketDataSource;

        public AddIndexView(IMarketDataSource marketDataSource)
        {
            _marketDataSource = marketDataSource;
            InitializeComponent();
        }

        public string GetName()
        {
            return txtName.Text;
        }

        public string GetSymbol()
        {
            return txtSymbol.Text;
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            lblCheckResult.Text = "";
            if (!string.IsNullOrEmpty(txtSymbol.Text))
            {
                var historicalData = _marketDataSource.GetHistoricalData(txtSymbol.Text, DateTime.Today.AddMonths(-1));
                if (historicalData != null)
                    lblCheckResult.Text = "Success. Valid Index!!";
                else
                    lblCheckResult.Text = "Fail. Invalid index!!!";
            }
        }
    }
}
