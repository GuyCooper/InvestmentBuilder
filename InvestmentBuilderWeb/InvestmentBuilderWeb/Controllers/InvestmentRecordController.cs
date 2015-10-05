using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InvestmentBuilderWeb.Models;
using InvestmentBuilderWeb.Services;
using Microsoft.AspNet.Identity;

namespace InvestmentBuilderWeb.Controllers
{
    [Authorize]
    [RoutePrefix("InvestmentRecord")]
    [Route("{action}")]
    public class InvestmentRecordController : Controller
    {
        private InvestmentRecordBuilderService _service;

        public InvestmentRecordController(InvestmentRecordBuilderService service)  
        {
            _service = service;

        }
        // GET: InvestmentRecord
        [Route("")]
        [Route("index")]
        public ActionResult Index()
        {
            var username = User.Identity.GetUserName();
            return View(_service.GetRecords());
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(InvestmentRecordModel data)
        {
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
            return View();
        }

        [HttpGet]
        public ActionResult Edit(InvestmentRecordModel data)
        {
            return View();
        }

        [HttpGet]
        public ActionResult Details(InvestmentRecordModel data)
        {
            return View();
        }

        [HttpGet]
        [Route("Delete")]
        public ActionResult Delete(InvestmentRecordModel data)
        {
            _service.DeleteRecord(data);
            return View("index", _service.GetRecords());
        }
    }
}