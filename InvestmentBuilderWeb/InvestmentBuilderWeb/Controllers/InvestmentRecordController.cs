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
        private IClientDataInterface _clientInterface;
        private IAuthorizationManager _authorizationManager;
        private InvestmentRecordSessionService _sessionService;
        private string _sessionId;

        public InvestmentRecordController(InvestmentBuilder.InvestmentBuilder investmentBuilder, IDataLayer dataLayer, IAuthorizationManager authorizationManager,
                                          IApplicationSessionService sessionService)  
        {
            _investmentBuilder = investmentBuilder;
            _clientInterface = dataLayer.ClientData;
            _authorizationManager = authorizationManager;
            _sessionService = sessionService as InvestmentRecordSessionService;
            _sessionId = System.Web.HttpContext.Current.Session.SessionID;
        }

        //setup the user account accounttoken and populate accounts list for user. if selectedAccount is null then just uses
        //first account for user if no usertoken setup for user otherwise just uses existing token 
        private UserAccountToken _SetupAccounts(string selectedAccount)
        {
            UserAccountToken token = null;
            if (User != null && User.Identity != null)
            {
                var username = User.Identity.GetUserName();
                var accounts = _clientInterface.GetAccountNames(User.Identity.GetUserName()).ToList();
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
            return _investmentBuilder.GetCurrentInvestments(token, _sessionService.GetManualPrices(_sessionId)).Select(x => x.ToCompanyDataModel());
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
            return View();
        }

        [HttpPost]
        public ActionResult Create(Stock data)
        {
            var token = _SetupAccounts(null);
            if (this.ModelState.IsValid)
            {
                _investmentBuilder.UpdateTrades(token, data.ToTrades(TransactionType.BUY), null);   
            }
            else
            {
                this.ModelState.AddModelError("", "Invalid data enterted");
                return View();
            }

            return View("Index", _GetCurrentInvestments(token));
        }

        [HttpGet]
        public ActionResult CashFlow()
        {
            _SetupAccounts(null);
            return View();
        }

        [HttpGet]
        public ActionResult Edit(string name)
        {
            var token =_SetupAccounts(null);
            var tradeItem = _clientInterface.GetTradeItem(token, name);
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
                return View(model);
            }
            return null;
        }

        [HttpPost]
        public ActionResult Edit(TradeItemModel tradeItem)
        {
            var token = _SetupAccounts(null);
            _investmentBuilder.UpdateTrades(token, tradeItem.ToTrades(tradeItem.Action), tradeItem.GetManualPrices());
            return View("Index", _GetCurrentInvestments(token));
        }

        [HttpGet]
        [Route("Delete")]
        public ActionResult Delete(string name)
        {
            //_service.DeleteRecord(data);
            var token = _SetupAccounts(null);
            var tradeItem = _clientInterface.GetTradeItem(token, name);
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
            return View("Index", _GetCurrentInvestments(token));
        }

        [HttpGet]
        public ActionResult UpdatePrice(string investment, double share_price)
        {
            var token = _SetupAccounts(null);

            _sessionService.AddManualPrice(_sessionId, investment, share_price);
            return View("Index", _GetCurrentInvestments(token));
        }
    }
}