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
    internal abstract partial class CashAccountView : Form
    {
        protected InvestmentDataModel _dataModel;
        protected CashAccountViewModel _vm;

        bool _bInitialised = false;

        public CashAccountView(InvestmentDataModel dataModel)
        {
            InitializeComponent();
            SetupGrid();
            SetupDataSource(dataModel);
            cashAccountGrid.DataSource = cashAccountBindingSource;
            _dataModel = dataModel;
            cmboDate.Items.AddRange(_dataModel.GetValuationDates().Cast<object>().ToArray());
            cmboDate.SelectedIndex = 0;
            _GetCashAccountData();            
            _bInitialised = true;
        }

        protected abstract void SetupDataSource(InvestmentDataModel dataModel);

        protected abstract string TransactionMnenomic { get; }

        protected void AddColumn(DataGridView gridView, string name, string propertyName)
        {
            gridView.Columns.Add(new DataGridViewColumn
                {
                    Name = name,
                    DataPropertyName = propertyName,
                    CellTemplate = new DataGridViewTextBoxCell()
                });
        }

        protected abstract void SetupGrid();
       
        private void AddGridStyling()
        {
            //currently just make the last row bold as this is the total row
            if (cashAccountGrid.Rows.Count > 0)
            {
                DataGridViewCellStyle style = new DataGridViewCellStyle();
                style.Font = new Font(cashAccountGrid.Font, FontStyle.Bold);
                cashAccountGrid.Rows[cashAccountGrid.Rows.Count - 1].DefaultCellStyle = style;
                cashAccountGrid.AutoResizeColumns();
            }
        }

        private void _GetCashAccountData()
        {
            DateTime dtValuationDate = (DateTime)cmboDate.SelectedItem;
            var total = GetCashAccountDataImpl(dtValuationDate);
            txtTotal.Text = total.ToString();
            AddGridStyling();
        }

        protected abstract double GetCashAccountDataImpl(DateTime dtValuationDate);
        
        private void OnValuationDateChanged(object sender, EventArgs e)
        {
            if (_bInitialised)
            {
                _GetCashAccountData();
            }
        }

        private void btnAddTransaction_Click(object sender, EventArgs e)
        {
            var view = new AddTransactionView(_dataModel, TransactionMnenomic);
            if(view.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var total = AddTransactionImpl(view.GetTransactionDate(), view.GetTransactionType(),
                               view.GetParameter(), view.GetAmount());
                txtTotal.Text = total.ToString();
                AddGridStyling();
            }
        }

        protected abstract double AddTransactionImpl(DateTime dtTransactionDate, string type, string parameter, double dAmount);

        private void btnDeleteTransaction_Click(object sender, EventArgs e)
        {
            foreach(DataGridViewRow row in cashAccountGrid.SelectedRows)
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
            btnDeleteTransaction.Enabled = true;
        }
    }
}
