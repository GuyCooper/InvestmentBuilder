using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InvestmentBuilderClient.ViewModel;
using InvestmentBuilderClient.DataModel;

namespace InvestmentBuilderClient.View
{
    internal abstract partial class CashAccountView : Form, IInvestmentBuilderView
    {
        private InvestmentDataModel _dataModel;
        private CashAccountViewModel _vm;

        bool _bInitialised = false;

        public CashAccountView(InvestmentDataModel dataModel)
        {
            InitializeComponent();
            SetupGrid();
            _vm = SetupDataSource(dataModel);
            cashAccountGrid.DataSource = cashAccountBindingSource;
            _dataModel = dataModel;
            //_GetCashAccountData();            
            _bInitialised = true;
        }

        protected abstract CashAccountViewModel SetupDataSource(InvestmentDataModel dataModel);

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

        private void _GetCashAccountData(DateTime dtValuation)
        {
            var total = GetCashAccountDataImpl(dtValuation);
            txtTotal.Text = total.ToString();
            AddGridStyling();
        }

        private double GetCashAccountDataImpl(DateTime dtValuationDate)
        {
            return _vm.GetTransactionData(dtValuationDate, TransactionMnenomic);
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

        private double AddTransactionImpl(DateTime dtTransactionDate, string type, string parameter, double dAmount)
        {
            return _vm.AddTransaction(dtTransactionDate, type, parameter, dAmount);
        }

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

        public void UpdateValuationDate(DateTime dtValuation)
        {
            if (_bInitialised)
            {
                _GetCashAccountData(dtValuation);
            }
        }

        public void CommitData(DateTime dtValuation)
        {
            _vm.CommitData(dtValuation);
        }

        public double GetTotal()
        {
            return double.Parse(txtTotal.Text);
        }
    }
}
