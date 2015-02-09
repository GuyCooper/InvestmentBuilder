using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestmentBuilderClient
{
    class PaymentsDataViewModel : CashAccountViewModel
    {
        public PaymentsDataViewModel(InvestmentDataModel dataModel) :
            base(dataModel)
        {
        }

        public override double GetTransactionData(DateTime dtValuationDate)
        {
            throw new NotImplementedException();
        }

        public override double AddTransaction(DateTime dtTransactionDate, string type, string parameter, double dAmount)
        {
            throw new NotImplementedException();
        }

        public override double DeleteTransaction(Transaction transaction)
        {
            throw new NotImplementedException();
        }
    }
}
