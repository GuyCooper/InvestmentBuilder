using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InvestmentBuilderCore;
using MarketDataServices;

namespace InvestmentBuilderClient.View
{
    internal partial class ConfigurationView : Form
    {
        private IConfigurationSettings _settings;
        private IMarketDataSource _marketDataSource;

        public ConfigurationView(IConfigurationSettings settings, IMarketDataSource marketDataSource)
        {
            InitializeComponent();
            _settings = settings;
            _marketDataSource = marketDataSource;
            this.txtDataSource.Text = _settings.DatasourceString;
            this.txtOutputFolder.Text = _settings.OutputFolder;
            foreach(var index in _settings.ComparisonIndexes)
            {
                var lvi = new ListViewItem(index.Name);
                lvi.SubItems.Add(index.Symbol);
                this.listIndexes.Items.Insert(0, lvi);
            }
        }

         private void btnSelectOutputFolder_Click(object sender, EventArgs e)
        {
            if(folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtOutputFolder.Text = folderBrowserDialog1.SelectedPath;
            }
        }

         public string GetOutputFolder()
        {
            return txtOutputFolder.Text;
        }

        public string GetDataSource()
        {
            return txtDataSource.Text;
        }

        public IEnumerable<Index> GetHistoricalIndexes()
        {
            foreach (var item in this.listIndexes.Items.Cast<ListViewItem>())
            {
                if (item.SubItems.Count == 2)
                {
                    yield return new Index
                    {
                        Name = item.SubItems[0].ToString(),
                        Symbol = item.SubItems[1].ToString()
                    };
                }
            }
        }

        private void btnAddIndex_Click(object sender, EventArgs e)
        {
            var addIndexView = new AddIndexView(_marketDataSource);
            if(addIndexView.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var lvi = new ListViewItem(addIndexView.GetName());
                lvi.SubItems.Add(addIndexView.GetSymbol());
                this.listIndexes.Items.Insert(0, lvi);
            }
        }

        private void btnRemoveIndex_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in this.listIndexes.SelectedItems)
            {
                this.listIndexes.Items.Remove(item);
            }
        }
    }
}
