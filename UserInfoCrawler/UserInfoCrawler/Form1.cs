using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Threading;
using System.Collections.Concurrent;
using System.Data.SqlClient;
using System.IO;
using System.Messaging;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Browser
{
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public partial class Form1 : Form
    {
        ConcurrentQueue<string> uidQueue;
        Thread listeningThread;
        ServiceHost host;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitQueue();
            browser.Navigate("http://www.weibo.com");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            uidQueue = new ConcurrentQueue<string>();

            listeningThread = new Thread(() =>
            {
                Uri baseAddress = new Uri("http://localhost:6526/UserInfoService");

                UserInfoService service = new UserInfoService(browser, tbLog);

                host = new ServiceHost(service, baseAddress);

                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
                host.Description.Behaviors.Add(smb);
                var behavior = host.Description.Behaviors.Find<ServiceBehaviorAttribute>();
                behavior.InstanceContextMode = InstanceContextMode.Single;

                try
                {
                    host.Open();
                }
                catch (ThreadAbortException ex)
                {
                    tbLog.Invoke((Action)(() =>
                    {
                        tbLog.Text += ex.Message + "\n";
                    }));
                }                    
                

            });

            listeningThread.Start();
            button1.Enabled = false;
            button2.Enabled = true;
        } 

        bool getUserInfo(WebBrowser browser)
        {
            var doc = browser.Document;
            if (doc.Body == null)
                return false;

            List<SqlParameter> paraList = new List<SqlParameter>();
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
            paraList.Add(new SqlParameter("@level", level));

            var uidNode = (from HtmlElement el in doc.GetElementsByTagName("a") where el.GetAttribute("suda-data") == "key=tblog_grade_float&value=grade_icon_click" select el).First();
            string uid = uidNode.GetAttribute("href").Substring("http://level.account.weibo.com/u/?id=".Length, 10);

            /*string uidCheckSql = "SELECT COUNT(*) FROM [User] WHERE uid = @uid";
            int userCount = (int)SqlHelper.ExecuteScalar(uidCheckSql, new SqlParameter[] { new SqlParameter("@uid", uid) });
            if (userCount > 0)
            {
                nextInteration();
                return;
            }*/

            int credit = 0;

            paraList.Add(new SqlParameter("@uid", uid));
            try
            {
                var creditList = (from HtmlElement node in doc.GetElementsByTagName("table") where node.GetAttribute("node-type") == "credit" select node).Single().GetElementsByTagName("tr");
                //string sql = "INSERT INTO [UserCredit](uid,credit,reason,time) VALUES(@uid,@credit,@reason,@time)";
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
                    /*SqlHelper.ExecuteNonQuery(sql, new SqlParameter[]{
                        new SqlParameter("@uid",uid),
                        new SqlParameter("@credit",credit),
                        new SqlParameter("@reason",reason),
                        new SqlParameter("@time",time)
                    });*/
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

            string userNickName, userLoc = "", userIntro;
            bool userSex;

            try
            {
                userNickName = (from HtmlElement el in doc.GetElementsByTagName("div") where el.GetAttribute("className") == "label S_txt2" && el.InnerText == "昵称" select el).First().NextSibling.InnerText;
                paraList.Add(new SqlParameter("@nick_name", userNickName));
            }
            catch (Exception ex)
            {
                paraList.Add(new SqlParameter("@nick_name", DBNull.Value));
            }
            try
            {
                userLoc = (from HtmlElement el in doc.GetElementsByTagName("div") where el.GetAttribute("className") == "label S_txt2" && el.InnerText == "所在地" select el).First().NextSibling.InnerText;
                paraList.Add(new SqlParameter("@loc", userLoc));
            }
            catch (Exception ex)
            {
                paraList.Add(new SqlParameter("@loc", DBNull.Value));
            }
            try
            {
                userSex = (from HtmlElement el in doc.GetElementsByTagName("div") where el.GetAttribute("className") == "label S_txt2" && el.InnerText == "性别" select el).First().NextSibling.InnerText == "男";
                paraList.Add(new SqlParameter("@sex", userSex));
            }
            catch (Exception ex)
            {
                paraList.Add(new SqlParameter("@sex", DBNull.Value));
            }
            try
            {
                userIntro = (from HtmlElement el in doc.GetElementsByTagName("div") where el.GetAttribute("className") == "label S_txt2" && el.InnerText == "简介" select el).First().NextSibling.InnerText;
                if (String.IsNullOrEmpty(userIntro))
                    paraList.Add(new SqlParameter("@intro", DBNull.Value));
                else
                    paraList.Add(new SqlParameter("@intro", userIntro));
            }
            catch (Exception ex)
            {
                paraList.Add(new SqlParameter("@intro", DBNull.Value));
            }
            string insertSql = "INSERT INTO [User](uid,follow,fans,weibo,verified,head_pic_url,nick_name,loc,sex,intro,level) VALUES(@uid,@follow,@fans,@weibo,@verified,@head_pic_url,@nick_name,@loc,@sex,@intro,@level)";
            //SqlHelper.ExecuteNonQuery(insertSql, paraList.ToArray());

            string data = String.Format("{0}|{1}|{2}|{3}|{4}|{5}", fans, weibo, credit, userLoc, level, verified ? 1 : 0);

            Send(data);

            tbLog.Invoke((Action)(() => {
                tbLog.Text += data + "\n";
            }));

            return true;
        }

        int count = 0;
        string userName;
        MessageQueue msmq;
        MessageQueue callBackQueue;

        public void Send(string data)
        {
            callBackQueue.Send(new System.Messaging.Message(data, new BinaryMessageFormatter()));
        }

        void InitQueue()
        {
            if (!MessageQueue.Exists(@".\private$\UserCrawl"))
            {
                MessageQueue.Create(@".\private$\UserCrawl");

            }
            msmq = new MessageQueue(@".\private$\UserCrawl");
            msmq.Formatter = new BinaryMessageFormatter();
            if (!MessageQueue.Exists(@".\private$\UserCrawlBack"))
            {
                MessageQueue.Create(@".\private$\UserCrawlBack");

            }
            callBackQueue = new MessageQueue(@".\private$\UserCrawlBack");
            callBackQueue.Formatter = new BinaryMessageFormatter();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listeningThread.Abort();
            button1.Enabled = true;
            button2.Enabled = false;

            try
            {
                host.Close();
            }
            catch (System.Exception ex)
            {
                tbLog.Invoke((Action)(() =>
                {
                    tbLog.Text += ex.Message + "\n";
                }));
            }
        }

        private void OnExiting(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }
    }


}
