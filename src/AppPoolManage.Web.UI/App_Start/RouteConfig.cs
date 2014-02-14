using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AppPoolManage.Web.UI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
              name: "AppPool",
              url: "{controller}/{action}/{appPoolName}/{command}",
              defaults: new { controller = "Home", action = "ControlAppPool", appPoolName = UrlParameter.Optional, command = UrlParameter.Optional }
          );

        }
    }
}