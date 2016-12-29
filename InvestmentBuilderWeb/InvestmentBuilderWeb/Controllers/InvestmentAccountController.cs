using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using InvestmentBuilder;
using InvestmentBuilderCore;
using InvestmentBuilderWeb.Models;
using InvestmentBuilderWeb.Interfaces;
using InvestmentBuilderWeb.Translators;
using Newtonsoft.Json;

namespace InvestmentBuilderWeb.Controllers
{
    public sealed class InvestmentAccountController : InvestmentBaseController
    {
        public InvestmentAccountController(IAuthorizationManager authorizationManager
                                           ,AccountManager accountManager
                                           ,InvestmentBuilder.InvestmentBuilder investmentBuilder
                                           , BrokerManager brokerManager
                                           , IApplicationSessionService sessionService
                                           , IDataLayer dataLayer
                                           ) :
            base(authorizationManager, accountManager, investmentBuilder, brokerManager, sessionService, dataLayer.ClientData)
        {
        }

        private void _AuthorisationDropdown()
        {
            ViewBag.AuthorisationType = Enum.GetNames(typeof(AuthorizationLevel)).Select(x =>
            {
                var item = new SelectListItem
                {
                    Text = x,
                    Value = x
                };
                return item;
            }).ToList();
        }

        private ActionResult _createInvestmentAccountPage()
        {
            var accountToken = _SetupAccounts(null);

            ViewBag.Currencies = _investmentBuilder.GetAllCurrencies().Select(x =>
              new SelectListItem
              {
                  Text = x,
                  Value = x,
                  Selected = x == "GBP"
              });

            ViewBag.Brokers = _brokerManager.GetBrokers().Select(x =>
              new SelectListItem
              {
                  Text = x,
                  Value = x
              });

            //by default, user is always made an adminstrstor of any accounts
            //they create
            _AuthorisationDropdown();

            var account = new AccountModelDto();
            account.AccountType = "Club";
            account.Members.Add(new AccountMemberDto
            {
                MemberID = accountToken.User,
                AuthorisationType = AuthorizationLevel.ADMINISTRATOR.ToString()
            });

            return View("AddAccount", account);

        }

        public ActionResult _editInvestmentAccountPage()
        {
            var token = _SetupAccounts(null);
            var account = _accountManager.GetAccountData(token, _sessionService.GetValuationDate(SessionId));
            var dto = account.ToAccountModelDto();

            _AuthorisationDropdown();

            ViewBag.Title = "Edit Account " + dto.AccountName;
            return View("EditAccount", dto);
        }

        [HttpGet]
        public ActionResult AddInvestmentAccount()
        {
            return _createInvestmentAccountPage();
        }

        [HttpGet]
        public ActionResult EditInvestmentAccount()
        {
            return _editInvestmentAccountPage();
        }

        [HttpPost]
        public ActionResult AddInvestmentAccount(AccountModelDto account)
        {
            var username = _GetThisUserName();
            if (username != null)
            {
                account.Members = JsonConvert.DeserializeObject<IList<AccountMemberDto>>(account.SerialisedMembers);
                //can only update account information if this is a valid user
                bool updated = _accountManager.CreateUserAccount(
                                                    username,
                                                    account.ToAccountModel(),
                                                    _sessionService.GetValuationDate(SessionId));
                if (updated == false)
                {
                    //update failed, probably due to validation error, return same
                    //page and let user try again
                    setupErrors(username);
                    return _createInvestmentAccountPage();
                }
            }
            return _CreateMainView("~/Views/InvestmentRecord/Index.cshtml", _GetCurrentInvestments(_SetupAccounts(null)));
        }

        [HttpPost]
        public ActionResult EditInvestmentAccount(AccountModelDto account)
        {
            var username = _GetThisUserName();
            if (username != null)
            {
                account.Members = JsonConvert.DeserializeObject<IList<AccountMemberDto>>(account.SerialisedMembers);
                //can only update account information if this is a valid user
                bool updated = _accountManager.UpdateUserAccount(
                                                    username,
                                                    account.ToAccountModel(),
                                                    _sessionService.GetValuationDate(SessionId));
                if (updated == false)
                {
                    //update failed, probably due to validation error, return same
                    //page and let user try again
                    setupErrors(username);
                    return _editInvestmentAccountPage();
                }
            }
            return _CreateMainView("~/Views/InvestmentRecord/Index.cshtml", _GetCurrentInvestments(_SetupAccounts(null)));
        }
    }
}