using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Browser
{
    [ServiceContract]
    public interface IUserInfoService
    {
        [OperationContract]
        UserInfo GetUserInfo(string name);
    }

    [DataContract]
    public class UserInfo
    {
        [DataMember]
        public string Uid { get; set; }
        [DataMember]
        public int Level { get; set; }
        [DataMember]
        public bool IsVerified { get; set; }
        [DataMember]
        public int Credit { get; set; }
        [DataMember]
        public int FollowNum { get; set; }
        [DataMember]
        public int FanNum { get; set; }
        [DataMember]
        public int WeiboNum { get; set; }
        [DataMember]
        public string NickName { get; set; }
        [DataMember]
        public string Location { get; set; }
        [DataMember]
        public bool Sex { get; set; }
        [DataMember]
        public string Intro { get; set; }

        public string ToString()
        {
            return String.Format(
                "Uid:{0}", Uid
                );
        }
    }

    public class UserInfoService : IUserInfoService
    {
        WebBrowser browser;
        RichTextBox tbLog;

        public UserInfoService(WebBrowser browser, RichTextBox tbLog)
        {
            this.browser = browser;
            this.tbLog = tbLog;
        }

        public UserInfo GetUserInfo(string name)
        {
            browser.Navigate("http://weibo.com/" + name + "/info");

            Thread.Sleep(2000);
            UserInfo userInfo = new UserInfo();

            while (!(bool)
                browser.Invoke((Func<bool>)(() =>
                {
                    return getUserInfo(ref userInfo);
                }))
            )
            {
                tbLog.Invoke((Action)(() =>
                {
                    tbLog.Text += "Try again" + "\r\n";
                }));
                Thread.Sleep(2000);
            }
            cleanMemory();

            return userInfo;
        }

        bool getUserInfo(ref UserInfo userInfo)
        {
            var doc = browser.Document;
            if (doc.Body == null)
                return false;

            if (doc.Body.InnerHtml == null || doc.Body.InnerHtml.Contains("你访问的页面地址有误，或者该页面不存在") || doc.Body.InnerHtml.Contains("您当前访问的帐号异常"))
            {
                return false;
            }
            var levelNode = (from HtmlElement el in doc.GetElementsByTagName("span") where el.GetAttribute("className").Contains("W_level_ico") select el);
            var userNickNameTest = (from HtmlElement el in doc.GetElementsByTagName("div") where el.GetAttribute("className") == "label S_txt2" && el.InnerText == "昵称" select el);
            if (levelNode.Count() == 0 || userNickNameTest.Count() == 0)
            {
                return false;
            }
            int level = Convert.ToInt32(
                levelNode.First().Children[0].GetAttribute("title").Substring("当前等级：".Length)
            );

            var uidNode = (from HtmlElement el in doc.GetElementsByTagName("a") where el.GetAttribute("suda-data") == "key=tblog_grade_float&value=grade_icon_click" select el).First();
            string uid = uidNode.GetAttribute("href").Substring("http://level.account.weibo.com/u/?id=".Length, 10);

            int credit = 0;

            try
            {
                var creditList = (from HtmlElement node in doc.GetElementsByTagName("table") where node.GetAttribute("node-type") == "credit" select node).Single().GetElementsByTagName("tr");
                foreach (HtmlElement rec in creditList)
                {
                    DateTime time = Convert.ToDateTime(rec.Children[0].InnerText);
                    string reason = rec.Children[1].InnerText;
                    int cur_credit = Convert.ToInt32(
                            rec.Children[2].InnerText.Substring(
                                0, rec.Children[2].InnerText.Length - 1
                            )
                        );
                    if (cur_credit < 0)
                        credit += cur_credit;
                }
            }
            catch (Exception ex)
            {

            }


            var followNode = (from HtmlElement el in doc.GetElementsByTagName("strong") where el.GetAttribute("node-type") == "follow" select el).First();
            int follow = Convert.ToInt32(
                    followNode.InnerText
                );

            var fansNode = (from HtmlElement el in doc.GetElementsByTagName("strong") where el.GetAttribute("node-type") == "fans" select el).First();
            int fans = Convert.ToInt32(
                               fansNode.InnerText
                           );

            var weiboNode = (from HtmlElement el in doc.GetElementsByTagName("strong") where el.GetAttribute("node-type") == "weibo" select el).First();
            int weibo = Convert.ToInt32(
                               weiboNode.InnerText
                           );

            bool verified = (from HtmlElement el in doc.GetElementsByTagName("div")
                             where el.GetAttribute("className") == "icon_bed"
                                 && el.Children[0].GetAttribute("href") == "http://verified.weibo.com/verify"
                             select el
                             ).Count() == 1;

            string headPicUrl = (from HtmlElement el in doc.GetElementsByTagName("div") where el.GetAttribute("className") == "pf_head_pic" select el).
                First().
                Children[0].GetAttribute("src");

            string userNickName=null, userLoc = null, userIntro=null;
            bool userSex=true;

            try
            {
                userNickName = (from HtmlElement el in doc.GetElementsByTagName("div") where el.GetAttribute("className") == "label S_txt2" && el.InnerText == "昵称" select el).First().NextSibling.InnerText;
            }
            catch (Exception ex)
            {
            }
            try
            {
                userLoc = (from HtmlElement el in doc.GetElementsByTagName("div") where el.GetAttribute("className") == "label S_txt2" && el.InnerText == "所在地" select el).First().NextSibling.InnerText;
            }
            catch (Exception ex)
            {
            }
            try
            {
                userSex = (from HtmlElement el in doc.GetElementsByTagName("div") where el.GetAttribute("className") == "label S_txt2" && el.InnerText == "性别" select el).First().NextSibling.InnerText == "男";
            }
            catch (Exception ex)
            {
            }
            try
            {
                userIntro = (from HtmlElement el in doc.GetElementsByTagName("div") where el.GetAttribute("className") == "label S_txt2" && el.InnerText == "简介" select el).First().NextSibling.InnerText;
            }
            catch (Exception ex)
            {
            }

            //string data = String.Format("{0}|{1}|{2}|{3}|{4}|{5}", fans, weibo, credit, userLoc, level, verified ? 1 : 0);

            userInfo.Uid = uid;
            userInfo.Level = level;
            userInfo.Intro = userIntro;
            userInfo.IsVerified = verified;
            userInfo.Location = userLoc;
            userInfo.NickName = userNickName;
            userInfo.Sex = userSex;
            userInfo.WeiboNum = weibo;
            userInfo.FanNum = fans;
            userInfo.FollowNum = follow;
            userInfo.Credit = credit;

            string log = userInfo.ToString();

            tbLog.Invoke((Action)(() =>
            {
                tbLog.Text += log + "\n";
            }));

            return true;
        }

        void cleanMemory()
        {
            IntPtr pHandle = GetCurrentProcess();
            SetProcessWorkingSetSize(pHandle, -1, -1);
        }

        [DllImport("KERNEL32.DLL", EntryPoint = "SetProcessWorkingSetSize", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool SetProcessWorkingSetSize(IntPtr pProcess, int dwMinimumWorkingSetSize, int dwMaximumWorkingSetSize);

        [DllImport("KERNEL32.DLL", EntryPoint = "GetCurrentProcess", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern IntPtr GetCurrentProcess();
    }
}
