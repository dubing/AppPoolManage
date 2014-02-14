using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPoolManage.Web
{
    public class WebSitePro
    {
        public string SiteName { get; set; }
        public string SiteStatus { get; set; }
        public string PoolName { get; set; }
        public string PoolStatus { get; set; }
        public string FilePath { get; set; }
        public bool IsUmbraco { get; set; }

    }
}
