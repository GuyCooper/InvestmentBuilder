using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using InvestmentBuilderWeb.Models;
using InvestmentBuilder;
using InvestmentBuilderCore;
using InvestmentBuilderWeb.Translators;
using InvestmentBuilderWeb.Interfaces;

namespace InvestmentBuilderWeb.Controllers
{
    //[Authorize]
    [RoutePrefix("InvestmentRecord")]
    [Route("{action}")]
    public sealed class InvestmentRecordController : InvestmentBaseController
    {
        private IInvestmentRecordInterface _recordData;
        private CashAccountTransactionManager _cashTransactionManager;

        private const string ALL = "All";

        public InvestmentRecordController(InvestmentBuilder.InvestmentBuilder investmentBuilder
                                        , IDataLayer dataLayer
                                        , IAuthorizationManager authorizationManager
                                        , IApplicationSessionService sessionService
                                        , CashAccountTransactionManager cashTransactionManager
                                        , AccountManager accountManager
                                        , BrokerManager brokerManager
                                        )  :
            base(authorizationManager, accountManager, investmentBuilder, brokerManager, sessionService, dataLayer.ClientData)
        {
            _recordData = dataLayer.InvestmentRecordData;
            _cashTransactionManager = cashTransactionManager;
        }

        // GET: InvestmentRecord
        [Route("")]
        [Route("index")]
        public ActionResult Index()
        {
            return _CreateMainView("Index", _GetCurrentInvestments(_SetupAccounts(null)));
        }

        [HttpGet]
        public ActionResult CreateTrade()
        {
            _SetupAccounts(null);
            return View("CreateTrade");
        }

        [HttpPost]
        public ActionResult CreateTrade(TradeItemModel tradeItem)
        {
            var token = _SetupAccounts(null);
            if (this.ModelState.IsValid)
            {
                _investmentBuilder.UpdateTrades(token, tradeItem.ToTrades(TransactionType.BUY), null);   
            }
            else
            {
                this.ModelState.AddModelError("", "Invalid data enterted");
                return View("CreateTrade");
            }

            return _CreateMainView("Index", _GetCurrentInvestments(token));
        }

        private CashFlowModel _GetCashFlowModel()
        {
            var token = _SetupAccounts(null);
            var dtValuation = _sessionService.GetValuationDate(SessionId);
            var dtPrevious = _clientData.GetPreviousAccountValuationDate(token, dtValuation);
            double dReceiptTotal, dPaymentTotal;
            var cashFlowModel = new CashFlowModel();
            cashFlowModel.Receipts = _cashTransactionManager.GetReceiptTransactions(token, dtValuation, dtPrevious, out dReceiptTotal).Select(x => x.ToReceiptCashFlowModel()).ToList();
            cashFlowModel.Payments = _cashTransactionManager.GetPaymentTransactions(token, dtValuation, out dPaymentTotal).Select(x => x.ToPaymentCashFlowModel()).ToList();
            cashFlowModel.ReceiptsTotal = dReceiptTotal;
            cashFlowModel.PaymentsTotal = dPaymentTotal;
            cashFlowModel.ValuationDate = dtValuation.ToShortDateString();

            if(cashFlowModel.ReceiptsTotal > 0 && cashFlowModel.ReceiptsTotal == cashFlowModel.PaymentsTotal)
            {
                //receipts and payments match, allow build report
                _sessionService.SetEnableBuildReport(SessionId, true);

            }
            return cashFlowModel;
        }

        [HttpGet]
        public ActionResult CashFlow()
        {
            var model = _GetCashFlowModel();
            return _CreateMainView("CashFlow", model);
        }

        [HttpGet]
        public ActionResult Reports()
        {
            var userToken = _SetupAccounts(null);
            var vm = new ReportsModel
            {
                RecentReports = _clientData.GetRecentValuationDates(userToken, DateTime.Now).Select(x =>
                                            x.ToShortDateString()).ToList()
            };

            return _CreateMainView("Reports", vm);
        }

