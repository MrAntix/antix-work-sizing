using System;
using System.Web;
using System.Web.Optimization;

namespace Antix.Work.Sizing.Portal
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            RegisterBundles(BundleTable.Bundles);
        }

        static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(
                new ScriptBundle("~/bundles/scripts")
                    .Include("~/Scripts/jquery-{version}.js")
                    .Include("~/Scripts/jquery.cookie.js")
                    .Include("~/Scripts/jquery.signalR-{version}.js")
                    .Include("~/Scripts/Common/define.js")
                    .IncludeDirectory("~/Scripts/Common/Modules/", "*.js")
                    .IncludeDirectory("~/Scripts/App/", "*.js")
                );

            bundles.Add(
                new ScriptBundle("~/bundles/scripts-ie")
                    .Include("~/Scripts/json2.js")
                    .Include("~/Scripts/jquery-ie.js")
                    .Include("~/Scripts/jquery.cookie.js")
                    .Include("~/Scripts/jquery.signalR-{version}.js")
                    .Include("~/Scripts/Common/define.js")
                    .IncludeDirectory("~/Scripts/Common/Modules/", "*.js")
                    .IncludeDirectory("~/Scripts/App/", "*.js")
                );

            bundles.Add(new StyleBundle("~/bundles/styles")
                            .IncludeDirectory("~/Styles", "Site.css")
                );

            bundles.Add(new StyleBundle("~/bundles/styles-ie")
                            .IncludeDirectory("~/Styles", "Site.css")
                            .IncludeDirectory("~/Styles/ie", "Site.css")
                );
        }
    }
}