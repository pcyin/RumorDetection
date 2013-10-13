using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SVMWrapper
{
    public class SVM
    {
        public static string SVMPath = "model/";
        public static string PredictFilePath = "";

        public void InputData(string data)
        {
            using(StreamWriter sw = new StreamWriter(Path.Combine(HttpRuntime.AppDomainAppPath,"model","predict.txt")))
            {
                sw.Write(data);
            }
        }

        public int Predict()
        {
            // Start the child process.
            Process p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            string originalDir = HttpRuntime.AppDomainAppPath;
            p.StartInfo.FileName = "svmpredict.bat";
            Directory.SetCurrentDirectory(Path.Combine(originalDir,SVMPath));
            p.Start();
            // Do not wait for the child process to exit before
            // reading to the end of its redirected stream.
            // p.WaitForExit();
            // Read the output stream first and then wait.
            p.WaitForExit();
            int result;
            Directory.SetCurrentDirectory(originalDir);
            using (StreamReader sr = new StreamReader(Path.Combine(HttpRuntime.AppDomainAppPath, "model", "output.txt")))
            {
                result = Convert.ToInt32(sr.ReadLine());
            }

            return result;
        }
    }
}
