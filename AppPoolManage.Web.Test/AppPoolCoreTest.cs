using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.DirectoryServices;
using System.Collections.Generic;

namespace AppPoolManage.Web.Test
{
    [TestClass]
    public class AppPoolCoreTest
    {
        [TestMethod]
        public void GetAppPoolsTest()
        {
            var poolsName = AppPoolProvider.GetAppPools();
            Assert.AreEqual(poolsName != null && poolsName.Count>0, true);
        }

        [TestMethod]
        public void GetVersionTest_VerifyVersionNumber()
        {
            var iisVersion = AppPoolProvider.GetIISVersion();
            Assert.AreEqual(iisVersion, "7");
        }

        [TestMethod]
        public void GetWebsitesTest()
        {
            var websites = AppPoolProvider.GetWebSites();
           Assert.AreEqual(websites != null && websites.Count>0, true);
        }


        [TestMethod]
        public void ControlAppPoolTest()
        {
            var poolsName = AppPoolProvider.GetAppPools();
            if (poolsName != null && poolsName.Count > 0)
            {
                string appPoolName = string.Empty;
                foreach (string name in poolsName.Keys)
                {
                    appPoolName = name;
                    break;
                }
                AppPoolProvider.ControlAppPool(appPoolName, CommandType.Recycle);
                Assert.AreEqual(GetStatus(appPoolName), "Running");
                AppPoolProvider.ControlAppPool(appPoolName, CommandType.Stop);
                Assert.AreEqual(GetStatus(appPoolName), "Stopped");
                AppPoolProvider.ControlAppPool(appPoolName, CommandType.Start);
                Assert.AreEqual(GetStatus(appPoolName), "Running");
            }
            else
            {
                Assert.Fail();
            }
        }

        private string GetStatus(string appPoolName)
        {
            string status = string.Empty;
            string appPoolPath = @"IIS://" + System.Environment.MachineName + "/W3SVC/AppPools/" + appPoolName;
            int intStatus = 0;
            try
            {
                DirectoryEntry w3svc = new DirectoryEntry(appPoolPath);
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
    }
}
