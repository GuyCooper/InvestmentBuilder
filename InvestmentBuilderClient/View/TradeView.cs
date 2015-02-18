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
using InvestmentBuilderClient.DataModel;

namespace InvestmentBuilderClient.View
{
    internal partial class TradeView : Form, IInvestmentBuilderView
    {
        TradeViewModel _vm;

        public TradeView(ConfigurationSettings settings)
        {
            InitializeComponent();
            ReLoadTrades(settings.TradeFile);
        }

        public void ReLoadTrades(string tradeFile)
        {
            _vm = new TradeViewModel(tradeFile);
            tradeViewBindingSource.DataSource = _vm.Trades;
            gridTrades.DataSource = tradeViewBindingSource;
        }

        private void btnAddTradeClick(object sender, EventArgs e)
        {
            var addView = new AddTradeView();
            if(addView.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _vm.AddTrade(new TradeDetails
                    {
                         TransactionDate = addView.GetTransactionDate(),
                         Currency = addView.GetCurrency(),
                         Name = addView.GetName(),
                         Number = addView.GetAmount(),
                         Type  = addView.GetTradeType(),
                         Symbol = addView.GetSymbol(),
                         ScalingFactor = addView.GetScalingFactor(),
                         TotalCost = addView.GetTotalCost()
                    });
            }
        }

        private void btnRemoveTrade_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in gridTrades.SelectedRows)
            {
                var trade = row.DataBoundItem as TradeDetails;
                if(trade != null)
                {
                    _vm.RemoveTrade(trade);
                }
            }
        }

        public void UpdateValuationDate(DateTime dtValuation)
        {
        }

        public void CommitData(DateTime dtValuation)
        {
            _vm.CommitTrades();
        }
     }
}
