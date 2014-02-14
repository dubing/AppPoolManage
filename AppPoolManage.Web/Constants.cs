using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPoolManage.Web
{
    public static class Constants
    {
        public static string AddressHeader = "IIS://" + System.Environment.MachineName + "/W3SVC";
        public static string Username = null;
        public static string Pwd = null;
        public static string INFO = "/INFO";
        public static string MajorIISVersionNumber = "MajorIISVersionNumber";
        public static string IIsWebServer = "IIsWebServer";
        public static string ServerComment = "ServerComment";
        public static string ServerState = "ServerState";
        public static string IIsWebVirtualDir = "IIsWebVirtualDir";
        public static string AppPoolId = "apppoolid";
        public static string Path = "Path";
        public static string AppPools = "/AppPools";
        public static string App_Data = "\\App_Data\\";
        public static string UmbracoDirectory = "\\Umbraco\\";
        public static string UmbracoConfig = "umbraco.config";
        public static string AppPoolState = "AppPoolState";

        

    }

    public enum EStates
    {
        Start = 2,
        Stop = 4,
        Pause = 6,
    }

    public enum DeleteStates
    {
        Successs = 1,
        NotExist = 2,
        Error = 3,
    }

    public enum SiteStates
    {
        Running,
        Stopped,
        Paused,
        NotExist,
        Unknown
    }
}
