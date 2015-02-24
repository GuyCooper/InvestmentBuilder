using System;
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
using System.Threading;
using NLog;

namespace InvestmentBuilderClient.View
{
    internal partial class MainView : Form
    {
        private InvestmentDataModel _dataModel;
        private List<IInvestmentBuilderView> _views;
        private ConfigurationSettings _settings;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        //the one and only log viewer
        private static LoggingView logView;

        private static SynchronizationContext _displayContext;

        public MainView(InvestmentDataModel dataModel, ConfigurationSettings settings)
        {
            InitializeComponent();
            _dataModel = dataModel;
            _views = new List<IInvestmentBuilderView>();
            _settings = settings;

            _displayContext = SynchronizationContext.Current;
        }

        public static void LogMethod(string level, string message)
        {
            if(_displayContext != null)
            {
                _displayContext.Post(o =>
                {
                    if(logView != null)
                    {
                        var kv = (KeyValuePair<string, string>)o;
                        logView.LogMessage(kv.Key, kv.Value);
                    }
                }, new KeyValuePair<string, string>(level,message));
            }
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
            //addthe only logview
            logView = new LoggingView();
            logView.MdiParent = this;
            logView.Show();

            _AddView(new PaymentsDataView(_dataModel));
            _AddView(new TradeView(_settings));
            _AddView(new ReceiptDataView(_dataModel));

            this.WindowState = FormWindowState.Maximized;
            this.LayoutMdi(MdiLayout.TileHorizontal);

            var  logConfig = LogManager.Configuration;
            logger.Log(LogLevel.Info, "welcome to the Investment Builder");

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
                    UpdateValuationDate();
                }
                if (_settings.UpdateTradeFile(configView.GetTradeFile()))
                {
                    _views.OfType<TradeView>().First().ReLoadTrades(_settings.TradeFile);
                }
            }
        }

        private void btnPerformance_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are You Sure?", "Build Charts", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                DateTime dtValuation = (DateTime)cmboValuationDate.SelectedItem;
                PerformanceBuilderLib.PerformanceBuilderExternal.RunBuilder(
                    _settings.OutputFolder,
                    _settings.DatasourceString,
                    dtValuation
                    );
            }
        }
    }
}
