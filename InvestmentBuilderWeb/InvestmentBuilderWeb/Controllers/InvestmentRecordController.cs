using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Threading;
using System.Threading.Tasks;
using InvestmentBuilderWeb.Models;
using InvestmentBuilder;
using InvestmentBuilderCore;
using InvestmentBuilderWeb.Translators;
using InvestmentBuilderWeb.Interfaces;
using InvestmentBuilderWeb.Utils;
using Newtonsoft.Json;
using PerformanceBuilderLib;

namespace InvestmentBuilderWeb.Controllers
{
    //[Authorize]
    [RoutePrefix("InvestmentRecord")]
    [Route("{action}")]
    public sealed class InvestmentRecordController : InvestmentBaseController
    {
        private IInvestmentRecordInterface _recordData;
        private CashAccountTransactionManager _cashTransactionManager;
        private PerformanceBuilder _performanceBuilder;

        private const string ALL = "All";

        public InvestmentRecordController(InvestmentBuilder.InvestmentBuilder investmentBuilder
                                        , IDataLayer dataLayer
                                        , IAuthorizationManager authorizationManager
                                        , IApplicationSessionService sessionService
                                        , CashAccountTransactionManager cashTransactionManager
                                        , AccountManager accountManager
                                        , BrokerManager brokerManager
                                        , PerformanceBuilder performanceBuilder
                                        )  :
            base(authorizationManager, accountManager, investmentBuilder, brokerManager, sessionService, dataLayer.ClientData)
        {
            _recordData = dataLayer.InvestmentRecordData;
            _cashTransactionManager = cashTransactionManager;
            _performanceBuilder = performanceBuilder;
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
                _investmentBuilder.UpdateTrades(token, tradeItem.ToTrades(TransactionType.BUY), null, null);   
            }
            else
            {
                this.ModelState.AddModelError("", "Invalid data enterted");
                return View("CreateTrade");
            }

            return _CreateMainView("Index", _GetCurrentInvestments(token));
        }

        private IEnumerable<CashFlowModel> _GetCashFlowModel(string sDateFrom)
        {
            var token = _SetupAccounts(null);
            var dtDateEarliest = string.IsNullOrEmpty(sDateFrom) ? _sessionService.GetValuationDate(SessionId) : DateTime.Parse(sDateFrom);
            var dtDateLatest = _sessionService.GetValuationDate(SessionId);
            var dtDateNext = dtDateLatest;

            var finished = false;
            while (!finished)
            {
                var dtDateFrom = _clientData.GetPreviousAccountValuationDate(token, dtDateNext);

                double dReceiptTotal, dPaymentTotal;
                var cashFlowModel = new CashFlowModel();
                cashFlowModel.Receipts = _cashTransactionManager.GetReceiptTransactions(token, dtDateNext, dtDateFrom, out dReceiptTotal).Select(x => x.ToReceiptCashFlowModel()).ToList();
                cashFlowModel.Payments = _cashTransactionManager.GetPaymentTransactions(token, dtDateNext, out dPaymentTotal).Select(x => x.ToPaymentCashFlowModel()).ToList();
                cashFlowModel.ReceiptsTotal = dReceiptTotal;
                cashFlowModel.PaymentsTotal = dPaymentTotal;
                cashFlowModel.ValuationDate = dtDateNext.ToString("yyyy-MM-dd"); //ISO 8601

                cashFlowModel.CanEdit = dtDateNext == dtDateLatest;
                cashFlowModel.CanBuild = cashFlowModel.CanEdit && cashFlowModel.ReceiptsTotal > 0 && cashFlowModel.ReceiptsTotal == cashFlowModel.PaymentsTotal;

                if (dtDateFrom.HasValue == false)
                {
                    finished = true;
                }
                else
                {
                    dtDateNext = dtDateFrom.Value;
                    if (dtDateFrom <= dtDateEarliest)
                    {
                        finished = true;
                    }
                }

                yield return cashFlowModel;
            }
        }

        private CashFlowModelAndParams _GetCashFlowModelAndParams(string sDateFrom)
        {
            return new CashFlowModelAndParams
            {
                CashFlows = _GetCashFlowModel(sDateFrom),
                //ReceiptParamTypes = new List<string> { "Subscription", "Sale", "Dividend", "Other" },
                ReceiptParamTypes = _cashTransactionManager.GetTransactionTypes(_cashTransactionManager.ReceiptMnemomic),
                PaymentParamTypes = _cashTransactionManager.GetTransactionTypes(_cashTransactionManager.PaymentMnemomic)
            };
        }
        //[HttpGet]
        //public ActionResult CashFlow()
        //{

        //    var model = _GetCashFlowModelAndParams(null);
        //    return _CreateMainView("CashFlow", model);
        //}

        [HttpGet]
        public string CashFlowContents(string sDateRequestedFrom)
        {
            var model = _GetCashFlowModelAndParams(sDateRequestedFrom);            
            return JsonConvert.SerializeObject(model);
        }

        [HttpGet]
        public ActionResult CashFlow()
        {
            return _CreateMainView("CashFlow", null);
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
                _investmentBuilder.UpdateTrades(token, tradeItem.ToTrades(tradeItem.Action), tradeItem.GetManualPrices(), null);
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
                _investmentBuilder.UpdateTrades(token, tradeItem.ToTrades(TransactionType.SELL), null, null);
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
            return _CreateMainView("CashFlow", _GetCashFlowModel(null));
        }

