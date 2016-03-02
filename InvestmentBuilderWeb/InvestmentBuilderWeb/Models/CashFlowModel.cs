using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InvestmentBuilder;

namespace InvestmentBuilderWeb.Models
{
    public class CashFlowModel
    {
        public IEnumerable<ReceiptCashFlowModel> Receipts { get; set; }
        public IEnumerable<PaymentCashFlowModel> Payments { get; set; }
        public double ReceiptsTotal { get; set; }
        public double PaymentsTotal { get; set; }
    }

    //todo add validation
    public class ReceiptCashFlowModel : ReceiptTransaction
    {
    }

    //todo add validation
    public class PaymentCashFlowModel : PaymentTransaction
    {
    }

}