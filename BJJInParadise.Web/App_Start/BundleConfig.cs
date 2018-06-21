using System.Web;
using System.Web.Optimization;

namespace BJJInParadise.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
 
var jqueryCdnPath = "https://code.jquery.com/jquery-3.3.1.min.js";
 var jqueryBundle = new ScriptBundle("~/bundles/jquery", jqueryCdnPath)
 .Include("~/Scripts/jquery-{version}.min.js");
    jqueryBundle.CdnFallbackExpression = "window.jQuery";
           bundles.Add(jqueryBundle);
            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));
		var bootstrapcdn = "https://maxcdn.bootstrapcdn.com/bootstrap/4.1.1/js/bootstrap.min.js";
		var bootstrapBundle = new ScriptBundle("~/bundles/bootstrap", bootstrapcdn).Include(
                      "~/Scripts/bootstrap.js");
            bundles.Add(bootstrapBundle);
			
			
var bootstrapcssCdn = "https://maxcdn.bootstrapcdn.com/bootstrap/4.1.1/css/bootstrap.min.css";
            bundles.Add(new StyleBundle("~/Content/css", bootstrapcssCdn).Include(
                      "~/Content/bootstrap.min.css"));
					  
					  BundleTable.EnableOptimizations = true;
					   bundles.UseCdn = true;
        }
    }
}
