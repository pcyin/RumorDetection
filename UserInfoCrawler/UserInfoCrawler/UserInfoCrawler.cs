using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections.Concurrent;
using System.Data.SqlClient;
using System.Threading;

namespace Browser
{
    
    class UserInfoCrawler
    {
        WebBrowser browser;
        ConcurrentQueue<string> uidQueue;
        bool loadComplete = false;
        public UserInfoCrawler(ConcurrentQueue<string> uidQueue, WebBrowser browser)
        {
            this.uidQueue = uidQueue;
            this.browser = browser;//new WebBrowser();
        }

        void onDocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            
            HtmlDocument doc = ((WebBrowser)sender).Document;
            List<SqlParameter> paraList = new List<SqlParameter>();

            var levelNode = (from HtmlElement el in doc.GetElementsByTagName("span") where el.GetAttribute("className").Contains("W_level_ico") select el);
            var userNickNameTest = (from HtmlElement el in doc.GetElementsByTagName("div") where el.GetAttribute("className") == "label S_txt2" && el.InnerText == "昵称" select el);
            if (levelNode.Count() == 0 || userNickNameTest.Count()==0)
            {
                browser.Navigate(e.Url);
                System.Threading.Thread.Sleep(4000);
                return;
            }
            int level = Convert.ToInt32(
                levelNode.First().Children[0].GetAttribute("title").Substring("当前等级：".Length)
            );
            paraList.Add(new SqlParameter("@level", level));
            
            var uidNode = (from HtmlElement el in doc.GetElementsByTagName("a") where el.GetAttribute("node-type") == "profileNavLink" select el).First();
            string uid = uidNode.GetAttribute("href").Substring("http://weibo.com/".Length, 10);

            string uidCheckSql = "SELECT COUNT(*) FROM [User] WHERE uid = @uid";
            /*int userCount = (int)SqlHelper.ExecuteScalar(uidCheckSql, new SqlParameter[] { new SqlParameter("@uid", uid) });
            if (userCount > 0)
            {
                return;
            }*/

            paraList.Add(new SqlParameter("@uid", uid));
            try
            {
                var creditList = (from HtmlElement node in doc.GetElementsByTagName("table") where node.GetAttribute("node-type") == "credit" select node).Single().GetElementsByTagName("tr");
                string sql = "INSERT INTO [UserCredit](uid,credit,reason,time) VALUES(@uid,@credit,@reason,@time)";
                foreach (HtmlElement rec in creditList)
                {
                    DateTime time = Convert.ToDateTime(rec.Children[0].InnerText);
                    string reason = rec.Children[1].InnerText;
                    int credit = Convert.ToInt32(
                            rec.Children[2].InnerText.Substring(
                                0, rec.Children[2].InnerText.Length - 1
                            )
                        );
                    SqlHelper.ExecuteNonQuery(sql,new SqlParameter[]{
                        new SqlParameter("@uid",uid),
                        new SqlParameter("@credit",credit),
                        new SqlParameter("@reason",reason),
                        new SqlParameter("@time",time)
                    });
                }
            }
            catch (Exception ex)
            {
                
            }

            
            var followNode = (from HtmlElement el in doc.GetElementsByTagName("strong") where el.GetAttribute("node-type") == "follow" select el).First();
            int follow = Convert.ToInt32(
                    followNode.InnerText
                );
            paraList.Add(new SqlParameter("@follow", follow));

            var fansNode = (from HtmlElement el in doc.GetElementsByTagName("strong") where el.GetAttribute("node-type") == "fans" select el).First();
            int fans = Convert.ToInt32(
                               fansNode.InnerText
                           );
            paraList.Add(new SqlParameter("@fans", fans));

            var weiboNode = (from HtmlElement el in doc.GetElementsByTagName("strong") where el.GetAttribute("node-type") == "weibo" select el).First();
            int weibo = Convert.ToInt32(
                               weiboNode.InnerText
                           );
            paraList.Add(new SqlParameter("@weibo", weibo));

            bool verified = (from HtmlElement el in doc.GetElementsByTagName("div")
                             where el.GetAttribute("className") == "icon_bed"
                                 && el.Children[0].GetAttribute("href") == "http://verified.weibo.com/verify"
                             select el
                             ).Count() == 1;
            paraList.Add(new SqlParameter("@verified", verified));

            string headPicUrl = (from HtmlElement el in doc.GetElementsByTagName("div") where el.GetAttribute("className") == "pf_head_pic" select el).
                First().
                Children[0].GetAttribute("src");
            paraList.Add(new SqlParameter("@head_pic_url", headPicUrl));

            string userNickName, userLoc, userIntro;
            bool userSex;
            
            try
            {
                userNickName = (from HtmlElement el in doc.GetElementsByTagName("div") where el.GetAttribute("className") == "label S_txt2" && el.InnerText == "昵称" select el).First().NextSibling.InnerText;
                paraList.Add(new SqlParameter("@nick_name",userNickName));
            }
            catch (Exception ex) {
                paraList.Add(new SqlParameter("@nick_name", DBNull.Value));
            }
            try
            {
                userLoc = (from HtmlElement el in doc.GetElementsByTagName("div") where el.GetAttribute("className") == "label S_txt2" && el.InnerText == "所在地" select el).First().NextSibling.InnerText;
                paraList.Add(new SqlParameter("@loc", userLoc));
            }
            catch (Exception ex) {
                paraList.Add(new SqlParameter("@loc", DBNull.Value));
            }
            try
            {
                userSex = (from HtmlElement el in doc.GetElementsByTagName("div") where el.GetAttribute("className") == "label S_txt2" && el.InnerText == "性别" select el).First().NextSibling.InnerText == "男";
                paraList.Add(new SqlParameter("@sex", userSex));
            }
            catch (Exception ex) {
                paraList.Add(new SqlParameter("@sex", DBNull.Value));
            }
            try
            {
                userIntro = (from HtmlElement el in doc.GetElementsByTagName("div") where el.GetAttribute("className") == "label S_txt2" && el.InnerText == "简介" select el).First().NextSibling.InnerText;
                if(String.IsNullOrEmpty(userIntro))
                    paraList.Add(new SqlParameter("@intro", DBNull.Value));
                else
                    paraList.Add(new SqlParameter("@intro", userIntro));
            }
            catch (Exception ex) {
                paraList.Add(new SqlParameter("@intro", DBNull.Value));
            }
            string insertSql = "INSERT INTO [User](uid,follow,fans,weibo,verified,head_pic_url,nick_name,loc,sex,intro,level) VALUES(@uid,@follow,@fans,@weibo,@verified,@head_pic_url,@nick_name,@loc,@sex,@intro,@level)";
            SqlHelper.ExecuteNonQuery(insertSql, paraList.ToArray());

            if (!nextInteration())
                return;
        }

        bool nextInteration(){
            string userName = null;
            while (!uidQueue.TryDequeue(out userName))
                ;
            if (userName == "exit")
                return false;
            Thread.Sleep(5 * 1000);
            browser.Navigate("http://weibo.com/" + userName + "/info");
            return true;
        }

        public void Run() {
            nextInteration();
        }
    }
}
