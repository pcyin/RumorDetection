using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using System.Data.SqlClient;
using System.Xml.Linq;
using MySql.Data.MySqlClient;
using MySql.Data;
using System.Messaging;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace WeiBoCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigManager.IsUseProxy = false;
            ConcurrentQueue<string> urlQueue = new ConcurrentQueue<string>();
            ConcurrentQueue<CommentCrawlJob> comJobQueue = new ConcurrentQueue<CommentCrawlJob>();
            List<WeiBoContentCrawler> contentCrawlerList = new List<WeiBoContentCrawler>();
            List<WeiBoCommentCrawler> commentCrawlerList = new List<WeiBoCommentCrawler>();

            string exePath = System.IO.Path.Combine(Environment.CurrentDirectory, "appconfig.xml");
            XElement rootNode = XElement.Load(exePath);
            var crawlers = from node in rootNode.Descendants("crawler") select node;

            foreach (var crawler in crawlers) {
                string id = crawler.Attribute("id").Value;
                string type = crawler.Attribute("type").Value;
                var cookies = from cookie in crawler.Element("cookies").Descendants() select cookie;
                CookieCollection cookieCol = new CookieCollection();
                foreach (var cookie in cookies) {
                    Cookie c = new Cookie(cookie.Attribute("key").Value, cookie.Attribute("value").Value);
                    c.Domain = ".weibo.cn";
                    cookieCol.Add(c);
                }
                Uri proxyUri = new Uri(
                        crawler.Element("proxy").Attribute("url").Value
                    );
                if (type == "ContentCrawler")
                {
                    WeiBoContentCrawler c = new WeiBoContentCrawler(urlQueue, comJobQueue, cookieCol, proxyUri);
                    contentCrawlerList.Add(c);
                }
                else {
                    WeiBoCommentCrawler c = new WeiBoCommentCrawler(comJobQueue, cookieCol, proxyUri);
                    commentCrawlerList.Add(c);
                }
            }

            foreach (var c in contentCrawlerList)
            {
                c.CommentCrawlerList = commentCrawlerList;
            }

            WeiBoManager.Init();

            Uri baseAddress = new Uri("http://localhost:6525/ContentCrawl");

            ContentCrawlService service = new ContentCrawlService(contentCrawlerList,commentCrawlerList);

            using (ServiceHost host = new ServiceHost(service, baseAddress))
            {
                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
                host.Description.Behaviors.Add(smb);
                var behavior = host.Description.Behaviors.Find<ServiceBehaviorAttribute>();
                behavior.InstanceContextMode = InstanceContextMode.Single;

                host.Open();

                Console.WriteLine("The ContentCrawl Service is ready at: {0}", baseAddress);
                Console.WriteLine("Press <Enter> to stop the service");
                Console.ReadKey();
                host.Close();
            }

        }
    }
}
