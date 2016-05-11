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
using InvestmentBuilder;
using NLog;

namespace InvestmentBuilderClient.View
{
    internal abstract partial class CashAccountView : Form, IInvestmentBuilderView
    {
        private InvestmentDataModel _dataModel;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        private bool _bInitialised = false;
        private DateTime? _valuationDate;

        public CashAccountView(InvestmentDataModel dataModel)
        {
            InitializeComponent();
            SetupGrid();
            _dataModel = dataModel;
            SetupDatasource(cashAccountBindingSource);
            cashAccountGrid.DataSource = cashAccountBindingSource;
            _bInitialised = true;
        }

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
            var total = GetCashAccountDataImpl(_dataModel, dtValuation);
            txtTotal.Text = total.ToString();
            AddGridStyling();
        }

        protected abstract void SetupDatasource(BindingSource source);

        protected abstract double GetCashAccountDataImpl(InvestmentDataModel dataModel, DateTime dtValuationDate);

        protected abstract string GetMnenomic(InvestmentDataModel dataModel);

        private void btnAddTransaction_Click(object sender, EventArgs e)
        {
            var view = new AddTransactionView(_dataModel, GetMnenomic(_dataModel));
            if(view.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (view.ValidateTransaction())
                {
                    var valuationDate = _valuationDate.HasValue ? _valuationDate.Value : DateTime.Today;
                    var transactionParams = view.GetParameter().ToList();
                    foreach (var param in transactionParams)
                    {
                        _dataModel.AddCashTransaction(valuationDate, view.GetTransactionDate(), view.GetTransactionType(),
                                  param, view.GetAmount());
                    }

                    _GetCashAccountData(valuationDate);
                }
                else
                {
                    logger.Log(LogLevel.Error, "transaction validation failed!");
                }
            }
        }

        private void btnDeleteTransaction_Click(object sender, EventArgs e)
        {
            foreach(DataGridViewRow row in cashAccountGrid.SelectedRows)
            {
                var transaction = row.DataBoundItem as Transaction;
                //if(!transaction.Added)
                //{
                //    MessageBox.Show("Can only delete transactions that have not been comitted");
                //}
                //else
                //{
                    _dataModel.RemoveCashTransaction(transaction.ValuationDate,
                                                     transaction.TransactionDate,
                                                     transaction.TransactionType,
                                                     transaction.Parameter);
                    _GetCashAccountData(transaction.ValuationDate);
                //}
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
                _valuationDate = dtValuation;
                _GetCashAccountData(dtValuation);
            }
        }

        public double GetTotal()
        {
            return double.Parse(txtTotal.Text);
        }

        private void cashAccountGrid_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            var transaction = cashAccountGrid.Rows[e.RowIndex].DataBoundItem as Transaction;
            if (transaction != null)
            {
                cashAccountGrid.Rows[e.RowIndex].ReadOnly = transaction.Added == false;
            }
        }

        public void UpdateAccountName(string account)
        {
        }
    }
}
