﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace epos.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(
                new ScriptBundle("~/scripts/libraries")
                    .Include("~/scripts/jquery-{version}.js")
                    .Include("~/scripts/knockout-{version}.js")
                    .Include("~/scripts/knockout.validation.js")
                    .Include("~/scripts/knockout.viewmodel.js")
                    .Include("~/scripts/toastr.js")
                    .Include("~/scripts/foundation/foundation.js")
                    .IncludeDirectory("~/scripts/foundation", "*.js")
                    .Include("~/scripts/moment-with-langs.js")
                );

            bundles.Add(
                new StyleBundle("~/content/css")
                    .Include("~/content/normalize.css")
                    .Include("~/content/foundation.css")
                    .Include("~/content/foundation-icons.css")
                    .Include("~/content/foundation-datepicker.css")
                    .Include("~/content/durandal.css")
                    .Include("~/content/toastr.css")
                    .Include("~/content/app.css")
                );
        }
    }
}