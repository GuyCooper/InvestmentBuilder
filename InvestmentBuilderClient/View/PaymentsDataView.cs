using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestmentBuilderClient.DataModel;
using InvestmentBuilderClient.ViewModel;
using InvestmentBuilder;
using System.ComponentModel;
using System.Windows.Forms;
using NLog;

namespace InvestmentBuilderClient.View
{
    internal class PaymentsDataView : CashAccountView
    {
        private BindingList<PaymentTransaction> _payments;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public PaymentsDataView(InvestmentDataModel dataModel) :
            base(dataModel)
        {
            logger.Info("creating payments data view");
            this.Text = "Payments Cash Account";
        }

        protected override void SetupDatasource(BindingSource source)
        {
            _payments = new BindingList<PaymentTransaction>();
            source.DataSource = _payments;
        }

        protected override double GetCashAccountDataImpl(InvestmentDataModel dataModel, DateTime dtValuationDate)
        {
            _payments.Clear();
            double dTotal;
            var transactions = dataModel.GetPaymentTransactions(dtValuationDate, out dTotal);
            foreach (var payment in transactions)
            {
                _payments.Add(payment);
            }
            return dTotal;
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

        protected override string GetMnenomic(InvestmentDataModel dataModel)
        {
            return dataModel.GetPaymentMnenomic();
        }
    }
}
