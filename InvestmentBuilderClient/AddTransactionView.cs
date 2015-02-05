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
    internal partial class AddTransactionView : Form
    {
        InvestmentDataModel _dataModel;
        string _side;

        public AddTransactionView(InvestmentDataModel dataModel, string side)
        {
            _dataModel = dataModel;
            InitializeComponent();
            _side = side;
        }

        private void AddTransactionView_Load(object sender, EventArgs e)
        {
            cmboType.Items.AddRange(_dataModel.GetsTransactionTypes(_side).ToArray());
        }

        private void OnPaymentTypeChanged(object sender, EventArgs e)
        {
            cmboParameters.Items.Clear();
            var type = (string)cmboType.SelectedItem;
            cmboParameters.Items.AddRange(_dataModel.GetParametersForType(type).ToArray());
            cmboParameters.SelectedIndex = 0;
        }

        public DateTime GetTransactionDate()
        {
            return dteTransaction.Value;
        }

        public string GetTransactionType()
        {
            return (string)cmboType.SelectedItem;
        }

        public string GetParameter()
        {
            return (string)cmboParameters.SelectedItem;
        }

        public double GetAmount()
        {
            return double.Parse(txtAmount.Text);
        }
    }
}
