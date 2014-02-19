using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppPoolManage.Web.UI.Attribute
{
    public class AkqaAuthorizeAttribute : AuthorizeAttribute
    {
        private static string AuthUser = "akqauser";

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }
            CheckUser(httpContext.ApplicationInstance.Context);
            return true;
        }

        private void CheckUser(HttpContext context)
        {
            if (context.Request.IsAuthenticated)
            {
                var loginName = GetUserLoginName(context);
                if (AuthUser.Equals(loginName))
                {
                    return ;
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