        [HttpGet]
        public string BuildReport(string sDateRequestedFrom)
        {
            var token = _SetupAccounts(null);

            IBuildMonitor monitor = _sessionService.GetBuildMonitor(SessionId);
            if(monitor == null)
            {
                monitor = new BuildReportMonitor(_GetThisUserName());
                _sessionService.SetBuildMonitor(SessionId, monitor);
            }
            monitor.StartBuilding();

            Task.Factory.StartNew( () =>
            {
                monitor.GetProgressCounter().ResetCounter(10);
                for (int i = 0; i < 10; ++i)
                {
                    System.Threading.Thread.Sleep(3000);
                    monitor.GetProgressCounter().IncrementCounter();
                }

                monitor.StopBuiliding();
                //first generate the asset report
                //var report = _investmentBuilder.BuildAssetReport(token
                //                                                , _sessionService.GetValuationDate(SessionId)
                //                                                , true
                //                                                , _sessionService.GetManualPrices(SessionId)
                //                                                , this);

                //if (report != null)
                //{
                //    //now generate the performance charts. by doing this the whole report will be persisted
                //    //to a pdf file
                //    _performanceBuilder.Run(token, _sessionService.GetValuationDate(SessionId));
                //}
            });

            return CashFlowContents(sDateRequestedFrom);
        }

        [HttpGet]
        public string CheckBuildStatus()
        {
            var monitor = _sessionService.GetBuildMonitor(SessionId);
            ReportStatus status = null;
            if (monitor != null)
            {
                status = _sessionService.GetBuildMonitor(SessionId).GetReportStatus();
            }
            else
            {
                status = new ReportStatus();
            }

            return JsonConvert.SerializeObject(status);
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

        private void _ProcessTransaction(TransactionModel transaction, string transactionType)
        {
            var token = _SetupAccounts(null);
            if (transaction.TransactionDate != null && transaction.Amount > 0)
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
        }

        [HttpGet]
        public ActionResult AddCashTransaction()
        {
            //return _AddTransactionView("Add Receipt", _cashTransactionManager.ReceiptMnemomic);
            return PartialView("AddTransactionAngular");
        }

        //[HttpGet]
        //public ActionResult AddPaymentTransaction()
        //{
        //    //return _AddTransactionView("Add Payment", _cashTransactionManager.PaymentMnemomic);
        //    return PartialView("AddTransactionAngular");
        //}

        //[HttpPost]
        //public ActionResult AddReceiptTransaction(TransactionModel transaction)
        //{
        //    return _ProcessTransaction(transaction, _cashTransactionManager.ReceiptMnemomic);
        //}
        private string _AddTransactionAngularImpl(string transactionDate, string paramType, string param, double amount, string dateRequestedFrom, string transactionType)
        {
            var transaction = new TransactionModel
            {
                TransactionDate = DateTime.Parse(transactionDate),
                ParameterType = paramType,
                Parameter = param,
                Amount = amount
            };

            _ProcessTransaction(transaction, transactionType);
            return CashFlowContents(dateRequestedFrom);
        }

        [HttpPost]
        public string AddReceiptTransactionAngular(string transactionDate, string paramType, string param, double amount, string dateRequestedFrom)
        {
            return _AddTransactionAngularImpl(transactionDate, paramType, param, amount, dateRequestedFrom, _cashTransactionManager.ReceiptMnemomic);
        }

        [HttpPost]
        public string AddPaymentTransactionAngular(string transactionDate, string paramType, string param, double amount, string dateRequestedFrom)
        {
            return _AddTransactionAngularImpl(transactionDate, paramType, param, amount, dateRequestedFrom, _cashTransactionManager.PaymentMnemomic);
        }

        //[HttpPost]
        //public ActionResult AddPaymentTransaction(TransactionModel transaction)
        //{
        //    return _ProcessTransaction(transaction, _cashTransactionManager.PaymentMnemomic);
        //}

        [HttpGet]
        public string GetParametersForTransaction(string ParameterType)
        {
            var token = _SetupAccounts(null);
            var latestRecordDate = _recordData.GetLatestRecordInvestmentValuationDate(token) ?? DateTime.Today;

            var parameters = _investmentBuilder.GetParametersForTransactionType(token, latestRecordDate, ParameterType).ToList();
            if(parameters.Count == 0)
            {
                parameters.Add(ParameterType);
            }
            else
            {
                parameters.Add(ALL);
            }

            return JsonConvert.SerializeObject(new { parameters = parameters });
        }
       
        private ActionResult _RemoveCashTransaction(Transaction transaction)
        {
            var token = _SetupAccounts(null);
            _cashTransactionManager.RemoveTransaction(token, transaction.ValuationDate, transaction.TransactionDate, transaction.TransactionType, transaction.Parameter);
            return _CreateMainView("CashFlow", _GetCashFlowModel(null));
        }

        [HttpPost]
        public string RemoveTransaction(DateTime valuationDate, DateTime transactionDate, string transactionType, string parameter, string dateRequestedFrom)
        {
            var token = _SetupAccounts(null);
            _cashTransactionManager.RemoveTransaction(token, valuationDate, transactionDate, transactionType, parameter);
            return CashFlowContents(dateRequestedFrom);
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