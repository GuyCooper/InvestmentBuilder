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

namespace InvestmentBuilderClient.View
{
    internal partial class AddTransactionView : Form
    {
        InvestmentDataModel _dataModel;
        string _side;

        private const string ALL = "All";

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
            cmboParameters.Items.Add(ALL);
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

        public IEnumerable<string> GetParameter()
        {
            var selectedItem = (string)cmboParameters.SelectedItem;
            if(selectedItem == ALL)
            {
                foreach (var item in cmboParameters.Items)
                {
                    var subItem = (string)item;
                    if(selectedItem != subItem)
                    {
                        yield return subItem;

                    }
                }
            }
            else
            {
                yield return selectedItem;
            }
        }

        public double GetAmount()
        {
            return double.Parse(txtAmount.Text);
        }

        public bool ValidateTransaction()
        {
            double dResult;
            return double.TryParse(txtAmount.Text, out dResult);
        }
    }
}
