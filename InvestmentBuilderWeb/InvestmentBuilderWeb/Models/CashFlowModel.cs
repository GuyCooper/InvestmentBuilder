using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InvestmentBuilder;
using System.ComponentModel.DataAnnotations;

namespace InvestmentBuilderWeb.Models
{
    public class CashFlowModel
    {
        public IEnumerable<ReceiptCashFlowModel> Receipts { get; set; }
        public IEnumerable<PaymentCashFlowModel> Payments { get; set; }
        public double ReceiptsTotal { get; set; }
        public double PaymentsTotal { get; set; }
        public string ValuationDate { get; set; }
    }

    public class CashFlowModelAndParams
    {
        public IEnumerable<CashFlowModel> CashFlows { get; set; }
        public IEnumerable<string> ReceiptParamTypes { get; set; }
        public IEnumerable<string> PaymentParamTypes { get; set; }
    }

    //todo add validation
    [MetadataType(typeof(ReceiptTransactionAttributes))]
    public class ReceiptCashFlowModel : ReceiptTransaction
    {
    }

    [MetadataType(typeof(PaymentTransactionAttributes))]
    public class PaymentCashFlowModel : PaymentTransaction
    {
    }

    public class TransactionAttributes
    {
        [DisplayFormat(DataFormatString="{0:d}")]
        public object ValuationDate { get; set; }
        [DisplayFormat(DataFormatString="{0:d}")]
        public object TransactionDate { get; set; }
        [MaxLength(30)]
        public object Parameter { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public object Other { get; set; }
  
    }

    public class PaymentTransactionAttributes : TransactionAttributes
    {
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public double Withdrawls { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public double Purchases { get; set; }
    }

    public class ReceiptTransactionAttributes : TransactionAttributes
    {
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public object Subscription { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public object Sale { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public object Dividend { get; set; }
    }

}