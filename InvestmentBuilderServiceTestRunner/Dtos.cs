using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestmentBuilderServiceTestRunner
{
    /// <summary>
    /// Contains Dto classes
    /// </summary>
    internal class Dtos
    {
        //transaction class.used for binding the cash account transactions to a displayable
        //view.
        public abstract class CashTransaction
        {
            public int TransactionID { get; set; }
            public DateTime ValuationDate { get; set; }
            //public DateTime TransactionDate { get; set; }
            public string TransactionDate { get; set; }
            public string TransactionType { get; set; }
            public string Parameter { get; set; }
            public double Amount { get; set; }
            public bool Added { get; set; }
            public bool IsTotal { get; set; }
        }

        /// <summary>
        /// Receipt transaction class.
        /// </summary>
        public class ReceiptTransaction : CashTransaction
        {
            public double Subscription { get; set; }
            public double Sale { get; set; }
            public double Dividend { get; set; }
            public double Other { get; set; }
        }

        /// <summary>
        /// Payment transaction class.
        /// </summary>
        public class PaymentTransaction : CashTransaction
        {
            public double Withdrawls { get; set; }
            public double Purchases { get; set; }
            public double Other { get; set; }
        }

        /// <summary>
        /// CashFlow model
        /// </summary>
        public class CashFlowModel
        {
            public IEnumerable<ReceiptTransaction> Receipts { get; set; }
            public IEnumerable<PaymentTransaction> Payments { get; set; }
            public string ReceiptsTotal { get; set; }
            public string PaymentsTotal { get; set; }
            public string ValuationDate { get; set; }
            public bool CanEdit { get; set; }
            public bool CanBuild { get; set; }
        }

        public class AccountIdentifier
        {
            public string Name { get; set; }
            public int AccountId { get; set; }
        }

        /// <summary>
        /// AccountMember dto.
        /// </summary>
        public class AccountMemberDto
        {
            public string Name { get; set; }
            public string Permission { get; set; }
        }

        public class CompanyData
        {
            public string Name { get; set; }
            public DateTime ValuationDate { get; set; }
            public DateTime LastBrought { get; set; }
            public int Quantity { get; set; }
            public double AveragePricePaid { get; set; }
            public double TotalCost { get; set; }
            public double SharePrice { get; set; }
            public double NetSellingValue { get; set; }
            public double ProfitLoss { get; set; }
            public double MonthChange { get; set; }
            public double MonthChangeRatio { get; set; }
            public double Dividend { get; set; }
            public string ManualPrice { get; set; }
            public double TotalReturn { get; set; }
        }
    }
}
