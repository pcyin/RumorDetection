using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;


namespace AntiRumorSite
{
    /// <summary>
    /// Summary description for Detect
    /// </summary>
    public class Detect : IHttpHandler
    {
        
        public void ProcessRequest(HttpContext context)
        {
            var url = context.Request["url"];
            context.Response.ContentType = "text/plain";

            if (Global.ResultMap.ContainsKey(url))
                context.Response.Write("201");
            else
            {
                context.Response.Write("200");
                Global.Queue.Enqueue(url);
            }
                
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}