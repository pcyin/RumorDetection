using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using System.Data.SqlClient;
using System.Net;
using MySql.Data.MySqlClient;
using MySql.Data;

namespace WeiBoCrawler
{
    class WeiBoCommentCrawler
    {
        ConcurrentQueue<CommentCrawlJob> commentCrawQueue;
        CookieCollection cookies;
        Uri proxyUri;
        HttpRequest request;
        Random rnd = new Random();

        public WeiBoCommentCrawler(ConcurrentQueue<CommentCrawlJob> commentCrawQueue, CookieCollection cookies, Uri proxyUri)
        {
            this.commentCrawQueue = commentCrawQueue;
            this.cookies = cookies;
            this.proxyUri = proxyUri;
            request = new HttpRequest();
            request.Cookies = cookies;
            if (ConfigManager.IsUseProxy)
                request.ProxyAddress = proxyUri;
        }

        public void Run()
        {

            CommentCrawlJob job = null;
            while (true)
            {
                while (!commentCrawQueue.TryDequeue(out job))
                    ;

                if (job.Url == "$END$")
                    break;

                for (int pageId = job.BeginPage; pageId <= job.EndPage; pageId++)
                {
                    string pageUrl = job.Url + "?page=" + pageId + "&st=86e0";

                    string content = request.GetHttpResponseStr(pageUrl);
                    int errorNum = 0;
                    while (Ultility.Error(content))
                    {
                        errorNum++;
                        System.Threading.Thread.Sleep(30 * 1000 * errorNum);
                        content = request.GetHttpResponseStr(pageUrl);
                    }
                    if (Ultility.WeiBoNotExist(content))
                    {
                        continue;
                    }
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(content);  
                    var q = from node in doc.DocumentNode.SelectNodes("//div[@class='c']") where node.GetAttributeValue("id", "null").StartsWith("C_") select node;

                    foreach (HtmlNode node in q)
                    {
                        bool isTop = node.SelectSingleNode("./span[class='kt']") != null;
                        if (isTop && pageId > 1)
                            continue;

                        string uid = node.SelectSingleNode("./a").GetAttributeValue("href", "null");
                        uid = Ultility.parseUid(uid);
                        string comContent = node.SelectSingleNode("./span[@class=\"ctt\"]").InnerText;
                        WeiBoManager.AddComment(comContent);

                        Console.WriteLine(String.Format("Url:{0} PageId:{1}", job.Url, pageId));

                    }
                    System.Threading.Thread.Sleep(1000 + rnd.Next(1000));
                }

            }
        }

    }
}
