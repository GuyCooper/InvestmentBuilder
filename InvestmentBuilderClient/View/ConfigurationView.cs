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

namespace InvestmentBuilderClient.View
{
    internal partial class ConfigurationView : Form
    {
        private IConfigurationSettings _settings;
        public ConfigurationView(IConfigurationSettings settings)
        {
            InitializeComponent();
            _settings = settings;
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
    }
}
