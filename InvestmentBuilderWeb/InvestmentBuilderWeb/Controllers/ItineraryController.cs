using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InvestmentBuilderWeb.Models;
using System.Text;

namespace InvestmentBuilderWeb.Controllers
{
    public class ItineraryController : Controller
    {
        [HttpGet()]
        public JsonResult VerifyAvailability(DateTime When, string Description)
        {
            //TODO specify the code here for verifying availability
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetAdParts()
        {
        var data = new List<AdParts>();
        data.Add(new AdParts
        {
        image = "<img src='http://placehold.it/180x100'>",
        caption =
        "Lorem ipsum dolor sit amet, consectetur adipiscing elit",
        url = "http://#"
        });
        data.Add(new AdParts
        {
        image = "<img src='http://placehold.it/180x100'>", url = "#",
        caption = "Donec adipiscing eros eget dui aliquet, nec tur."
        });
        return Json(data);
        }

        [HttpPost]
        public JsonResult GetAdsSimple()
        {
            var data = new StringBuilder();
            data.Append("<ul>");
            data.Append("<li><img src='http://placehold.it/180x100'></li>");
            data.Append("<li><img src='http://placehold.it/180x100/ffffff/aaaaff'></li></ul>");
            return Json(data.ToString( ));
            }
        // GET: Itinerary
        public ActionResult Index()
        {
            ViewBag.accountId = new List<SelectListItem> { new SelectListItem { Text = "test", Value = "test", Selected = true } };
            return View();
        }

        public ActionResult Create()
        {
            //var model = new ItineraryItem
            //{
            //    Description = "hello",
            //    When = DateTime.Now
            //    Duration = "one week"
            //};

            ViewBag.accountId = new List<SelectListItem> { new SelectListItem { Text = "test", Value = "test", Selected = true } };
            return View();
        }

        [HttpPost]
        public ActionResult Create(ItineraryItem data)
        {
            if(this.ModelState.IsValid)
            {
                
            }
            else
            {
                this.ModelState.AddModelError("", "Invalid data enterted");
            }

            ViewBag.accountId = new List<SelectListItem> { new SelectListItem { Text = "test", Value = "test", Selected = true } };
            return View();
        }

        [HttpPost]
        public JsonResult GetHelp()
        {
            return Json("<p>This is the help page</p>");
        }
    }

    public class AdParts
    {
        public string image { get; set; }
        public string url { get; set; }
        public string caption { get; set; }
    }
}