using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace webApp
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundle)
        {
            StyleBundle myLoginCssBundle = new StyleBundle("~/asset/css/MyLoginCSS");
            myLoginCssBundle.Include("~/assets/css/sweetalert2.min.css",
                "~/assets/css/bootstrap.min.css",
                "~/assets/css/app.min.css").Include("~/assets/css/icons.min.css", new CssRewriteUrlTransform());            

            StyleBundle mySiteCssBundle = new StyleBundle("~/asset/css/MySiteCSS");
            mySiteCssBundle.Include("~/assets/select2/css/select2.min.css",
                "~/assets/css/sweetalert2.min.css",
                "~/assets/css/dataTables.bootstrap4.min.css",
                "~/assets/css/buttons.bootstrap4.min.css",
                "~/assets/css/responsive.bootstrap4.min.css",
                "~/assets/css/bootstrap.min.css",
                "~/assets/css/app.min.css").Include("~/assets/css/icons.min.css", new CssRewriteUrlTransform()); 

            ScriptBundle myJSBundle = new ScriptBundle("~/asset/js/mySiteScript");
            myJSBundle.Include("~/assets/js/pages/jquery.min.js",
                "~/assets/js/pages/bootstrap.bundle.min.js",
                "~/assets/js/pages/metisMenu.min.js",
                "~/assets/js/pages/simplebar.min.js",
                "~/assets/js/pages/waves.min.js",
                "~/assets/select2/js/select2.min.js",
                "~/assets/js/sweetalert2.min.js",
                "~/assets/js/jquery.dataTables.min.js",
                "~/assets/js/dataTables.bootstrap4.min.js",
                "~/assets/js/dataTables.buttons.min.js",
                "~/assets/js/buttons.bootstrap4.min.js",
                "~/assets/js/jszip.min.js",
                "~/assets/js/pdfmake.min.js",
                "~/assets/js/vfs_fonts.js",
                "~/assets/js/buttons.html5.min.js",
                "~/assets/js/buttons.print.min.js",
                "~/assets/js/buttons.colVis.min.js",
                "~/assets/js/dataTables.responsive.min.js",
                "~/assets/js/responsive.bootstrap4.min.js");

            bundle.Add(myLoginCssBundle);
            bundle.Add(mySiteCssBundle);
            bundle.Add(myJSBundle);

            BundleTable.EnableOptimizations = true;
        }
    }
}