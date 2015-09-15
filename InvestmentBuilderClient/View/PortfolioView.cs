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
    internal partial class PortfolioView : Form, IInvestmentBuilderView
    {
        private PortfolioViewModel _vm;
        private InvestmentDataModel _dataModel;

        public PortfolioView(InvestmentDataModel dataModel)
        {
            InitializeComponent();
            _vm = new PortfolioViewModel(dataModel);
            _dataModel = dataModel;
     
            //add the columns for the company data
            _AddTextColumn("Name", "Name", true);
            _AddTextColumn("Quantity", "Quantity");
            _AddTextColumn("Price/Share", "AveragePricePaid");
            _AddTextColumn("Current Price", "SharePrice");
            _AddTextColumn("Net Value", "NetSellingValue");
            _AddTextColumn("Profit/Loss", "ProfitLoss");
            _AddTextColumn("Month Change %", "MonthChangeRatio");
            _AddTextColumn("Dividend", "Dividend");

            var btnCol = new DataGridViewButtonColumn
            {
                Name = "OnDeal",
                Text = "Deal",
                UseColumnTextForButtonValue = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet,
            };

            _AddTextColumn("Manual Price", "ManualPrice");

            btnCol.CellTemplate.Style.Padding = new Padding(2);

            gridPortfolio.AutoGenerateColumns = false;

            gridPortfolio.Columns.Add(btnCol);

            gridPortfolio.CellClick += OnPortfolioItemClick;
             
        }

        private void OnPortfolioItemClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex !=
                gridPortfolio.Columns["OnDeal"].Index) return;

            //todo - edit trade dialog
            var addView = new AddTradeView(_dataModel, null, _vm.GetTradeItem(e.RowIndex));
            if (addView.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _dataModel.UpdateTrade(addView.GetTrade());
            }
        }
        private void _AddTextColumn(string header, string property, bool bFrozen = false)
        {
            gridPortfolio.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = header,
                    DataPropertyName = property,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet,
                    Frozen = bFrozen,
                });
        }

        public void UpdateValuationDate(DateTime dtValuation)
        {
            //throw new NotImplementedException();
        }

        public void CommitData(DateTime dtValuation)
        {
            //throw new NotImplementedException();
        }

        public void UpdateAccountName(string account)
        {
            _vm.Account = account;
            gridPortfolio.DataSource = _vm.ItemsList; 
        }
    }
}
