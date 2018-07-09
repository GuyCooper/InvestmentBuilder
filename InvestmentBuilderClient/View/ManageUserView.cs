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
    internal partial class ManageUserView : Form
    {
        private InvestmentDataModel _dataModel;
        public ManageUserView(string account, InvestmentDataModel dataModel)
        {
            _dataModel = dataModel;
            InitializeComponent();
            //chkEnableAccount.Checked = true;
            cmboType.Items.AddRange(_dataModel.GetAccountTypes().ToArray<object>());
            cmboBroker.Items.AddRange(_dataModel.GetBrokers().ToArray<object>());
            cmboBroker.Items.Add("other");
            InitialiseFromData(_dataModel.GetAccountData(account));
        }

        public IList<AccountMember> GetMembers()
        {
            //return lstVwMembers.Items.Cast<KeyValuePair<string, AuthorizationLevel>>().ToList();
            var result = new List<AccountMember>();
            foreach(ListViewItem item in lstVwMembers.Items)
            {
                result.Add(
                    new AccountMember(
                                    item.Text,
                                    (AuthorizationLevel)Enum.Parse(typeof(AuthorizationLevel), item.SubItems[1].Text)));
            }
            return result;
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

        public string GetBroker()
        {
            return cmboBroker.SelectedItem as string;
        }

        private void btnAddMember_Click(object sender, EventArgs e)
        {
            var addMemberView = new AddMember(null);
            if(addMemberView.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _addMember(addMemberView.GetName(), addMemberView.GetAuthorization());
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if(this.lstVwMembers.SelectedIndices.Count > 0)
            {
                foreach(var selected in lstVwMembers.SelectedIndices)
                {
                    this.lstVwMembers.Items.RemoveAt((int)selected);
                }
            }
        }

        private void _addMember(string member, AuthorizationLevel level)
        {
            var matched = this.lstVwMembers.Items.Find(member, false);
            if (matched.Length > 0)
            {
                if (matched[0].SubItems.Count > 1)
                    matched[0].SubItems[1].Text = level.ToString();
                else
                    matched[0].SubItems.Add(level.ToString());
            }
            else
            {
                var lvi = new ListViewItem(member) { Name = member };
                lvi.SubItems.Add(level.ToString());
                this.lstVwMembers.Items.Insert(0, lvi);
            }
        }

        private void InitialiseFromData(AccountModel modelData)
        {
            txtDescription.Text = string.Empty;
            txtPassword.Text = string.Empty;
            txtCurrency.Text = string.Empty;
            this.lstVwMembers.Items.Clear();
            if (modelData != null)
            {
                if(string.IsNullOrEmpty(txtAccountName.Text))
                {
                    txtAccountName.Text = modelData.Name;
                }
                txtDescription.Text = modelData.Description;
                //txtPassword.Text = modelData.Password;
                txtCurrency.Text = modelData.ReportingCurrency;
                chkEnableAccount.Checked = modelData.Enabled;
                cmboType.SelectedItem = modelData.Type;

                this.lstVwMembers.BeginUpdate();
                foreach (var member in modelData.Members)
                {
                    _addMember(member.Name, member.AuthLevel);
                }
                this.lstVwMembers.EndUpdate();
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

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (this.lstVwMembers.SelectedIndices.Count > 0)
            {
                int index = (int)this.lstVwMembers.SelectedIndices[0];
                var member = this.lstVwMembers.Items[index].Text; 
                var addMemberView = new AddMember(member);
                if (addMemberView.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _addMember(addMemberView.GetName(), addMemberView.GetAuthorization());
                }
            }
        }
    }
}