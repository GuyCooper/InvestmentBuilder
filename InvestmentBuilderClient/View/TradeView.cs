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
using NLog;
using InvestmentBuilderCore;
using MarketDataServices;

namespace InvestmentBuilderClient.View
{
    internal partial class TradeView : Form, IInvestmentBuilderView
    {
        TradeViewModel _vm;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        private IMarketDataSource _marketDataSource;
 
        public TradeView(IConfigurationSettings settings, IMarketDataSource marketDataSource, string account)
        {
            InitializeComponent();
            _marketDataSource = marketDataSource;
            ReLoadTrades(settings.GetTradeFile(account));
        }

        public void ReLoadTrades(string tradeFile)
        {
            logger.Log(LogLevel.Info, "loading trades from file {0}", tradeFile);
            _vm = new TradeViewModel(tradeFile);
            tradeViewBindingSource.DataSource = _vm.Trades;
            gridTrades.DataSource = tradeViewBindingSource;
        }

        private void btnAddTradeClick(object sender, EventArgs e)
        {
            var addView = new AddTradeView(_marketDataSource, null);
            if(addView.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _vm.AddTrade(addView.GetTrade());
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

        public void UpdateAccountName(string account)
        {
        }

        public void CommitData(DateTime dtValuation)
        {
           
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _vm.CommitTrades();
        }
     }
}
