﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace HLGranite.Mvc
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public const string MONEY_FORMAT = "###,###,###,##0.00";// "C2";

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            // HACK: Customize Malaysia currency format to 2 digit decimal. Default only shown RM1,234. But we need RM1,234.50
            System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo(System.Threading.Thread.CurrentThread.CurrentCulture.ToString());//"ms-MY"
            culture.NumberFormat.CurrencyDecimalDigits = 2;
            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
        }
    }
}