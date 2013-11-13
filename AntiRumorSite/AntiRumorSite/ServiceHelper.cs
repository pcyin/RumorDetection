using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using WeiBoCrawler;
using Browser;

namespace AntiRumorSite
{
    public class ServiceHelper
    {
        static Dictionary<string, int> province = new Dictionary<string, int>();

        static ServiceHelper()
        {
            initProList();
        }

        private static void initProList()
        {
            string strReadFilePath = Path.Combine(HttpRuntime.AppDomainAppPath,"province.txt");
            StreamReader sr = new StreamReader(strReadFilePath);
            int i = 0;
            while (!sr.EndOfStream)
            {
                province.Add(sr.ReadLine(), i);
                i++;
            }
        }

        private static string GetProvinceId(string str)
        {
            if (str.Contains(" "))
                str = str.Substring(0, str.IndexOf(" "));
            return province[str].ToString();
        }

        public static string GetFeatureVector(ContentCrawlResult contentInfo, UserInfo userInfo)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("1:").Append(contentInfo.Sentiment).Append(" ");   //sentiment
            sb.Append("2:").Append(contentInfo.HasImg ? "1" : "0").Append(" ");   //HasImg ? "1" : "0"
            sb.Append("3:").Append(contentInfo.HasUrl ? "1" : "0").Append(" ");   //HasUrl ? "1" : "0"
            sb.Append("4:").Append(userInfo.FanNum).Append(" ");    //fans
            sb.Append("5:").Append(userInfo.WeiboNum).Append(" ");    //weibo
            sb.Append("6:").Append(userInfo.Credit).Append(" ");    //credit
            sb.Append("7:").Append(GetProvinceId(userInfo.Location)).Append(" "); //userLoc
            sb.Append("8:").Append(userInfo.Level).Append(" ");    //level
            sb.Append("9:").Append(userInfo.IsVerified).Append(" ");    //verified
            sb.Append("10:").Append(contentInfo.CommentEval).Append(" ");  //comval 
            return sb.ToString();
        }

        public static void GetServiceInfo(string url, out ContentCrawlResult contentInfo, out UserInfo userInfo)
        {
            string[] temp = url.Split('/');

            ContentCrawlServiceClient contentClient = new ContentCrawlServiceClient();
            UserInfoServiceClient infoClient = new UserInfoServiceClient();
            contentInfo = contentClient.GetContentCrawlResult(temp[3] + '|' + temp[4]);
            userInfo = infoClient.GetUserInfo(temp[3]);
        }
    }
}