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

namespace AppPoolManage.Web
{
    public static class AppPoolCore
    {


        public static string GetIISVersion()
        {
            DirectoryEntry getEntity = GetDirectoryEntry(Constants.AddressHeader + "/INFO", null, null);
            return getEntity.Properties["MajorIISVersionNumber"].Value.ToString();
        }

        private static DirectoryEntry test(string path)
        {
            return new DirectoryEntry(path);
        }

        public static Dictionary<string, string> GetWebsites()
        {
            List<string> siteNames = new List<string>();

            var root = new DirectoryEntry("IIS://localhost/W3SVC");

           // DirectoryEntry root = test("IIS://localhost/W3SVC");
            foreach (DirectoryEntry e in root.Children)
            {
                if (e.SchemaClassName == "IIsWebServer")
                {
                    siteNames.Add(e.Properties["ServerComment"].Value.ToString());
                }
            }


            //DirectoryEntry root = GetDirectoryEntry(Constants.AddressHeader, null, null);
            //Dictionary<string, string> Websites = new Dictionary<string, string>();
            //foreach (DirectoryEntry e in root.Children)
            //{
            //    if (e.SchemaClassName == "IIsWebServer")
            //    {
            //        Websites.Add(e.Properties["ServerComment"].Value.ToString(), GetWebSiteStatus(e.Name));
            //    }
            //}



            return null;
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
                        status = "Running";
                        break;
                    case 4:
                        status = "Stopped";
                        break;
                    default:
                        status = "Unknown";
                        break;
                }
            }
            catch
            {
                return null;
            }
            return status;
        }

        private static string GetWebSiteStatus(string siteId)
        {
            string result = "unknown";
            DirectoryEntry root = GetDirectoryEntry(Constants.AddressHeader + siteId, Constants.Username, Constants.Pwd);
            PropertyValueCollection pvc;
            pvc = root.Properties["ServerState"];
            if (pvc.Value != null)
                result = (pvc.Value.Equals((int)eStates.Start) ? "Running" :
                          pvc.Value.Equals((int)eStates.Stop) ? "Stopped" :
                          pvc.Value.Equals((int)eStates.Pause) ? "Paused" :
                          pvc.Value.ToString());
            return result;
        }

        private static DirectoryEntry GetDirectoryEntry(string path, string username, string pwd)
        {
            return new DirectoryEntry(path);
        //    DirectoryEntry root = null;

        //    try
        //    {
        //        root = username == null ? new DirectoryEntry(path) : new DirectoryEntry(path, username, pwd, AuthenticationTypes.Secure);
        //    }

        //    catch (Exception e)
        //    {
        //        throw new ArgumentException("username or pwd is wrong");
        //    }

        //    return root;
        }


    }
}
