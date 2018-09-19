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
using InvestmentBuilderCore;

namespace InvestmentBuilderClient.View
{
    internal partial class RedemptionsView : Form, IInvestmentBuilderView
    {
        private InvestmentDataModel _dataModel;

        DateTime _dtValuation;

        public RedemptionsView(InvestmentDataModel dataModel, DateTime dtValuation)
        {
            InitializeComponent();

            _AddTextColumn("User", "User");
            _AddTextColumn("Amount","Amount");
            _AddTextColumn("TransactionDate","TransactionDate");
            _AddTextColumn("Status", "Status");

            _dataModel = dataModel;

            UpdateValuationDate(dtValuation);
            
        }

    private void _AddTextColumn(string header, string property, bool bFrozen = false)
        {
            gridRedemptions.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = header,
                    DataPropertyName = property,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet,
                    Frozen = bFrozen,
                });

        }

        private void btnAddRedemption_Click(object sender, EventArgs e)
        {
            var addRedemptionView = new AddRedemptionView(_dataModel);
            if(addRedemptionView.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string user = addRedemptionView.GetSelectedUser();
                double? dAmount = addRedemptionView.GetAmount();
                if(user != null && dAmount.HasValue)
                {
                    if(_dataModel.RequestRedemption(user, dAmount.Value, _dtValuation) == false)
                    {
                        MessageBox.Show("Failed to create redemption request, check log for details");
                    }
                    else
                    {
                        gridRedemptions.DataSource = _dataModel.GetRedemptions(_dtValuation);
                    }
                }
            }
        }

        public void UpdateValuationDate(DateTime dtValuation)
        {
            _dtValuation = dtValuation.Date;
            gridRedemptions.DataSource = _dataModel.GetRedemptions(_dtValuation);
        }

        public void UpdateAccountName(AccountIdentifier account)
        {
            //throw new NotImplementedException();
        }

        public void CommitData(DateTime dtValuation)
        {
            //throw new NotImplementedException();
        }
    }
}
