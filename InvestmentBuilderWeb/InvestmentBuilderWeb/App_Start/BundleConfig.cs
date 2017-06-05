using System.Web;
using System.Web.Optimization;

namespace InvestmentBuilderWeb
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui*"));

            bundles.Add(new ScriptBundle("~/bundles/InvestmentBuilderWeb").Include(
                       "~/Scripts/myapp/CommonTypes.js",
                       "~/Scripts/myapp/InvestmentAccount.js",
                       "~/Scripts/myapp/Portfolio.js",
                       "~/Scripts/myapp/CashFlow.js",
                       "~/Scripts/myapp/Utils.js",
                       "~/Scripts/myapp/CreateTrade.js",
                       "~/Scripts/myapp/RegisterModules.js"
                       ));

            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                       "~/Scripts/angular.js",
                       "~/Scripts/ui-bootstrap-tpls-2.5.0.js"));

              // Use the development version of Modernizr to develop with and learn from. Then, when you're
              // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
              bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/jquery-ui.css",
                      "~/Content/jquery-ui.structure.css",
                      "~/Content/jquery-ui.theme.css",
                      "~/Content/InvestmentBuilderWeb.css",
                      "~/Content/ag-Grid/ag-grid.css",
                      "~/Content/ag-Grid/theme-bootstrap.css"));

            bundles.Add(new ScriptBundle("~/bundles/agGrid").Include(
                "~/Scripts/ag-Grid/ag-grid.js"));

        }
    }
}
