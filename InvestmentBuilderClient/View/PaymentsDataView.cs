using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderClient.DataModel;
using InvestmentBuilderClient.ViewModel;

namespace InvestmentBuilderClient.View
{
    internal class PaymentsDataView : CashAccountView
    {
        public PaymentsDataView(InvestmentDataModel dataModel) :
            base(dataModel)
        {
            this.Text = "Payments Cash Account";
        }

        protected override string TransactionMnenomic { get { return "P"; } }

        protected override CashAccountViewModel SetupDataSource(InvestmentDataModel dataModel)
        {
            var vm = new PaymentsDataViewModel(dataModel);
            cashAccountBindingSource.DataSource = vm.Payments;
            return vm;
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
