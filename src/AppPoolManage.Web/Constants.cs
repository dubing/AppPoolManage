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
        public static string Success = "success";

        

    }

    public enum CommandType
    {
        Recycle,
        Start,
        Stop
    }

    public enum DeleteStates
    {
        Successs = 1,
        NotExist = 2,
        Error = 3
    }

    public enum PoolStates
    {
        Running = 2,
        Stopped = 4,
        Paused = 6,
        Unknown = 9
    }

    public enum SiteStates
    {
        Running =2,
        Stopped =4,
        Paused =6,
        NotExist=0,
        Unknown =9
    }
}
