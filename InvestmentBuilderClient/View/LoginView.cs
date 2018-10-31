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
    /// <summary>
    /// Loginview class. allows user to enter the user name
    /// </summary>
    public partial class LoginView : Form
    {
        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        public LoginView(string username, string password)
        {
            InitializeComponent();
            txtUsername.Text = username;
            txtPassword.Text = password;
        }

        /// <summary>
        /// return the entered username.
        /// </summary>
        public string GetUserName()
        {
            return txtUsername.Text;
        }

        #endregion

    }
}
