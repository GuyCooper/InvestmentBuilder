﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InvestmentBuilderClient.DataModel;
using InvestmentBuilder;

namespace InvestmentBuilderClient.View
{
    internal partial class MainView : Form
    {
        private InvestmentDataModel _dataModel;
        private List<IInvestmentBuilderView> _views;
        private ConfigurationSettings _settings;

        public MainView(InvestmentDataModel dataModel, ConfigurationSettings settings)
        {
            InitializeComponent();
            _dataModel = dataModel;
            _views = new List<IInvestmentBuilderView>();
            _settings = settings;
        }

        private void UpdateValuationDate()
        {
            DateTime  dtValuation = (DateTime)cmboValuationDate.SelectedItem;
            foreach(var view in _views)
            {
                view.UpdateValuationDate(dtValuation);
            }
        }

        private void _AddView(Form view)
        {
            view.MdiParent = this;
            view.Show();
            var investmentView = view as IInvestmentBuilderView;
            if (investmentView != null)
            {
                _views.Add(investmentView);
            }
        }

        private void MainView_Load(object sender, EventArgs e)
        {
            _AddView(new ReceiptDataView(_dataModel));
            _AddView(new PaymentsDataView(_dataModel));
            _AddView(new TradeView(_settings));

            this.WindowState = FormWindowState.Maximized;
            this.LayoutMdi(MdiLayout.TileHorizontal);

            cmboValuationDate.Items.AddRange(_dataModel.GetValuationDates().Cast<object>().ToArray());
            cmboValuationDate.SelectedIndex = 0;
            UpdateValuationDate();
        }

        private void btnCommitData_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are You Sure?", "Commit Data", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                if(Validator.Validate(_views) == false)
                {
                    MessageBox.Show("data validation failure,please check all your values");
                }
                else
                {
                    DateTime dtValuation = (DateTime)cmboValuationDate.SelectedItem;
                    foreach (var view in _views)
                    {
                        view.CommitData(dtValuation);
                    }
                }
            }
        }

        private void btnRunBuilder_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are You Sure?", "Run Builder", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                DateTime  dtValuation = (DateTime)cmboValuationDate.SelectedItem;
                AssetSheetBuilder.BuildAssetSheet(_settings.TradeFile,
                                                  _settings.OutputFolder,
                                                  _settings.DatasourceString,
                                                  false,
                                                  dtValuation,
                                                  DataFormat.DATABASE);
            }
        }

        private void OnValuationDateChanged(object sender, EventArgs e)
        {
            UpdateValuationDate();
        }

        private void btnConfig_Click(object sender, EventArgs e)
        {
            var configView = new ConfigurationView(_settings);
            if(configView.ShowDialog()== System.Windows.Forms.DialogResult.OK)
            {
                if(_settings.UpdateDatasource(configView.GetDataSource()))
                {
                    _dataModel.ReloadData(_settings.DatasourceString);
                }
                if (_settings.UpdateTradeFile(configView.GetTradeFile()))
                {
                    _views.OfType<TradeView>().First().ReLoadTrades(_settings.TradeFile);
                }
            }
        }
    }
}
