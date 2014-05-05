using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AppPoolManage.Web.UI.Controllers
{
    public class BaseController : Controller
    {
        private static string AuthUser = "wppuser";
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            CheckUser(requestContext.HttpContext.ApplicationInstance.Context);
        }

        private void CheckUser(HttpContext context)
        {
            if (context.Request.IsAuthenticated)
            {
                var loginName = GetUserLoginName(context);
                if (AuthUser.Equals(loginName))
                {
                    return;
                }
            }
            context.Response.Redirect("~/LoginError.html");
        }

        private string GetUserLoginName(HttpContext context)
        {
            if (context == null)
                return null;

            if (context.Request.IsAuthenticated == false)
                return null;

            string userName = context.User.Identity.Name;

            string[] array = userName.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
            if (array.Length == 2)
                return array[1];

            return null;
        }
    }
}