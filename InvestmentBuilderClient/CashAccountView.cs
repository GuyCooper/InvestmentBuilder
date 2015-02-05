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
    internal partial class CashAccountView : Form
    {
        InvestmentDataModel _dataModel;
        CashAccountViewModel _vm;

        bool _bInitialised = false;

        public CashAccountView(InvestmentDataModel dataModel)
        {
            InitializeComponent();
            SetupGrid();
            _dataModel = dataModel;
            _vm = new CashAccountViewModel(_dataModel);
            receiptsBindingSource.DataSource = _vm.Receipts;
            receiptsGrid.DataSource = receiptsBindingSource;
            cmboDate.Items.AddRange(_dataModel.GetValuationDates().Cast<object>().ToArray());
            _bInitialised = true;
        }

        private void AddColumn(DataGridView gridView, string name, string propertyName)
        {
            gridView.Columns.Add(new DataGridViewColumn
                {
                    Name = name,
                    DataPropertyName = propertyName,
                    CellTemplate = new DataGridViewTextBoxCell()
                });
        }

        private void SetupGrid()
        {
            receiptsGrid.AutoGenerateColumns = false;
            AddColumn(receiptsGrid, "Transaction Date", "TransactionDate");
            AddColumn(receiptsGrid, "Parameter", "Parameter");
            AddColumn(receiptsGrid, "Subscription", "Subscription");
            AddColumn(receiptsGrid, "Sale", "Sale");
            AddColumn(receiptsGrid, "Dividend", "Dividend");
            AddColumn(receiptsGrid, "Other", "Other");
        }

        private void AddGridStyling()
        {
            //currently just make the last row bold as this is the total row
            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Font = new Font(receiptsGrid.Font, FontStyle.Bold);
            receiptsGrid.Rows[receiptsGrid.Rows.Count - 1].DefaultCellStyle = style;
            receiptsGrid.AutoResizeColumns();
        }

        private void OnValuationDateChanged(object sender, EventArgs e)
        {
            if (_bInitialised)
            {
                DateTime dtValuationDate = (DateTime)cmboDate.SelectedItem;
                var total = _vm.GetReceipts(dtValuationDate);
                txtTotal.Text = total.ToString();
                AddGridStyling();
            }
        }

        private void btnAddReceipt_Click(object sender, EventArgs e)
        {
            var view = new AddTransactionView(_dataModel, "R");
            if(view.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var total  = _vm.AddReceipt(view.GetTransactionDate(), view.GetTransactionType(),
                               view.GetParameter(), view.GetAmount());
                txtTotal.Text = total.ToString();
                AddGridStyling();
            }
        }

        private void btnDeleteReceipt_Click(object sender, EventArgs e)
        {
            foreach(DataGridViewRow row in receiptsGrid.SelectedRows)
            {
                var transaction = row.DataBoundItem as Transaction;
                if(!transaction.Added)
                {
                    MessageBox.Show("Can only delete transactions that have not been comitted");
                }
                else
                {
                    var total = _vm.DeleteTransaction(transaction);
                    txtTotal.Text = total.ToString();
                    AddGridStyling();
                }
            }
        }

        private void OnSelectedTransactionChanged(object sender, EventArgs e)
        {
            btnDeleteReceipt.Enabled = true;
        }
    }
}
