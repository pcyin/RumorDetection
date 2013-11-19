using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using WebServiceTest;

namespace WeiBoCrawler
{
    class WeiBoManager
    {
        public static List<string> commentList = new List<string>();
        public static string body;
        public static bool HasImg;
        public static bool HasUrl;
        public static object lockObj = new object();

        static List<string> keywords = new List<string>();
        static bool judge(string str)
        {
            foreach (string keyword in keywords)
            {
                if (str.Contains(keyword))
                    return true;
            }
            return false;
        }

        public static void Init()
        {
            keywords.AddRange(new string[] { "扯淡", "钓鱼", "编的", "瞎编", "假消息", "传谣", "谣言", "调侃", "有点黑", "假的", "三人成虎", "骗人", "造谣", "不实", "谣传", "假新闻", "不靠谱", "太假", "智商拙计", "八卦", "真的假", "求真相", "全家火葬场", "真假", "求辟谣", "求真相" });
            keywords.AddRange(new string[] { "没有", "哪有", "哪里有", "不可能", "没有" });
        }

        public static void Clear()
        {
            commentList.Clear();
        }

        public static void AddBody(string _body)
        {
            body = _body;
        }

        public static void AddComment(string str)
        {
            lock (lockObj)
            {
                commentList.Add(str);
            }
        }

        public static double CalcComment()
        {
            int count = 0;

            foreach (string c in commentList)
            {
                if (judge(c))
                    count++;
            }

            return (double)count / (double)commentList.Count;
        }

        public static int CalcSent()
        {
            body = SenRequest.GetHttpResponseStr("http://218.241.236.92:8080/clean/" + System.Web.HttpUtility.UrlEncode(body));
            return Convert.ToInt32(SenRequest.GetHttpResponseStr("http://218.241.236.92:8080/sentiment/" + System.Web.HttpUtility.UrlEncode(body)));
        }

        public static string GetContent() 
        {
            return body;
        }
    }
}
