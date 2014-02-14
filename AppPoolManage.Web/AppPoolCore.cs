using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Management;
using System.Collections;
using Microsoft.Web.Administration;
using Microsoft.Web.Management.Server;
using System.DirectoryServices;
using System.IO;

namespace AppPoolManage.Web
{
    public static class AppPoolCore
    {

        public static string GetIISVersion()
        {
            DirectoryEntry getEntity = GetDirectoryEntry(Constants.AddressHeader + Constants.INFO, null, null);
            return getEntity.Properties[Constants.MajorIISVersionNumber].Value.ToString();
        }

        public static List<WebSitePro> GetWebSites()
        {
            List<WebSitePro> webSitePros = new List<WebSitePro>();

            var root = new DirectoryEntry(Constants.AddressHeader);

            foreach (DirectoryEntry site in root.Children)
            {
                if (site.SchemaClassName == "IIsWebServer")
                {
                    var website = new WebSitePro();
                    website.SiteName = site.Properties["ServerComment"].Value.ToString();
                    website.SiteStatus = GetWebSiteStatus(Convert.ToInt32(site.Properties["ServerState"].Value.ToString()));

                    foreach (DirectoryEntry vsite in site.Children)
                    {
                        if (vsite.SchemaClassName == "IIsWebVirtualDir")
                        {
                            website.PoolName = vsite.Properties["apppoolid"].Value.ToString();
                            website.PoolStatus = GetStatus(website.PoolName);
                            website.FilePath = vsite.Properties["Path"].Value.ToString();
                            website.IsUmbraco = CheckUmbraco(website.FilePath);
                        }

                    }
                    webSitePros.Add(website);
                }
            }

            return webSitePros;

        }

        public static Dictionary<string, string> GetAppPools(string username = null, string pwd = null)
        {
            return GetApplicationPools(Constants.AddressHeader, Constants.Username, Constants.Pwd);
        }

        public static bool ControlAppPool(string appPoolName, string command, string username = null, string pwd = null)
        {
            string appPoolPath = Constants.AddressHeader + "/AppPools/" + appPoolName;

            try
            {
                using (DirectoryEntry appPoolEntry = username == null ? new DirectoryEntry(appPoolPath) : new DirectoryEntry(appPoolPath, username, pwd, AuthenticationTypes.Secure))
                {
                    appPoolEntry.Invoke(command, null);
                    appPoolEntry.Close();
                    return true;
                }
            }
            catch
            {
                return false;
            }

        }

        public static string DeleteUmbracoConfig(string path)
        {
            try
            {
                FileInfo file = new FileInfo(path + "\\App_Data\\umbraco.config");
                if (file.Exists)
                {
                    file.Delete();
                    return DeleteStates.Successs.ToString();
                }
                else
                {
                    return DeleteStates.NotExist.ToString();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private static bool CheckUmbraco(string path)
        {
            DirectoryInfo rootFolder = new DirectoryInfo(path + "\\App_Data\\");
            if (rootFolder.Exists)
            {
                foreach (FileInfo file in rootFolder.GetFiles())
                {
                    if (file.Name.Equals("umbraco.config", StringComparison.CurrentCultureIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static Dictionary<string, string> GetApplicationPools(string computerName, string username, string pwd)
        {
            var root = GetDirectoryEntry(Constants.AddressHeader + "/AppPools", username, pwd);

            if (root == null) return null;
            var items = (from DirectoryEntry entry in root.Children let properties = entry.Properties select entry.Name).ToList();

            var appPools = items.ToDictionary(appPoolName => appPoolName, GetStatus);
            return appPools;
        }

        private static string GetStatus(string appPoolName)
        {
            string status = string.Empty;
            string appPoolPath = Constants.AddressHeader + "/AppPools/" + appPoolName;
            int intStatus = 0;
            try
            {
                var w3svc = new DirectoryEntry(appPoolPath);
                intStatus = (int)w3svc.InvokeGet("AppPoolState");
                switch (intStatus)
                {
                    case 2:
                        status = SiteStates.Running.ToString();
                        break;
                    case 4:
                        status = SiteStates.Stopped.ToString();
                        break;
                    default:
                        status = SiteStates.Paused.ToString();
                        break;
                }
            }
            catch
            {
                return null;
            }
            return status;
        }

        private static string GetWebSiteStatus(int status)
        {
            string result = SiteStates.Unknown.ToString();

            return (status.Equals((int)EStates.Start) ? SiteStates.Running.ToString() :
                      status.Equals((int)EStates.Stop) ? SiteStates.Stopped.ToString() :
                      status.Equals((int)EStates.Pause) ? SiteStates.Paused.ToString() :
                      status.ToString());

        }

        private static string GetSiteIdByName(string siteName)
        {
            DirectoryEntry root = new DirectoryEntry(Constants.AddressHeader); ;
            foreach (DirectoryEntry e in root.Children)
            {
                if (e.SchemaClassName == "IIsWebServer")
                {
                    if (e.Properties["ServerComment"].Value.ToString().Equals(siteName, StringComparison.OrdinalIgnoreCase))
                    {
                        return e.Name;
                    }
                }
            }
            return null;
        }

        private static DirectoryEntry GetDirectoryEntry(string path, string username, string pwd)
        {
            DirectoryEntry root = null;

            try
            {
                root = username == null ? new DirectoryEntry(path) : new DirectoryEntry(path, username, pwd, AuthenticationTypes.Secure);
            }

            catch
            {
                throw new ArgumentException("username or pwd is wrong");
            }

            return root;
        }


    }
}
