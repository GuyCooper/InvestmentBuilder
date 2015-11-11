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
        private Services.InvestmentRecordBuilderService _service;

        public InvestmentRecordController(InvestmentBuilder.InvestmentBuilder investmentBuilder, IDataLayer dataLayer, IAuthorizationManager authorizationManager,
                                          Services.InvestmentRecordBuilderService service)  
        {
            _investmentBuilder = investmentBuilder;
            _clientInterface = dataLayer.ClientData;
            _authorizationManager = authorizationManager;
            _service = service;
        }

        private void SetupAccounts(string selectedAccount)
        {
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

                UserAccountToken token = _authorizationManager.GetCurrentTokenForUser(username);
                ViewBag.accountId = accounts.Select(x =>
                   new SelectListItem
                   {
                       Text = x,
                       Value = x,
                       Selected = token.Account == x
                   });
            }
        }

        // GET: InvestmentRecord
        [Route("")]
        [Route("index")]
        public ActionResult Index()
        {
            SetupAccounts(null);
            return View(_service.GetRecords());
        }

        [HttpGet]
        public ActionResult Create()
        {
            SetupAccounts(null);
            return View();
        }

        [HttpPost]
        public ActionResult Create(InvestmentRecordModel data)
        {
            SetupAccounts(null);

            if (this.ModelState.IsValid)
            {
                _service.AddRecord(data);
            }
            else
            {
                this.ModelState.AddModelError("", "Invalid data enterted");
                return View();
            }

            return View("Index", _service.GetRecords());
        }

        [HttpGet]
        public ActionResult CashFlow()
        {
            SetupAccounts(null);
            return View();
        }

        [HttpGet]
        public ActionResult Edit(InvestmentRecordModel data)
        {
            SetupAccounts(null);
            return View();
        }

        [HttpGet]
        public ActionResult Details(InvestmentRecordModel data)
        {
            SetupAccounts(null);
            return View();
        }

        [HttpGet]
        [Route("Delete")]
        public ActionResult Delete(InvestmentRecordModel data)
        {
            SetupAccounts(null);
            _service.DeleteRecord(data);
            return View("Index", _service.GetRecords());
        }

        [HttpGet]
        public ActionResult UpdateAccount(string accountId)
        {
            SetupAccounts(accountId);
            return View("Index", _service.GetRecords());
        }
    }
}