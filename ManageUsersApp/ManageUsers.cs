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
using SQLServerDataLayer;

namespace ManageUsersApp
{
    public partial class ManageUsers : Form
    {
        public ManageUsers()
        {
            InitializeComponent();
        }

        private void btnCreateUser_Click(object sender, EventArgs e)
        {
            if(txtPassword.Text != txtPasswordConfirm.Text)
            {
                MessageBox.Show("Passwords do not match", "Add New User", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var settings = new ConfigurationSettings("InvestmentBuilderConfig.xml");
            var authData = new SQLAuthData(settings);
            var salt = SaltedHash.GenerateSalt();
            var result = authData.AddNewUser(txtUserName.Text,
                                txtEmailAddress.Text,
                                salt,
                                SaltedHash.GenerateHash(txtPassword.Text, salt),
                                txtPhoneNumber.Text,
                                chkEnableTwoFactor.Checked);

            string message = "";
            switch(result)
            {
                case 0:
                    message = "User added successfully!";
                    break;
                case 1:
                    message = "invalid email";
                    break;
                case 2:
                    message = "invalid password";
                    break;
                case 3:
                    message = "user already exists";
                    break;
            }
            MessageBox.Show(message, "Add User", MessageBoxButtons.OK, result == 0 ? MessageBoxIcon.Information : MessageBoxIcon.Error);
        }
    }
}
