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
    internal partial class ManageUserView : Form
    {
        private IInvestmentDataModel _dataModel;
        public ManageUserView(string account, IInvestmentDataModel dataModel)
        {
            _dataModel = dataModel;
            InitializeComponent();
            //chkEnableAccount.Checked = true;
            InitialiseFromData(_dataModel.GetAccountData(account));
            cmboType.Items.AddRange(_dataModel.GetAccountTypes().ToArray<object>());
        }

        public IList<string> GetMembers()
        {
            return listMembers.Items.Cast<string>().ToList();
        }

        public string GetAccountName()
        {
            return txtAccountName.Text;
        }

        public bool GetIsEnabled()
        {
            return chkEnableAccount.Checked;
        }

        public string GetDescription()
        {
            return txtDescription.Text;
        }

        public string GetPassword()
        {
            return txtPassword.Text;
        }

        public string GetAccountType()
        {
            return cmboType.SelectedItem as string;
        }

        public string GetCurrency()
        {
            return txtCurrency.Text;
        }

        private void btnAddMember_Click(object sender, EventArgs e)
        {
            var addMemberView = new AddMember();
            if(addMemberView.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                listMembers.Items.Add(addMemberView.GetName());
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if(listMembers.SelectedItem != null)
            {
                listMembers.Items.Remove(listMembers.SelectedItem);
            }
        }

        private void InitialiseFromData(AccountModel modelData)
        {
            txtDescription.Text = string.Empty;
            txtPassword.Text = string.Empty;
            txtCurrency.Text = string.Empty;
            listMembers.Items.Clear();
            if (modelData != null)
            {
                if(string.IsNullOrEmpty(txtAccountName.Text))
                {
                    txtAccountName.Text = modelData.Name;
                }
                txtDescription.Text = modelData.Description;
                txtPassword.Text = modelData.Password;
                txtCurrency.Text = modelData.ReportingCurrency;
                chkEnableAccount.Checked = modelData.Enabled;
                listMembers.BeginUpdate();
                foreach (var name in modelData.Members)
                {
                    listMembers.Items.Add(name);
                }
                listMembers.EndUpdate();
            }
        }

        private void txtAccountName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == '\r')
            {
                InitialiseFromData(_dataModel.GetAccountData(txtAccountName.Text));
                e.Handled = true;
            }
        }
    }
}