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
    public partial class CashAccountView : Form
    {
        InvestmentDataModel _paymentsModel;
        bool _bInitialised = false;

        public CashAccountView()
        {
            InitializeComponent();
            SetupGrid();
            _paymentsModel = new InvestmentDataModel();
            receiptsBindingSource.DataSource = _paymentsModel.Receipts;
            receiptsGrid.DataSource = receiptsBindingSource;
            cmboDate.Items.AddRange(_paymentsModel.GetValuationDates().Cast<object>().ToArray());
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

        protected override void OnClosed(EventArgs e)
        {
            _paymentsModel.Dispose();
            base.OnClosed(e);
        }

        private void OnValuationDateChanged(object sender, EventArgs e)
        {
            if (_bInitialised)
            {
                DateTime dtValuationDate = (DateTime)cmboDate.SelectedItem;
                _paymentsModel.GetReceipts(dtValuationDate);
                DataGridViewCellStyle style = new DataGridViewCellStyle();
                style.Font = new Font(receiptsGrid.Font, FontStyle.Bold);
                receiptsGrid.Rows[receiptsGrid.Rows.Count - 1].DefaultCellStyle = style;
                receiptsGrid.AutoResizeColumns();
            }
        }
    }
}
