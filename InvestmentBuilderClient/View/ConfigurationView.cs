using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InvestmentBuilderClient.View
{
    internal partial class ConfigurationView : Form
    {
        public ConfigurationView(ConfigurationSettings settings)
        {
            InitializeComponent();
            this.txtTradeFile.Text = settings.TradeFile;
            this.txtDataSource.Text = settings.DatasourceString;
            this.txtOutputFolder.Text = settings.OutputFolder;
        }

        private void btnSelectTradeFile_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtTradeFile.Text = openFileDialog1.FileName;
            }
        }

        private void btnSelectOutputFolder_Click(object sender, EventArgs e)
        {
            if(folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtOutputFolder.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        public string GetTradeFile()
        {
            return txtTradeFile.Text;
        }

        public string GetOutputFolder()
        {
            return txtOutputFolder.Text;
        }

        public string GetDataSource()
        {
            return txtDataSource.Text;
        }
    }
}
