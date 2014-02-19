using AppPoolManage.Web.UI.Attribute;
using AppPoolManage.Web.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AppPoolManage.Web.UI.Controllers
{
    [AkqaAuthorizeAttribute]
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            return View(GetWebSites());
        }

        public ActionResult ControlAppPool(string appPoolName, string command)
        {
            try
            {
                CommandType poolCommand = (CommandType)Enum.Parse(typeof(CommandType), command, true);
                var doResult = AppPoolProvider.ControlAppPool(appPoolName, poolCommand);
                StringBuilder message = new StringBuilder();
                message.AppendFormat("PoolName:" + appPoolName + " " + command + " ");
                if (doResult.Equals(Constants.Success))
                {
                    message.Append("success");
                }
                else
                {
                    message.Append("fail " + doResult);
                }
                TempData["Message"] = message.ToString();
            }
            catch
            {
                TempData["Message"] = "system error,please contact with the administrator";
            }
            return RedirectToAction("index");

        }

        public ActionResult DeleteUmbracoConfig(string path)
        {
            var doResult = AppPoolProvider.DeleteUmbracoConfig(path);
            TempData["Message"] = "Delete UmbracoConfig " + doResult.ToString();
            return RedirectToAction("index", GetWebSites());
        }

        private List<WebSitePro> GetWebSites()
        {
            return AppPoolProvider.GetWebSites();
        }



    }
}
