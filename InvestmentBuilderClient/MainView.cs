using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InvestmentBuilderClient
{
    internal partial class MainView : Form
    {
        private InvestmentDataModel _dataModel;

        public MainView(InvestmentDataModel dataModel)
        {
            InitializeComponent();
            _dataModel = dataModel;
        }

        private void MainView_Load(object sender, EventArgs e)
        {
            var receiptView = new ReceiptDataView(_dataModel);
            receiptView.MdiParent = this;
            receiptView.Show();
            this.LayoutMdi(MdiLayout.TileHorizontal);
        }
    }
}
