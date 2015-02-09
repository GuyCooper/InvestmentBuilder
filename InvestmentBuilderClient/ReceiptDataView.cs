using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestmentBuilderClient
{
    class ReceiptDataView: CashAccountView
    {
        public ReceiptDataView(InvestmentDataModel dataModel) :
            base(dataModel)
        {
            this.Text = "Receipt Cash Account";
        }

        protected override string TransactionMnenomic { get { return "R"; } }

        protected override void SetupDataSource(InvestmentDataModel dataModel)
        {
            var vm = new ReceiptDataViewModel(dataModel);
            _vm = vm;
            cashAccountBindingSource.DataSource = vm.Receipts;
        }

        protected override void SetupGrid()
        {
            cashAccountGrid.AutoGenerateColumns = false;
            AddColumn(cashAccountGrid, "Transaction Date", "TransactionDate");
            AddColumn(cashAccountGrid, "Parameter", "Parameter");
            AddColumn(cashAccountGrid, "Subscription", "Subscription");
            AddColumn(cashAccountGrid, "Sale", "Sale");
            AddColumn(cashAccountGrid, "Dividend", "Dividend");
            AddColumn(cashAccountGrid, "Other", "Other");
        }

        protected override double GetCashAccountDataImpl(DateTime dtValuationDate)
        {
            return _vm.GetTransactionData(dtValuationDate);
        }

        protected override double AddTransactionImpl(DateTime dtTransactionDate, string type, string parameter, double dAmount)
        {
            return _vm.AddTransaction(dtTransactionDate, type, parameter, dAmount);
        }
    }
}
