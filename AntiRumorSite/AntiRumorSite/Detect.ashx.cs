using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using Browser;
using WeiBoCrawler;
using SVMWrapper;
using System.Text;

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

            UserInfo uinfo = new UserInfo();
            ContentCrawlResult cinfo = new ContentCrawlResult();

            ServiceHelper.GetServiceInfo(url, out cinfo, out uinfo);
            var vec = ServiceHelper.GetFeatureVector(cinfo, uinfo);
            SVM.InputData(vec);
            var result = SVM.Predict();

            var data = String.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|{13}|{14}|{15}|{16}|{17}",
                uinfo.Uid,
                uinfo.Uid,
                uinfo.Sex,
                uinfo.Level,
                uinfo.IsVerified,
                uinfo.Credit,
                uinfo.FollowNum,
                uinfo.FanNum,
                uinfo.WeiboNum,
                uinfo.NickName,
                uinfo.Location,
                uinfo.Intro,
                cinfo.Content,
                cinfo.CommentEval,
                cinfo.HasImg,
                cinfo.Sentiment,
                cinfo.HasUrl,
                result);

            context.Response.Write(data);
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