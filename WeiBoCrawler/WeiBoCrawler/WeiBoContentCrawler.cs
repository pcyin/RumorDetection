using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using System.Data.SqlClient;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Net;
using System.Threading;

namespace WeiBoCrawler
{
    class WeiBoContentCrawler
    {
        ConcurrentQueue<string> urlQueue;
        ConcurrentQueue<CommentCrawlJob> commentCrawQueue;
        CookieCollection cookies;
        Uri proxyUri;
        HttpRequest request;
        Random rnd = new Random();

        public WeiBoContentCrawler(ConcurrentQueue<string> urlQueue, ConcurrentQueue<CommentCrawlJob> commentCrawQueue, CookieCollection cookies, Uri proxyUri)
        {
            this.urlQueue = urlQueue;
            this.commentCrawQueue = commentCrawQueue;
            this.cookies = cookies;
            this.proxyUri = proxyUri;
            request = new HttpRequest();
            request.Cookies = cookies;
            if (ConfigManager.IsUseProxy)
                request.ProxyAddress = proxyUri;
        }

        public void Run(string pageUrl)
        {
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
                return;
            }
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(content);
            try
            {
                HtmlNode contentNode = doc.GetElementbyId("M_");
                string weiboContent = contentNode.SelectSingleNode("//span[@class=\"ctt\"]").InnerText;
                var q = from node in contentNode.SelectNodes("//a") where node.InnerText == "原图" select node;

                bool hasImg = false;
                if (q.Count() > 0)
                    hasImg = true;

                bool hasUrl = weiboContent.Contains("http");

                WeiBoManager.HasImg = hasImg;
                WeiBoManager.HasUrl = hasUrl;
                WeiBoManager.AddBody(weiboContent);

                int comPageNum = doc.DocumentNode.SelectSingleNode("//input[@name=\"mp\"]") == null ? 1 : doc.DocumentNode.SelectSingleNode("//input[@name=\"mp\"]").GetAttributeValue("value", 1);

                //crawl the comments in the first page
                q = from node in doc.DocumentNode.SelectNodes("//div[@class='c']") where node.GetAttributeValue("id", "null").StartsWith("C_") select node;

                foreach (HtmlNode node in q)
                {
                    string uid = node.SelectSingleNode("./a").GetAttributeValue("href", "null");
                    uid = Ultility.parseUid(uid);
                    string comContent = node.SelectSingleNode("./span[@class=\"ctt\"]").InnerText;

                    Console.WriteLine(String.Format("Url:{0} PageId:{1}", pageUrl, 1));
                    WeiBoManager.AddComment(comContent);
                }

                if (comPageNum > 1)
                {
                    comPageNum = comPageNum > 10 ? 10 : comPageNum;
                    for (int i = 2; i < comPageNum; i += 5)
                        commentCrawQueue.Enqueue(
                            new CommentCrawlJob() { BeginPage = i, EndPage = i + 4 > comPageNum ? comPageNum : i + 4, Url = pageUrl }
                        );
                }
            }
            catch (Exception ex)
            {
            }
        }


    }
}