        [HttpGet]
        public ActionResult LoadReport(string ValuationDate)
        {
            var token = _SetupAccounts(null);
            var dtValuation = DateTime.Parse(ValuationDate);
            var reportFile =_investmentBuilder.GetInvestmentReport(token, dtValuation);
            
            return File(reportFile, "application/pdf");
        }

        [HttpGet]
        public ActionResult EditTrade(string name)
        {
            var token =_SetupAccounts(null);
            var tradeItem = _clientData.GetTradeItem(token, name);
            if(tradeItem != null)
            {
                var model = tradeItem.ToTradeItemModel();

                ViewBag.Actions = Enum.GetNames(typeof(TransactionType)).Select(x =>
                   new SelectListItem
                   {
                       Text = x,
                       Value = x,
                       Selected = x == model.Action.ToString()
                   });

                return PartialView(model);
            }
            return null;
        }

        [HttpPost]
        public ActionResult EditTrade(TradeItemModel tradeItem)
        {
            var token = _SetupAccounts(null);
            if (this.ModelState.IsValid && tradeItem != null && tradeItem.Action != TransactionType.NONE)
            {
                _investmentBuilder.UpdateTrades(token, tradeItem.ToTrades(tradeItem.Action), tradeItem.GetManualPrices());
            }
            else
            {
                this.ModelState.AddModelError("", "Invalid data enterted");
                return View("Edit");
            }
            return _CreateMainView("Index", _GetCurrentInvestments(token));
        }

        [HttpGet]
        [Route("Delete")]
        public ActionResult Delete(string name)
        {
            //_service.DeleteRecord(data);
            var token = _SetupAccounts(null);
            var tradeItem = _clientData.GetTradeItem(token, name);
            if (tradeItem != null)
            {
                _investmentBuilder.UpdateTrades(token, tradeItem.ToTrades(TransactionType.SELL), null);
                return _CreateMainView("Index", _GetCurrentInvestments(token));
            }
            return null;
        }

        [HttpGet]
        public ActionResult UpdateAccount(string accountId)
        {
            var token = _SetupAccounts(accountId);
            _sessionService.ResetValuationDate(SessionId);
            return _CreateMainView("Index", _GetCurrentInvestments(token));
        }

        [HttpGet]
        public ActionResult UpdatePrice(string investment, double share_price)
        {
            var token = _SetupAccounts(null);

            _sessionService.AddManualPrice(SessionId, investment, share_price);
            return _CreateMainView("Index", _GetCurrentInvestments(token));
        }

        [HttpGet]
        public ActionResult UpdateValuationDate(DateTime dtValaution)
        {
            _sessionService.SetValuationDate(SessionId, dtValaution);
            return _CreateMainView("CashFlow", _GetCashFlowModel());
        }

        [HttpGet]
        public ActionResult BuildReport()
        {
            var token = _SetupAccounts(null);
            var report = _investmentBuilder.BuildAssetReport(token
                                                            , _sessionService.GetValuationDate(SessionId)
                                                            , true
                                                            , _sessionService.GetManualPrices(SessionId));
            if(report == null)
            {
                setupErrors(token.User);
                return CashFlow();
            }

            var reportFile = _investmentBuilder.GetInvestmentReport(token, _sessionService.GetValuationDate(SessionId));
            return File(reportFile, "application/pdf");
        }

        private ActionResult _AddTransactionView(string title, string transactionType)
        {
           _SetupAccounts(null);
            ViewBag.Title = title;

            ViewBag.ParameterType = _cashTransactionManager.GetTransactionTypes(transactionType)
                .Select(x =>
            new SelectListItem
            {
                Text = x,
                Value = x,
                Selected = false
            }).ToList();

            //insert an empty row at the beginning to be the default
            ViewBag.ParameterType.Insert(0, new SelectListItem { Text = "", Value = "" });

            ViewBag.DummyParameters = new List<SelectListItem> {
                new SelectListItem
                {
                     Text = "Please select a parameter type"
                }
            };
 
            return PartialView("AddTransaction");
        }

