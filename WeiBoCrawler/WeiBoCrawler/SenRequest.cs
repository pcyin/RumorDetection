using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace WebServiceTest
{
    class SenRequest
    {
        public static string UserAgent { get; set; }

        public static string GetHttpResponseStr(string url) {
            try
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "GET";

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                Stream receiveStream = response.GetResponseStream();
                Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                StreamReader readStream = new StreamReader(receiveStream, encode);
                return readStream.ReadToEnd();
            }
            catch (Exception ex) {
                System.Console.WriteLine(ex.Message);
                return null;
            }
        }

        
    }
}
