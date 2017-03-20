using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InvestmentBuilder;
using InvestmentBuilderCore;
using InvestmentBuilderWeb.Interfaces;
using InvestmentBuilderWeb.Translators;
using InvestmentBuilderWeb.Models;

namespace InvestmentBuilderWeb.Controllers
{
    public abstract class InvestmentBaseController : Controller
    {
        protected IAuthorizationManager _authorizationManager;
        protected AccountManager _accountManager;
        protected InvestmentBuilder.InvestmentBuilder _investmentBuilder;
        protected BrokerManager _brokerManager;
        protected IApplicationSessionService _sessionService;
        protected IClientDataInterface _clientData;

        protected string SessionId { get; set; }

        protected InvestmentBaseController(IAuthorizationManager authorizationManager
                                           ,AccountManager accountManager
                                           ,InvestmentBuilder.InvestmentBuilder investmentBuilder
                                           ,BrokerManager brokerManager
                                           , IApplicationSessionService sessionService
                                           , IClientDataInterface clientData)
        {
            _authorizationManager = authorizationManager;
            _accountManager = accountManager;
            _investmentBuilder = investmentBuilder;
            _brokerManager = brokerManager;
            _sessionService = sessionService;
            _clientData = clientData;
            SessionId = System.Web.HttpContext.Current.Session.SessionID;
        }

        protected string _GetThisUserName()
        {
            return "bob@bob.com"; //just for testing!!
            //if (User != null && User.Identity != null)
            //{
            //    return User.Identity.GetUserName();
            //}
            //return null;
        }
        //setup the user account accounttoken and populate accounts list for user. if selectedAccount is null then just uses
        //first account for user if no usertoken setup for user otherwise just uses existing token 
        protected UserAccountToken _SetupAccounts(string selectedAccount)
        {
            UserAccountToken token = null;
            var username = _GetThisUserName();
            if (username != null)
            {
                var accounts = _accountManager.GetAccountNames(username).ToList();
                if (_authorizationManager.GetCurrentTokenForUser(username) == null)
                {
                    _authorizationManager.SetUserAccountToken(username, accounts.FirstOrDefault());
                }

                if (string.IsNullOrEmpty(selectedAccount) == false)
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

        protected InvestmentSummaryModel _GetSummaryModel(UserAccountToken token, DateTime? dtValuation)
        {
            if (dtValuation.HasValue)
            {
                var summaryData = _sessionService.GetSummaryData(SessionId, dtValuation.Value);
                if (summaryData == null)
                {
                    summaryData = _investmentBuilder.BuildAssetReport(token, dtValuation.Value, false, null, null).ToInvestmentSummaryModel();
                    _sessionService.SetSummaryData(SessionId, dtValuation.Value, summaryData);
                }
                return summaryData;
            }

            return new InvestmentSummaryModel
            {
                AccountName = token.Account,
                ValuationDate = _sessionService.GetValuationDate(SessionId),
                ValuePerUnit = "1"
            };
        }

        //needs to be called before time one of the main views are displayed
        protected void _GenerateSummary()
        {
            var token = _SetupAccounts(null);
            var dtValuation = _sessionService.GetValuationDate(SessionId);
            var dtPrevious = _clientData.GetPreviousAccountValuationDate(token, dtValuation);
            ViewBag.SummaryData = _GetSummaryModel(token, dtPrevious);
            ViewBag.EnableBuildReport = _sessionService.GetEnableBuildReport(SessionId);
        }

        protected ViewResult _CreateMainView(string name, object model)
        {
            _GenerateSummary();
            return View(name, model);
        }

        protected IEnumerable<CompanyDataModel> _GetCurrentInvestments(UserAccountToken token)
        {
            return _investmentBuilder.GetCurrentInvestments(token, _sessionService.GetManualPrices(SessionId))
                .OrderBy(x => x.Name)
                .Select(x => x.ToCompanyDataModel());
        }
    }
}