using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderClient.DataModel;
using InvestmentBuilderClient.ViewModel;

namespace InvestmentBuilderClient.View
{
    internal class ReceiptDataView: CashAccountView
    {
        public ReceiptDataView(InvestmentDataModel dataModel) :
            base(dataModel)
        {
            this.Text = "Receipt Cash Account";
        }

        protected override string TransactionMnenomic { get { return "R"; } }

        protected override CashAccountViewModel SetupDataSource(InvestmentDataModel dataModel)
        {
            var vm = new ReceiptDataViewModel(dataModel);
            cashAccountBindingSource.DataSource = vm.Receipts;
            return vm;
        }

        protected override void SetupGrid()
        {
            cashAccountGrid.AutoGenerateColumns = false;
            AddColumn(cashAccountGrid, "Transaction Date", "TransactionDate");
            AddColumn(cashAccountGrid, "Parameter", "Parameter");
            AddColumn(cashAccountGrid, "Subscription", "Subscription");
            AddColumn(cashAccountGrid, "Sale", "Sale");
            AddColumn(cashAccountGrid, "Dividend", "Dividend");
            AddColumn(cashAccountGrid, "Other Receipts", "Other");
        }
    }
}
