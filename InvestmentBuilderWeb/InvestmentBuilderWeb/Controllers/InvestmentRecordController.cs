using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InvestmentBuilderWeb.Models;
using InvestmentBuilderWeb.Services;
using Microsoft.AspNet.Identity;
using InvestmentBuilder;
using InvestmentBuilderCore;
using InvestmentBuilderWeb.Translators;
using InvestmentBuilderWeb.Interfaces;

namespace InvestmentBuilderWeb.Controllers
{
    [Authorize]
    [RoutePrefix("InvestmentRecord")]
    [Route("{action}")]
    public class InvestmentRecordController : Controller
    {
        private InvestmentBuilder.InvestmentBuilder _investmentBuilder;
        private IClientDataInterface _clientData;
        private IInvestmentRecordInterface _recordData;
        private IAuthorizationManager _authorizationManager;
        private InvestmentRecordSessionService _sessionService;
        private CashAccountTransactionManager _cashTransactionManager;
        private string _sessionId;

        private const string ALL = "All";

        public InvestmentRecordController(InvestmentBuilder.InvestmentBuilder investmentBuilder
                                        , IDataLayer dataLayer
                                        , IAuthorizationManager authorizationManager
                                        , IApplicationSessionService sessionService
                                        , CashAccountTransactionManager cashTransactionManager)  
        {
            _investmentBuilder = investmentBuilder;
            _clientData = dataLayer.ClientData;
            _recordData = dataLayer.InvestmentRecordData;
            _authorizationManager = authorizationManager;
            _sessionService = sessionService as InvestmentRecordSessionService;
            _sessionId = System.Web.HttpContext.Current.Session.SessionID;
            _cashTransactionManager = cashTransactionManager;
        }

        //setup the user account accounttoken and populate accounts list for user. if selectedAccount is null then just uses
        //first account for user if no usertoken setup for user otherwise just uses existing token 
        private UserAccountToken _SetupAccounts(string selectedAccount)
        {
            UserAccountToken token = null;
            if (User != null && User.Identity != null)
            {
                var username = User.Identity.GetUserName();
                var accounts = _clientData.GetAccountNames(username).ToList();
                if (_authorizationManager.GetCurrentTokenForUser(username) == null)
                {
                    _authorizationManager.SetUserAccountToken(username, accounts.FirstOrDefault());
                }
                
                if(string.IsNullOrEmpty(selectedAccount) == false)
                {
                    _authorizationManager.SetUserAccountToken(username, selectedAccount);
                }

                token = _authorizationManager.GetCurrentTokenForUser(username);
                ViewBag.accountId = accounts.Select(x =>
                   new SelectListItem
                   {
                       Text = x,
                       Value = x,
                       Selected = token.Account == x
                   });
            }
            return token;
        }

        private IEnumerable<CompanyDataModel> _GetCurrentInvestments(UserAccountToken token)
        {
            return _investmentBuilder.GetCurrentInvestments(token, _sessionService.GetManualPrices(_sessionId))
                .OrderBy(x => x.Name)
                .Select(x => x.ToCompanyDataModel());
        }

        // GET: InvestmentRecord
        [Route("")]
        [Route("index")]
        public ActionResult Index()
        {
            return View(_GetCurrentInvestments(_SetupAccounts(null)));
        }

        [HttpGet]
        public ActionResult Create()
        {
            _SetupAccounts(null);
            ViewBag.Title = "Add New Trade";
            return View("Edit");
        }

        [HttpPost]
        public ActionResult Create(TradeItemModel tradeItem)
        {
            var token = _SetupAccounts(null);
            if (this.ModelState.IsValid)
            {
                _investmentBuilder.UpdateTrades(token, tradeItem.ToTrades(TransactionType.BUY), null);   
            }
            else
            {
                this.ModelState.AddModelError("", "Invalid data enterted");
                return View("Edit");
            }

            return View("Index", _GetCurrentInvestments(token));
        }
         
        private CashFlowModel _GetCashFlowModel()
        {
            var token = _SetupAccounts(null);
            var dtValuation = _sessionService.GetValuationDate(_sessionId);
            var dtPrevious = _clientData.GetPreviousAccountValuationDate(token, dtValuation);
            double dReceiptTotal, dPaymentTotal;
            var cashFlowModel = new CashFlowModel();
            cashFlowModel.Receipts = _cashTransactionManager.GetReceiptTransactions(token, dtValuation, dtPrevious, out dReceiptTotal).Select(x => x.ToReceiptCashFlowModel()).ToList();
            cashFlowModel.Payments = _cashTransactionManager.GetPaymentTransactions(token, dtValuation, out dPaymentTotal).Select(x => x.ToPaymentCashFlowModel()).ToList();
            cashFlowModel.ReceiptsTotal = dReceiptTotal;
            cashFlowModel.PaymentsTotal = dPaymentTotal;
            cashFlowModel.ValuationDate = dtValuation.ToShortDateString();
            return cashFlowModel;
        }

        [HttpGet]
        public ActionResult CashFlow()
        {
            return View("CashFlow", _GetCashFlowModel());
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
            return View("Reports", vm);
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
        public ActionResult Edit(string name)
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

                ViewBag.Title = "Edit Trade";
                return View(model);
            }
            return null;
        }

        [HttpPost]
        public ActionResult Edit(TradeItemModel tradeItem)
        {
            var token = _SetupAccounts(null);
            if (this.ModelState.IsValid)
            {
                _investmentBuilder.UpdateTrades(token, tradeItem.ToTrades(tradeItem.Action), tradeItem.GetManualPrices());
            }
            else
            {
                this.ModelState.AddModelError("", "Invalid data enterted");
                return View("Edit");
            }
            return View("Index", _GetCurrentInvestments(token));
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
                return View("Index", _GetCurrentInvestments(token));
            }
            return null;
        }

        [HttpGet]
        public ActionResult UpdateAccount(string accountId)
        {
            var token = _SetupAccounts(accountId);
            _sessionService.ResetValuationDate(_sessionId);
            return View("Index", _GetCurrentInvestments(token));
        }

        [HttpGet]
        public ActionResult UpdatePrice(string investment, double share_price)
        {
            var token = _SetupAccounts(null);

            _sessionService.AddManualPrice(_sessionId, investment, share_price);
            return View("Index", _GetCurrentInvestments(token));
        }

        [HttpGet]
        public ActionResult UpdateValuationDate(DateTime dtValaution)
        {
            _sessionService.SetValuationDate(_sessionId, dtValaution);
            return View("CashFlow", _GetCashFlowModel());
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
 
            return View("AddTransaction");
        }

        private void _ProcessCashTransaction(UserAccountToken token, DateTime transactionDate, string transactionType, string parameter, double amount)
        {
            _cashTransactionManager.AddTransaction(token, _sessionService.GetValuationDate(_sessionId),
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
                return AddReceiptTransaction();
            }

            return View("CashFlow", _GetCashFlowModel());
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

        private ActionResult _RemoveCashTransaction(Transaction transaction)
        {
            var token = _SetupAccounts(null);
            _cashTransactionManager.RemoveTransaction(token, transaction.ValuationDate, transaction.TransactionDate, transaction.TransactionType, transaction.Parameter);
            return View("CashFlow", _GetCashFlowModel());

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

        //[HttpGet]
        //public ActionResult BuildReport()
        //{

        //}
    }
}