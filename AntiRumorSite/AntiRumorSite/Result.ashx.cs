using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AntiRumorSite
{
    /// <summary>
    /// Summary description for Result
    /// </summary>
    public class Result : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var url = context.Request["url"];
            context.Response.ContentType = "text/plain";

            if (Global.ResultMap.ContainsKey(url))
            {
                context.Response.Write(Global.ResultMap[url]);
            }
            else
            {
                context.Response.Write(201);
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