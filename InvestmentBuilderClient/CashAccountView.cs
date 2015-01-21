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
    public partial class CashAccountView : Form
    {
        InvestmentDataModel _paymentsModel;

        public CashAccountView()
        {
            InitializeComponent();

            _paymentsModel = new InvestmentDataModel();
            //receiptsBindingSource.DataSource = _paymentsModel.DataSource;
            receiptsGrid.DataSource = _paymentsModel.DataSource; // receiptsBindingSource;
            cmboDate.Items.AddRange(_paymentsModel.GetValuationDates().Cast<object>().ToArray());           
        }

        private void OnSelectedDateChange(object sender, EventArgs e)
        {
           if(cmboDate.SelectedItem != null)
            {
                DateTime? previousDate = cmboDate.SelectedIndex > 0 ? (DateTime?)cmboDate.Items[cmboDate.SelectedIndex - 1] : null;
                DateTime? nextDate = (DateTime?)cmboDate.SelectedItem;
                _paymentsModel.GetData(previousDate, nextDate);
            }
        }

        private void btnGetData_Click(object sender, EventArgs e)
        {
            _paymentsModel.GetData(null, null);
            receiptsGrid.Update();
        }

        protected override void OnClosed(EventArgs e)
        {
            _paymentsModel.Dispose();
            base.OnClosed(e);
        }
    }
}
