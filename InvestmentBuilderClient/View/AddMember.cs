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

namespace InvestmentBuilderClient.View
{
    internal partial class AddMember : Form
    {
        public AddMember(string member)
        {
            InitializeComponent();

            if(string.IsNullOrEmpty(member) == false)
            {
                this.txtName.Text = member;
                this.txtName.ReadOnly = true;
                this.Text = "Edit Member";
            }

            foreach(var name in Enum.GetNames(typeof(AuthorizationLevel)))
            {
                cmboAuthorization.Items.Add(name);
            }
        }

        public string GetName()
        {
            return txtName.Text;
        }

        public AuthorizationLevel GetAuthorization()
        {
            return (AuthorizationLevel)Enum.Parse(typeof(AuthorizationLevel), cmboAuthorization.SelectedItem as string);
        }
    }
}
