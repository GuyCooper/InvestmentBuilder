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
    internal partial class LoggingView : Form
    {
        public LoggingView()
        {
            InitializeComponent();
        }

        public void LogMessage(string loglevel, string message)
        {
            var lvi = new ListViewItem(loglevel);
            lvi.SubItems.Add(message);
            listLogging.Items.Add(lvi);
        }
    }
}
