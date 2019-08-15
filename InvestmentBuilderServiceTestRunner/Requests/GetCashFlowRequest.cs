using System.Collections.Generic;
using System.Linq;

namespace InvestmentBuilderServiceTestRunner.Requests
{
    /// <summary>
    /// GetCashFlow request tests.
    /// </summary>
    class GetCashFlowRequest : RequestEndpoint<GetCashFlowRequest.Request, GetCashFlowRequest.Result>
    {
        #region Internal classes

        /// <summary>
        /// Request Dto
        /// </summary>
        public class Request : Dto
        {
            public string DateFrom { get; set; }
        }

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
        public GetCashFlowRequest() : base("GET_CASH_FLOW_REQUEST", "GET_CASH_FLOW_RESPONSE")
        {
        }

        #region Test Methods

        /// <summary>
        /// Get cash flow for empty user
        /// </summary>
        public void GetCashFlowEmptyUser(ConnectionService connectionService)
        {
            // Get account names..
            LogMessage("GetCashFlow");
            var result = SendRequest(new GetCashFlowRequest.Request(), connectionService);
            Assert.IsTrue(result.Success, "GetCashFlow", result.Error);
            Assert.AreEqual(1, result.Result.CashFlows.Count(), "GetCashFlow Number of cash flows");
            Assert.AreEqual(1, result.Result.CashFlows.First().Payments.Count(), "GetCashFlow Number of payments");
            Assert.AreEqual(1, result.Result.CashFlows.First().Receipts.Count(), "GetCashFlow Number of receipts");
            Assert.AreEqual(4, result.Result.PaymentParamTypes.Count(), "GetCashFlow Number of payment types");
            Assert.AreEqual(5, result.Result.ReceiptParamTypes.Count(), "GetCashFlow Number of receipt types");
        }

        #endregion
    }
}
