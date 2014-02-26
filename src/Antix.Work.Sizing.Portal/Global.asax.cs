using System;
using System.Web;
using System.Web.Optimization;

using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Antix.Work.Sizing.Portal
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            RegisterBundles(BundleTable.Bundles);

            GlobalHost.DependencyResolver
                .Register(typeof(IJavaScriptMinifier), () => new AjaxMinMinifier());
        }

        class AjaxMinMinifier : IJavaScriptMinifier
        {
            public string Minify(string source)
            {
                return new Minifier().MinifyJavaScript(source);
            }
        }

        static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(
                new ScriptBundle("~/bundles/scripts")
                    .IncludeDirectory("~/Scripts/GoogleAnalytics/", "*.js")
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

            bundles.Add(
                new ScriptBundle("~/bundles/scripts-app")
                    .Include("~/Scripts/app.js")
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