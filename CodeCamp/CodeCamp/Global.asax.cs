﻿using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using CodeCamp.Infrastructure.Controllers;
using CodeCamp.Infrastructure.Filters;
using CodeCamp.Infrastructure.IoC;
using CodeCamp.Infrastructure.Routing;
using CodeCamp.Infrastructure.Views;
using NLog;

namespace CodeCamp {
    public class MvcApplication : HttpApplication {
        static readonly Logger Log = LogManager.GetLogger(typeof(MvcApplication).FullName);

        protected void Application_Start() {
            // Work around nasty .NET framework bug
            try {
                new Uri("http://fail/first/time?only=%2bplus");
            } catch(Exception) {}

            Log.Info("Tally Code Camp - Starting");

            Error += delegate {
                var exception = Server.GetLastError();
                Log.Error(exception);

                Response.Clear();
                Server.ClearError();
                Response.Redirect("~/error");
            };

            ContainerConfig.Configure();
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            new RouteConfig(RouteTable.Routes).Configure();

            ControllerBuilder.Current.SetControllerFactory(new ControllerFactory());

            Log.Info("Tally Code Camp - Started");
        }

        protected void Session_Start(Object sender, EventArgs e) {
            Session["init"] = 0; //need to access session in order to get a consistent session id
        }
    }
}