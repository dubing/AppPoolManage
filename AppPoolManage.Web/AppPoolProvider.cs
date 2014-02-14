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
    public static class AppPoolProvider
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
                if (site.SchemaClassName == Constants.IIsWebServer)
                {
                    var website = new WebSitePro();
                    website.SiteName = site.Properties[Constants.ServerComment].Value.ToString();
                    website.SiteStatus = GetWebSiteStatus(Convert.ToInt32(site.Properties[Constants.ServerState].Value.ToString()));

                    foreach (DirectoryEntry vsite in site.Children)
                    {
                        if (vsite.SchemaClassName == Constants.IIsWebVirtualDir)
                        {
                            website.PoolName = vsite.Properties[Constants.AppPoolId].Value.ToString();
                            website.PoolStatus = GetStatus(website.PoolName);
                            website.FilePath = vsite.Properties[Constants.Path].Value.ToString();
                            website.IsUmbraco = CheckUmbraco(website.FilePath);
                        }

                    }
                    webSitePros.Add(website);
                }
            }
            return webSitePros;
        }

         public static Dictionary<string, string> GetAppPools()
        {
            return GetAppPools(Constants.Username, Constants.Pwd);
        }

        public static Dictionary<string, string> GetAppPools(string username, string pwd)
        {
            return GetApplicationPools(Constants.AddressHeader, username, pwd);
        }



        public static bool ControlAppPool(string appPoolName, string command)
        {
            return ControlAppPool(appPoolName, command, null, null);
        }

        public static bool ControlAppPool(string appPoolName, string command, string username, string pwd)
        {
            string appPoolPath = Constants.AddressHeader + Constants.AppPools + "/" + appPoolName;

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
                FileInfo file = new FileInfo(path + Constants.App_Data + Constants.UmbracoConfig);
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
            DirectoryInfo rootFolder = new DirectoryInfo(path + Constants.UmbracoDirectory);
            return rootFolder.Exists;
        }

        private static Dictionary<string, string> GetApplicationPools(string computerName, string username, string pwd)
        {
            var root = GetDirectoryEntry(Constants.AddressHeader + Constants.AppPools, username, pwd);

            if (root == null) return null;
            var items = (from DirectoryEntry entry in root.Children let properties = entry.Properties select entry.Name).ToList();

            var appPools = items.ToDictionary(appPoolName => appPoolName, GetStatus);
            return appPools;
        }

        private static string GetStatus(string appPoolName)
        {
            string status = string.Empty;
            string appPoolPath = Constants.AddressHeader + Constants.AppPools + "/" + appPoolName;
            int intStatus = 0;
            try
            {
                var w3svc = new DirectoryEntry(appPoolPath);
                intStatus = (int)w3svc.InvokeGet(Constants.AppPoolState);
                return ((PoolStates)intStatus).ToString();
            }
            catch
            {
                return PoolStates.Unknown.ToString();
            }
        }

        private static string GetWebSiteStatus(int status)
        {
            try
            {
                return ((SiteStates)status).ToString();
            }
            catch
            {
                return SiteStates.Unknown.ToString();
            }

        }

        private static string GetSiteIdByName(string siteName)
        {
            DirectoryEntry root = new DirectoryEntry(Constants.AddressHeader); ;
            foreach (DirectoryEntry e in root.Children)
            {
                if (e.SchemaClassName == Constants.IIsWebServer)
                {
                    if (e.Properties[Constants.ServerComment].Value.ToString().Equals(siteName, StringComparison.OrdinalIgnoreCase))
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
