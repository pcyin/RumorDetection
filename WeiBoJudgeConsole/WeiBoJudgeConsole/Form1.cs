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
            Send(usermsmq,temp[3]);
            Send(weibomsmq,temp[3] + '|' + temp[4]);

            System.Messaging.Message wcmsg = weiboCallBackQueue.Receive();
            System.Messaging.Message ucmsg = userCallBackQueue.Receive();
            string weiboData = Convert.ToString(wcmsg.Body);
            string userData = Convert.ToString(ucmsg.Body);
            //MessageBox.Show(weiboData);
            //MessageBox.Show(userData);
            string[] weiboDataList = weiboData.Split('|');
            string[] userDataList = userData.Split('|');
            StringBuilder sb = new StringBuilder();
            sb.Append("1:").Append(weiboDataList[0]).Append(" ");
            sb.Append("2:").Append(weiboDataList[1]).Append(" ");
            sb.Append("3:").Append(weiboDataList[2]).Append(" ");
            sb.Append("4:").Append(userDataList[0]).Append(" ");
            sb.Append("5:").Append(userDataList[1]).Append(" ");
            sb.Append("6:").Append(userDataList[2]).Append(" ");
            sb.Append("7:").Append(getLocVal(userDataList[3])).Append(" ");
            sb.Append("8:").Append(userDataList[4]).Append(" ");
            sb.Append("9:").Append(userDataList[5]).Append(" ");
            sb.Append("10:").Append(weiboDataList[3]).Append(" ");
            //MessageBox.Show(sb.ToString());
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
    }
}
