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
        public static Dictionary<string, string> GetAppPools(string username = null, string pwd = null)
        {
            string computerName = System.Environment.MachineName;
            return GetApplicationPools(computerName, username, pwd);
        }

        public static bool ControlAppPool(string appPoolName, string command, string username = null, string pwd = null)
        {
            string appPoolPath = "IIS://localhost/W3SVC/AppPools/" + appPoolName;

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
            var root = GetDirectoryEntry(@"IIS://" + computerName + "/W3SVC/AppPools", username, pwd);

            if (root == null) return null;
            var items = (from DirectoryEntry entry in root.Children let properties = entry.Properties select entry.Name).ToList();

            var appPools = items.ToDictionary(appPoolName => appPoolName, GetStatus);
            return appPools;
        }

        private static string GetStatus(string appPoolName)
        {
            string status = string.Empty;
            string appPoolPath = @"IIS://" + System.Environment.MachineName + "/W3SVC/AppPools/" + appPoolName;
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

        private static DirectoryEntry GetDirectoryEntry(string path, string username, string pwd)
        {
            DirectoryEntry root = null;

            try
            {
                root = username == null ? new DirectoryEntry(path) : new DirectoryEntry(path, username, pwd, AuthenticationTypes.Secure);
            }

            catch (Exception e)
            {
                throw new ArgumentException("username or pwd is wrong");
            }

            return root;
        }
    }
}
