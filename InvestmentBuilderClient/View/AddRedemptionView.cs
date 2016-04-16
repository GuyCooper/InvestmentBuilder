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
    internal partial class AddRedemptionView : Form
    {
        public AddRedemptionView(InvestmentDataModel dataModel)
        {
            InitializeComponent();

            cmboUsers.Items.AddRange(dataModel.GetParametersForType("Subscription")
                .Cast<object>().ToArray());
            if (cmboUsers.Items.Count > 0)
            {
                cmboUsers.SelectedIndex = 0;
            }
        }

        public string GetSelectedUser()
        {
            return cmboUsers.SelectedItem as string;
        }

        public double? GetAmount()
        {
            double dAmount;
            if(Double.TryParse(txtAmount.Text, out dAmount) == false)
            {
                MessageBox.Show("Invalid Amount Entered!!!!");
                return null;
            }
            return dAmount;
        }
    }
}
