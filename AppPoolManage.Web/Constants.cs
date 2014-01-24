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


    }

    public enum eStates
    {
        Start = 2,
        Stop = 4,
        Pause = 6,
    }
}
