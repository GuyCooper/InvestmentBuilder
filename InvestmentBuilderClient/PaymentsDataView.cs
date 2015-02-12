using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestmentBuilderClient
{
    class PaymentsDataView : CashAccountView
    {
        public PaymentsDataView(InvestmentDataModel dataModel) :
            base(dataModel)
        {
            this.Text = "Payments Cash Account";
        }

        protected override string TransactionMnenomic { get { return "P"; } }

        protected override void SetupDataSource(InvestmentDataModel dataModel)
        {
            var vm = new PaymentsDataViewModel(dataModel);
            _vm = vm;
            cashAccountBindingSource.DataSource = vm.Payments;
        }

        protected override void SetupGrid()
        {
            cashAccountGrid.AutoGenerateColumns = false;
            AddColumn(cashAccountGrid, "Transaction Date", "TransactionDate");
            AddColumn(cashAccountGrid, "Parameter", "Parameter");
            AddColumn(cashAccountGrid, "Withdrawls", "Withdrawls");
            AddColumn(cashAccountGrid, "Purchases", "Purchases");
            AddColumn(cashAccountGrid, "Other Payments", "Other");
        }
    }
}
