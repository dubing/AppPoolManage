using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.DirectoryServices;
using System.Collections.Generic;

namespace AppPoolManage.Web.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var poolsName = AppPoolCore.GetAppPools();
            Assert.AreEqual(poolsName != null && poolsName.ContainsKey("phase3"), true);
        }

        [TestMethod]
        public void GetVersion()
        {
            var iisVersion = AppPoolCore.GetIISVersion();
            Assert.AreEqual(iisVersion, "7");
        }

        [TestMethod]
        public void GetWebsite()
        {
            var websites = AppPoolCore.GetWebsites();
            Assert.AreEqual(websites != null && websites.ContainsKey("phase3"), true);
        }

        [TestMethod]
        public void test()
        {
            List<string> siteNames = new List<string>();

            var root = new DirectoryEntry("IIS://localhost/W3SVC");
            Console.WriteLine(root.Children);

            // DirectoryEntry root = test("IIS://localhost/W3SVC");
            //foreach (DirectoryEntry e in root.Children)
            //{
            //    if (e.SchemaClassName == "IIsWebServer")
            //    {
            //        siteNames.Add(e.Properties["ServerComment"].Value.ToString());
            //    }
            //}

        }


        [TestMethod]
        public void RecycleAppPool()
        {
            string appPoolName = "phase3";
            AppPoolCore.ControlAppPool(appPoolName, "Recycle");
            Assert.AreEqual(GetStatus(appPoolName), "Running");
            AppPoolCore.ControlAppPool(appPoolName, "Stop");
            Assert.AreEqual(GetStatus(appPoolName), "Stopped");
            AppPoolCore.ControlAppPool(appPoolName, "Start");
            Assert.AreEqual(GetStatus(appPoolName), "Running");


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
