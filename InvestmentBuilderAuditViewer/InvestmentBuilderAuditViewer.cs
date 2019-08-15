using InvestmentBuilderAuditLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace InvestmentBuilderAuditViewer
{
    /// <summary>
    /// Forms displays all the audit entries from a SQLite dat file.
    /// </summary>
    public partial class InvestmentBuilderAuditViewer : Form
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public InvestmentBuilderAuditViewer()
        {
            InitializeComponent();

            AddColumnToView("Time", "AuditTime");

        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Helper method for adding a column to the view.
        /// </summary>
        private void AddColumnToView(string name, string property)
        {
            dataGridAuditLog.Columns.Add(new DataGridViewColumn
            {
                Name = name,
                DataPropertyName = property,
                CellTemplate = new DataGridViewTextBoxCell()
            });
        }

        /// <summary>
        /// Called when user clicks the open file button
        /// </summary>
        private void OnOpenFileButtonClick(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtFileName.Text = openFileDialog1.FileName;
            }
        }

        /// <summary>
        /// Load the audit log from the file
        /// </summary>
        private void btnLoad_Click(object sender, EventArgs e)
        {
            if(m_messageReader == null)
            {
                if(string.IsNullOrEmpty(txtFileName.Text) == true)
                {
                    MessageBox.Show("Please specify a valid file.");
                    return;
                }

                m_messageReader = new SQLiteAuditMessageReader(txtFileName.Text);
            }

            m_datasource = m_messageReader.GetMessages().ToList();

            dataGridAuditLog.DataSource = m_datasource;
        }

        #endregion

        #region Private Data

        //Datasource.
        private SQLiteAuditMessageReader m_messageReader;

        private IList<AuditMessage> m_datasource;

        #endregion

        private void InvestmentBuilderAuditViewer_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_messageReader?.Dispose();
        }
    }
}
