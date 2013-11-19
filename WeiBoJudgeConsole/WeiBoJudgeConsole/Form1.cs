using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Messaging;
using System.IO;
using NewsWeiboSim;


namespace WeiBoJudgeConsole
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            initProList();
        }

        static Dictionary<string, int> province = new Dictionary<string, int>();
        NewsWeiboSimlarity sim;

        private static void initProList()
        {
            string strReadFilePath = @"../../pro.txt";
            StreamReader sr = new StreamReader(strReadFilePath);
            int i = 0;
            while (!sr.EndOfStream)
            {
                province.Add(sr.ReadLine(), i);
                i++;
            }
        }

        private static string getLocVal(string str)
        {
            if (str.Contains(" "))
                str = str.Substring(0, str.IndexOf(" "));
            return province[str].ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] temp = textBox1.Text.Split('/');

            ContentCrawlServiceClient contentClient = new ContentCrawlServiceClient();
            UserInfoServiceClient infoClient = new UserInfoServiceClient();
            var contentInfo = contentClient.GetContentCrawlResult(temp[3] + '|' + temp[4]);
            var userInfo = infoClient.GetUserInfo(temp[3]);

            StringBuilder sb = new StringBuilder();
            sb.Append("1:").Append(contentInfo.Sentiment).Append(" ");   //sentiment
            sb.Append("2:").Append(contentInfo.HasImg ? "1" : "0").Append(" ");   //HasImg ? "1" : "0"
            sb.Append("3:").Append(contentInfo.HasUrl ? "1" : "0").Append(" ");   //HasUrl ? "1" : "0"
            sb.Append("4:").Append(userInfo.FanNum).Append(" ");    //fans
            sb.Append("5:").Append(userInfo.WeiboNum).Append(" ");    //weibo
            sb.Append("6:").Append(userInfo.Credit).Append(" ");    //credit
            sb.Append("7:").Append(getLocVal(userInfo.Location)).Append(" "); //userLoc
            sb.Append("8:").Append(userInfo.Level).Append(" ");    //level
            sb.Append("9:").Append(userInfo.IsVerified).Append(" ");    //verified
            sb.Append("10:").Append(contentInfo.CommentEval).Append(" ");  //comval
            vector.Text = sb.ToString();
            /*List<string> newsList = sim.BiggestFiveTitle(weiboDataList[4]);
            foreach(string str in newsList)
            {
                news.Text += str + '\n';
            }*/
        }

        public void Send(MessageQueue queue,string data)
        {
            queue.Send(new System.Messaging.Message(data,new BinaryMessageFormatter()));
           // queue.Send(data);
        }

        MessageQueue weibomsmq, weiboCallBackQueue, usermsmq, userCallBackQueue;

        void InitQueue()
        {
            if (!MessageQueue.Exists(@".\private$\WeiBoCrawl"))
            {
                MessageQueue.Create(@".\private$\WeiBoCrawl");
            }
            weibomsmq = new MessageQueue(@".\private$\WeiBoCrawl");
            weibomsmq.Formatter = new BinaryMessageFormatter();
            if (!MessageQueue.Exists(@".\private$\WeiBoCrawlBack"))
            {
                MessageQueue.Create(@".\private$\WeiBoCrawlBack");
            }
            weiboCallBackQueue = new MessageQueue(@".\private$\WeiBoCrawlBack");
            weiboCallBackQueue.Formatter = new BinaryMessageFormatter();
            if (!MessageQueue.Exists(@".\private$\UserCrawl"))
            {
                MessageQueue.Create(@".\private$\UserCrawl");
            }
            usermsmq = new MessageQueue(@".\private$\UserCrawl");
            usermsmq.Formatter = new BinaryMessageFormatter();
            if (!MessageQueue.Exists(@".\private$\UserCrawlBack"))
            {
                MessageQueue.Create(@".\private$\UserCrawlBack");
            }
            userCallBackQueue = new MessageQueue(@".\private$\UserCrawlBack");
            userCallBackQueue.Formatter = new BinaryMessageFormatter();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitQueue();
            //sim = new NewsWeiboSimlarity();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
