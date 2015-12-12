using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;
using InvestmentBuilderWeb.Interfaces;
using Microsoft.Practices.Unity;

namespace InvestmentBuilderWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        public void Session_Start(object sender, EventArgs e)
        {
            var sessionService = App_Start.UnityConfig.GetConfiguredContainer().Resolve<IApplicationSessionService>();
            if(sessionService != null)
            {
                sessionService.StartSession(HttpContext.Current.Session.SessionID);
            }
        }

        public void Session_End(object sender, EventArgs e)
        {
            var sessionService = App_Start.UnityConfig.GetConfiguredContainer().Resolve<IApplicationSessionService>();
            if (sessionService != null)
            {
                sessionService.EndSession(HttpContext.Current.Session.SessionID);
            }
        }
    }
}
