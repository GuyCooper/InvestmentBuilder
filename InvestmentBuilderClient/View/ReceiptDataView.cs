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
    internal class ReceiptDataView: CashAccountView
    {
        private BindingList<ReceiptTransaction> _receipts;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public ReceiptDataView(InvestmentDataModel dataModel) :
            base(dataModel)
        {
            logger.Info("creating receipt data view");
            this.Text = "Receipt Cash Account";
        }

        protected override void SetupDatasource(BindingSource source)
        {
            _receipts = new BindingList<ReceiptTransaction>();
            source.DataSource = _receipts;
        }

        protected override double GetCashAccountDataImpl(InvestmentDataModel dataModel, DateTime dtValuationDate)
        {
            _receipts.Clear();
            double dTotal;
            var transactions = dataModel.GetReceiptTransactions(dtValuationDate, out dTotal);
            foreach (var transaction in transactions)
            {
                _receipts.Add(transaction);
            }
            return dTotal;
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

        protected override string GetMnenomic(InvestmentDataModel dataModel)
        {
            return dataModel.GetReceiptMnenomic();
        }
    }
}
