using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading;

namespace WeiBoCrawler
{
    [ServiceContract]
    public interface IContentCrawlService
    {
        [OperationContract]
        ContentCrawlResult GetContentCrawlResult(string data);
    }

    [DataContract]
    public class ContentCrawlResult
    {
        [DataMember]
        public int Sentiment { get; set; }
        [DataMember]
        public bool HasImg { get; set; }
        [DataMember]
        public bool HasUrl { get; set; }
        [DataMember]
        public double CommentEval { get; set; }
        [DataMember]
        public string Content { get; set; }
    }

    class ContentCrawlService:IContentCrawlService
    {
        List<WeiBoContentCrawler> contentCrawlerList;
        List<WeiBoCommentCrawler> commentCrawlerList;

        public ContentCrawlService(List<WeiBoContentCrawler> contentCrawlerList, List<WeiBoCommentCrawler> commentCrawlerList)
        {
            this.contentCrawlerList = contentCrawlerList;
            this.commentCrawlerList = commentCrawlerList;
        }

        public ContentCrawlResult GetContentCrawlResult(string _data)
        {
            var data = _data.Split('|');
            string weiboUrl = "http://weibo.com/" + data[0] + "/" + data[1];
            WeiBoManager.commentList.Clear();

            var contentThread = new Thread(() => { contentCrawlerList[0].Run(weiboUrl); });
            contentThread.Start();

            Thread[] commentThreadList = new Thread[commentCrawlerList.Count];
            int i = 0;

            foreach (WeiBoCommentCrawler c in commentCrawlerList)
            {
                Thread t = new Thread(c.Run);
                commentThreadList[i++] = t;
                t.Start();
            }

            contentThread.Join();

            foreach (Thread t in commentThreadList)
                t.Join();

            ContentCrawlResult result = new ContentCrawlResult();

            result.Sentiment = WeiBoManager.CalcSent();
            result.CommentEval = WeiBoManager.CalcComment();
            result.HasImg = WeiBoManager.HasImg;
            result.HasUrl = WeiBoManager.HasUrl;
            result.Content = WeiBoManager.GetContent();

            return result;
        }
    }
}
