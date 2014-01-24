using AppPoolManage.Web.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AppPoolManage.Web.UI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View(GetAppPools());
        }

        public ActionResult ControlAppPool(string appPoolName, string command)
        {
            var doResult = AppPoolCore.ControlAppPool(appPoolName, command);
            StringBuilder message = new StringBuilder();
            message.Append("PoolName:");
            message.Append(appPoolName);
            message.Append(" ");
            message.Append(command);
            message.Append(" ");
            if (doResult)
            {
                message.Append("success");
            }
            else
            {
                message.Append("fail");
            }
            ViewData["Message"] = message.ToString();

            return View("index", GetAppPools());
        }

        private IList<AppPool> GetAppPools()
        {
            var appPools = AppPoolCore.GetAppPools();
            return appPools.Select(appPool => new AppPool { AppPoolsName = appPool.Key, Status = appPool.Value }).ToList();
        }

    }
}
