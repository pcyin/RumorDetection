using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Threading;
using AntiRumorSite;
using SVMWrapper;

namespace AntiRumorSite
{
    public class Global : System.Web.HttpApplication
    {
        public static Dictionary<string, int> ResultMap = new Dictionary<string, int>();
        public static ConcurrentQueue<string> Queue = new ConcurrentQueue<string>();

        protected void Application_Start(object sender, EventArgs e)
        {
            //new Thread(() => {
            //    new WorkerThread().Run();
            //}).Start();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}