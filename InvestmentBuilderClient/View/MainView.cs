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
using InvestmentBuilderCore;
using PerformanceBuilderLib;
using MarketDataServices;

namespace InvestmentBuilderClient.View
{
    internal partial class MainView : Form
    {
        private InvestmentDataModel _dataModel;
        private List<IInvestmentBuilderView> _views;
        private IConfigurationSettings _settings;
        private IMarketDataSource _marketDataSource;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        //the one and only log viewer
        private static LoggingView logView;

        private static SynchronizationContext _displayContext;

        public MainView(InvestmentDataModel dataModel, IConfigurationSettings settings, IMarketDataSource marketDataSource)
        {
            InitializeComponent();
            _dataModel = dataModel;
            _marketDataSource = marketDataSource;
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
            DateTime dtValuation = _GetSelectedValuationDate();
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

            var logConfig = LogManager.Configuration;
            logger.Log(LogLevel.Info, "welcome to the Investment Builder");

            InitialiseValues();

            _AddView(new PaymentsDataView(_dataModel));
            _AddView(new TradeView(_settings, _marketDataSource, cmboAccountName.SelectedItem as string));
            _AddView(new ReceiptDataView(_dataModel));

            UpdateValuationDate();

            this.WindowState = FormWindowState.Maximized;
            this.LayoutMdi(MdiLayout.TileHorizontal);

        }

        private void PopulateValuationDates()
        {
            cmboValuationDate.Items.Clear();
            cmboValuationDate.Items.AddRange(_dataModel.GetValuationDates().Cast<object>().ToArray());
            if (cmboValuationDate.Items.Count > 0)
            {
                cmboValuationDate.SelectedIndex = 0;
            }
        }

        private void InitialiseValues()
        {
            cmboAccountName.Items.Clear();
            cmboAccountName.Items.AddRange(_dataModel.GetAccountNames().Cast<object>().ToArray());
            cmboAccountName.SelectedIndex = 0;

            _dataModel.UpdateAccountName(cmboAccountName.SelectedItem as string);

            //PopulateValuationDates();        
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
                    DateTime dtValuation = _GetSelectedValuationDate();
                    foreach (var view in _views)
                    {
                        view.CommitData(dtValuation);
                    }
                }
            }
        }

        private void btnRunBuilder_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are You Sure?", "Run Accounts Builder", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                DateTime dtValuation = _GetSelectedValuationDate();
                string selectedAccount = (string)cmboAccountName.SelectedItem;

                Task.Factory.StartNew(() =>
                    {

                        var report = ContainerManager.ResolveValue<InvestmentBuilder.InvestmentBuilder>()
                                               .BuildAssetReport(selectedAccount, dtValuation, true);
                        DisplayAssetReport(report);
                    });
            }
        }

        private void OnValuationDateChanged(object sender, EventArgs e)
        {
            UpdateValuationDate();
        }

        private void btnConfig_Click(object sender, EventArgs e)
        {
            var configView = new ConfigurationView(_settings, _marketDataSource);
            if (configView.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (_settings.UpdateDatasource(configView.GetDataSource()))
                {
                    _dataModel.ReloadData(_settings.DatasourceString);
                    InitialiseValues();
                    UpdateValuationDate();
                }

                _settings.UpdateComparisonIndexes(configView.GetHistoricalIndexes());

                if(_settings.UpdateOutputFolder(configView.GetOutputFolder()))
                {
                    logger.Log(LogLevel.Info,"changed output folder to {0}", _settings.OutputFolder);
                    _views.OfType<TradeView>().First().ReLoadTrades(_settings.GetTradeFile(cmboAccountName.SelectedItem as string));
                }
            }
        }

        private void btnPerformance_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are You Sure?", "Build Charts", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                DateTime dtValuation = _GetSelectedValuationDate();
                string account = (string)cmboAccountName.SelectedItem;
                Task.Factory.StartNew(() =>
                    {
                        var performanceData = ContainerManager.ResolveValue<PerformanceBuilder>().Run(account, dtValuation);
                        if(performanceData != null && _displayContext != null)
                        {
                            _displayContext.Post(o =>
                            {
                                var data = (IList<IndexedRangeData>)o;
                                foreach(var range in data)
                                {
                                    var chartView = new PerformanceChartView(range);
                                    chartView.TopLevel = true;
                                    chartView.Show();
                                }
                            },performanceData);
                        }
                    });
            }
        }

        private void cmboAccountName_SelectedIndexChanged(object sender, EventArgs e)
        {
            _dataModel.UpdateAccountName(cmboAccountName.SelectedItem as string);
            PopulateValuationDates();
            UpdateValuationDate();
            var tradeView = _views.OfType<TradeView>().FirstOrDefault();
            if(tradeView != null)
            {
                tradeView.ReLoadTrades(_settings.GetTradeFile(cmboAccountName.SelectedItem as string));
            }
        }

        private void btnViewReport_Click(object sender, EventArgs e)
        {
            DateTime dtValuation = _GetSelectedValuationDate();
            string selectedAccount = (string)cmboAccountName.SelectedItem;

            if(_dataModel.IsExistingValuationDate(dtValuation))
            {
                //marshall view builder onto a seperate thread
                Task.Factory.StartNew(() =>
                    {
                        var report = ContainerManager.ResolveValue<InvestmentBuilder.InvestmentBuilder>()
                                         .BuildAssetReport(selectedAccount, dtValuation, false);
                        DisplayAssetReport(report);
                    });
            }
            else
            {
                logger.Log(LogLevel.Warn, "no report available for date {0}", dtValuation.ToShortDateString());
            }
        }

        private void DisplayAssetReport(AssetReport report)
        {
            if (report != null)
            {
                //now marshall back onto main thread to display report
                if (_displayContext != null)
                {
                    _displayContext.Post(o =>
                    {
                        var reportView = new AssetReportView((AssetReport)o);
                        reportView.TopLevel = true;
                        reportView.Show();
                    }, report);
                }
            }
        }

        private void btnManageUsers_Click(object sender, EventArgs e)
        {
            var view = new ManageUserView(cmboAccountName.SelectedItem as string, _dataModel);
            if(view.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var account = new AccountModel
                {
                    Name = view.GetAccountName(),
                    Description = view.GetDescription(),
                    Password = view.GetPassword(),
                    Type = view.GetAccountType(),
                    Enabled = view.GetIsEnabled(),
                    Members = view.GetMembers(),
                    ReportingCurrency = view.GetCurrency()
                };

                logger.Log(LogLevel.Info, "updating account {0}", account.Name);
                //ensure the account folder has been created 
                _settings.GetOutputPath(account.Name);
                _dataModel.UpdateUserAccount(account);
                
                InitialiseValues();
                PopulateValuationDates();
            }
        }

        private void OnValuationDateEnter(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == '\r')
            {
                UpdateValuationDate();
            }
        }

        private DateTime _GetSelectedValuationDate()
        {
            return cmboValuationDate.SelectedItem != null ? (DateTime)cmboValuationDate.SelectedItem :
                                                            DateTime.Parse(cmboValuationDate.Text);
        }

        private void btnOpenOutputFolder_Click(object sender, EventArgs e)
        {
            var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "explorer.exe";
            process.StartInfo.Arguments = _settings.GetOutputPath(cmboAccountName.SelectedItem as string);
            process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            process.Start();
        }
    }
}
