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
    internal partial class AddMember : Form
    {
        public AddMember()
        {
            InitializeComponent();
        }

        public string GetName()
        {
            return txtName.Text;
        }
    }
}
