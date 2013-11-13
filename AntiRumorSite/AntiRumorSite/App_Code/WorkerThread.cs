using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Messaging;
using SVMWrapper;
using System.Text;
using System.Threading;
using System.IO;

namespace AntiRumorSite
{
    public class WorkerThread
    {
        //MessageQueue weibomsmq, weiboCallBackQueue, usermsmq, userCallBackQueue;
        //static Dictionary<string, int> province = new Dictionary<string, int>();

        //public WorkerThread()
        //{

        //}

        //private static void initProList()
        //{
        //    string strReadFilePath = Path.Combine(HttpRuntime.AppDomainAppPath,"province.txt");
        //    StreamReader sr = new StreamReader(strReadFilePath);
        //    int i = 0;
        //    while (!sr.EndOfStream)
        //    {
        //        province.Add(sr.ReadLine(), i);
        //        i++;
        //    }
        //}

        //private static string getLocVal(string str)
        //{
        //    if (str.Contains(" "))
        //        str = str.Substring(0, str.IndexOf(" "));
        //    return province[str].ToString();
        //}

        //public void Run()
        //{
        //    var svm = new SVM();
        //    InitQueue();
        //    initProList();

        //    while (true)
        //    {
        //        string url;
        //        while (!Global.Queue.TryDequeue(out url))
        //            Thread.Sleep(500);

        //        string[] temp = url.Split('/');
        //        Send(usermsmq, temp[3]);
        //        Send(weibomsmq, temp[3] + '|' + temp[4]);

        //        System.Messaging.Message wcmsg = weiboCallBackQueue.Receive();
        //        System.Messaging.Message ucmsg = userCallBackQueue.Receive();
        //        string weiboData = Convert.ToString(wcmsg.Body);
        //        string userData = Convert.ToString(ucmsg.Body);
        //        //MessageBox.Show(weiboData);
        //        //MessageBox.Show(userData);
        //        string[] weiboDataList = weiboData.Split('|');
        //        string[] userDataList = userData.Split('|');
        //        StringBuilder sb = new StringBuilder();
        //        sb.Append("1:").Append(weiboDataList[0]).Append(" ");
        //        sb.Append("2:").Append(weiboDataList[1]).Append(" ");
        //        sb.Append("3:").Append(weiboDataList[2]).Append(" ");
        //        sb.Append("4:").Append(userDataList[0]).Append(" ");
        //        sb.Append("5:").Append(userDataList[1]).Append(" ");
        //        sb.Append("6:").Append(userDataList[2]).Append(" ");
        //        sb.Append("7:").Append(getLocVal(userDataList[3])).Append(" ");
        //        sb.Append("8:").Append(userDataList[4]).Append(" ");
        //        sb.Append("9:").Append(userDataList[5]).Append(" ");
        //        sb.Append("10:").Append(weiboDataList[3]).Append(" ");

        //        svm.InputData(sb.ToString());
        //        var res = svm.Predict();

        //        Global.ResultMap[url] = res;
        //    }
        //}

        //void InitQueue()
        //{
        //    if (!MessageQueue.Exists(@".\private$\WeiBoCrawl"))
        //    {
        //        MessageQueue.Create(@".\private$\WeiBoCrawl");
        //    }
        //    weibomsmq = new MessageQueue(@".\private$\WeiBoCrawl");
        //    weibomsmq.Formatter = new BinaryMessageFormatter();
        //    if (!MessageQueue.Exists(@".\private$\WeiBoCrawlBack"))
        //    {
        //        MessageQueue.Create(@".\private$\WeiBoCrawlBack");
        //    }
        //    weiboCallBackQueue = new MessageQueue(@".\private$\WeiBoCrawlBack");
        //    weiboCallBackQueue.Formatter = new BinaryMessageFormatter();
        //    if (!MessageQueue.Exists(@".\private$\UserCrawl"))
        //    {
        //        MessageQueue.Create(@".\private$\UserCrawl");
        //    }
        //    usermsmq = new MessageQueue(@".\private$\UserCrawl");
        //    usermsmq.Formatter = new BinaryMessageFormatter();
        //    if (!MessageQueue.Exists(@".\private$\UserCrawlBack"))
        //    {
        //        MessageQueue.Create(@".\private$\UserCrawlBack");
        //    }
        //    userCallBackQueue = new MessageQueue(@".\private$\UserCrawlBack");
        //    userCallBackQueue.Formatter = new BinaryMessageFormatter();
        //}

        //public void Send(MessageQueue queue, string data)
        //{
        //    queue.Send(new System.Messaging.Message(data, new BinaryMessageFormatter()));
        //}
    }
}