using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestmentBuilderServiceTestRunner.Requests
{
    /// <summary>
    /// AddTransaction request tests.
    /// </summary>
    abstract class AddTransactionRequest : RequestEndpoint<AddTransactionRequest.Request, AddTransactionRequest.Result>
    {
        #region Internal classes

        /// <summary>
        /// Request Dto
        /// </summary>
        public class Request : Dto
        {
            public string TransactionDate { get; set; }
            public string ParamType { get; set; }
            public string[] Parameter { get; set; }
            public double Amount { get; set; }
            public string DateRequestedFrom { get; set; }
        }

        /// <summary>
        /// Result Dto.
        /// </summary>
        public class Result : Dto
        {
            public IEnumerable<Dtos.CashFlowModel> CashFlows { get; set; }
            public IEnumerable<string> ReceiptParamTypes { get; set; }
            public IEnumerable<string> PaymentParamTypes { get; set; }
        }

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public AddTransactionRequest(string requestChannel, string responseChannel)
            : base(requestChannel, responseChannel)
        {
        }
    }

    /// <summary>
    /// RecieptTransaction Request tests.
    /// </summary>
    class RecieptTransactionRequest : AddTransactionRequest
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public RecieptTransactionRequest()
            : base("ADD_RECEIPT_TRANSACTION_REQUEST", "ADD_RECEIPT_TRANSACTION_RESPONSE")
        {
        }

        #region Test Methods

        /// <summary>
        /// Add a subscription transaction
        /// </summary>
        void AddSubscriptionTest(ConnectionService connectionService)
        {
            //var request = new AddTransactionRequest.Request
            //{
            //    public string TransactionDate { get; set; }
            //    public string ParamType { get; set; }
            //    public string[] Parameter { get; set; }
            //    public double Amount { get; set; }
            //    public string DateRequestedFrom { get; set; }
            //};
        }

        #endregion
    }

    /// <summary>
    /// PaymentTransaction Request tests.
    /// </summary>
    class PaymentTransactionRequest : AddTransactionRequest
{
        /// <summary>
        /// Constructor.
        /// </summary>
        public PaymentTransactionRequest()
            : base("ADD_PAYMENT_TRANSACTION_REQUEST", "ADD_PAYMENT_TRANSACTION_RESPONSE")
        {
        }

        #region Test Methods

        #endregion
    }
}