        private void _ProcessCashTransaction(UserAccountToken token, DateTime transactionDate, string transactionType, string parameter, double amount)
        {
            _cashTransactionManager.AddTransaction(token, _sessionService.GetValuationDate(SessionId),
                                    transactionDate,
                                    transactionType, 
                                    parameter,
                                    amount);
        }

        private ActionResult _ProcessTransaction(TransactionModel transaction, string transactionType)
        {
            var token = _SetupAccounts(null);
            if (this.ModelState.IsValid && transaction.TransactionDate != null)
            {
                if (transaction.Parameter == ALL)
                {
                    var latestRecordDate = _recordData.GetLatestRecordInvestmentValuationDate(token) ?? DateTime.Today;
                    var parameters = _investmentBuilder.GetParametersForTransactionType(token, latestRecordDate, transaction.ParameterType).ToList();
                    foreach (var parameter in parameters)
                    {
                        _ProcessCashTransaction(token, transaction.TransactionDate.Value, transaction.ParameterType,
                            parameter, transaction.Amount);   
                    }
                }
                else
                {
                    _ProcessCashTransaction(token, transaction.TransactionDate.Value, transaction.ParameterType,
                                        transaction.Parameter, transaction.Amount);   
                }
            }
            else
            {
                this.ModelState.AddModelError("", "Invalid data enterted");
            }

            return _CreateMainView("CashFlow", _GetCashFlowModel());
        }

        [HttpGet]
        public ActionResult AddReceiptTransaction()
        {
            return _AddTransactionView("Add Receipt", _cashTransactionManager.ReceiptMnemomic);
        }

        [HttpGet]
        public ActionResult AddPaymentTransaction()
        {
            return _AddTransactionView("Add Payment", _cashTransactionManager.PaymentMnemomic);
        }

        [HttpPost]
        public ActionResult AddReceiptTransaction(TransactionModel transaction)
        {
            return _ProcessTransaction(transaction, _cashTransactionManager.ReceiptMnemomic);
        }

        [HttpPost]
        public ActionResult AddPaymentTransaction(TransactionModel transaction)
        {
            return _ProcessTransaction(transaction, _cashTransactionManager.PaymentMnemomic);
        }

        [HttpGet]
        public string GetParametersForTransaction(string ParameterType)
        {
            var token = _SetupAccounts(null);
            var latestRecordDate = _recordData.GetLatestRecordInvestmentValuationDate(token) ?? DateTime.Today;
            var parameters = _investmentBuilder.
                GetParametersForTransactionType(token,
                                                latestRecordDate,
                                                ParameterType).Select(x => string.Format("\"{0}\"", x)).ToList();

            if (parameters.Count > 0)
            {
                parameters.Add(string.Format("\"{0}\"", ALL));
                return string.Format("[{0}]", string.Join(",", parameters));
            }

            return string.Format("[\"{0}\"]", ParameterType);
        }

        [HttpPost]
        public JsonResult GetTestReceipt()
        {
            return Json("<p>This is the help page</p>");
        }

        
        private ActionResult _RemoveCashTransaction(Transaction transaction)
        {
            var token = _SetupAccounts(null);
            _cashTransactionManager.RemoveTransaction(token, transaction.ValuationDate, transaction.TransactionDate, transaction.TransactionType, transaction.Parameter);
            return _CreateMainView("CashFlow", _GetCashFlowModel());
        }

        [HttpGet]
        public ActionResult RemoveReceiptTransaction(ReceiptCashFlowModel item)
        {
            return _RemoveCashTransaction(item);
        }

        [HttpGet]
        public ActionResult RemovePaymentTransaction(PaymentCashFlowModel item)
        {
            return _RemoveCashTransaction(item);
        }
    }
}