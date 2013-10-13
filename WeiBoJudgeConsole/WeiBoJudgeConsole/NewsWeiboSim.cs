using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Sql;
using System.IO;
using System.Runtime.InteropServices;
using System.Data.Odbc;

namespace NewsWeiboSim
{
    public struct Sentence_Fre{
        public double fre;
        public string sentence;
    };
    [StructLayout(LayoutKind.Explicit)]
    public struct result_t
    {
        [FieldOffset(0)]
        public int start;
        [FieldOffset(4)]
        public int length;
        [FieldOffset(8)]
        public int sPos1;
        [FieldOffset(12)]
        public int sPos2;
        [FieldOffset(16)]
        public int sPos3;
        [FieldOffset(20)]
        public int sPos4;
        [FieldOffset(24)]
        public int sPos5;
        [FieldOffset(28)]
        public int sPos6;
        [FieldOffset(32)]
        public int sPos7;
        [FieldOffset(36)]
        public int sPos8;
        [FieldOffset(40)]
        public int sPos9;
        [FieldOffset(44)]
        public int sPos10;
        //[FieldOffset(12)] public int sPosLow;
        [FieldOffset(48)]
        public int POS_id;
        [FieldOffset(52)]
        public int word_ID;
        [FieldOffset(56)]
        public int word_type;
        [FieldOffset(60)]
        public double weight;
    }


    class NewsWeiboSimlarity
    {
            List<string> weiboSegment = new List<string>();
            List<string> newsTitle = new List<string>();
            string weibo;
            List<List<string>> titleSegment = new List<List<string>>();
            List<Sentence_Fre> a;
            List<string> stopWord = new List<string>();
            FileStream simFile;
            StreamWriter sw;


            public NewsWeiboSimlarity()
            {
                //tbResult.Clear();
                a = new List<Sentence_Fre>();
                Sentence_Fre f1;
                for (int i = 0; i < 5; i++)
                {
                    f1.fre = 0;
                    f1.sentence = "";
                    a.Add(f1);
                }
                


                Stopword_Load();
                Weibo_Load();
                simFile = new FileStream("newsfile_rumorsim.txt", FileMode.Create, FileAccess.Write);
                readNewsFile(@"e:/work", "alltitle.csv");

                sw = new StreamWriter(simFile);

                if (!NLPIR_Init(@"e:/work/nlpir/", 0))//给出Data文件所在的路径，注意根据实际情况修改。
                {
                    // MessageBox.Show("Init ICTCLAS failed!");
                    Console.Write("Init ICTCLAS failed!");
                }
                segNewsFile();

                sw.Close();
                simFile.Close();
          }
            #region path
            const string path = @"e:/work/nlpir/bin/NLPIR.dll";//设定dll的路径
            [DllImport(path, CharSet = CharSet.Ansi, EntryPoint = "NLPIR_Init")]
            public static extern bool NLPIR_Init(String sInitDirPath, int encoding);

            [DllImport(path, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Winapi, EntryPoint = "NLPIR_ParagraphProcess")]
            public static extern IntPtr NLPIR_ParagraphProcess(String sParagraph, int bPOStagged = 1);

            [DllImport(path, CharSet = CharSet.Ansi, EntryPoint = "NLPIR_Exit")]
            public static extern bool NLPIR_Exit();

            [DllImport(path, CharSet = CharSet.Ansi, EntryPoint = "NLPIR_ImportUserDict")]
            public static extern int NLPIR_ImportUserDict(String sFilename);

            [DllImport(path, CharSet = CharSet.Ansi, EntryPoint = "NLPIR_FileProcess")]
            public static extern bool NLPIR_FileProcess(String sSrcFilename, String sDestFilename, int bPOStagged = 1);

            [DllImport(path, CharSet = CharSet.Ansi, EntryPoint = "NLPIR_FileProcessEx")]
            public static extern bool NLPIR_FileProcessEx(String sSrcFilename, String sDestFilename);

            [DllImport(path, CharSet = CharSet.Ansi, EntryPoint = "NLPIR_GetParagraphProcessAWordCount")]
            static extern int NLPIR_GetParagraphProcessAWordCount(String sParagraph);
            //NLPIR_GetParagraphProcessAWordCount
            [DllImport(path, CharSet = CharSet.Ansi, EntryPoint = "NLPIR_ParagraphProcessAW")]
            static extern void NLPIR_ParagraphProcessAW(int nCount, [Out, MarshalAs(UnmanagedType.LPArray)] result_t[] result);

            [DllImport(path, CharSet = CharSet.Ansi, EntryPoint = "NLPIR_AddUserWord")]
            static extern int NLPIR_AddUserWord(String sWord);

            [DllImport(path, CharSet = CharSet.Ansi, EntryPoint = "NLPIR_SaveTheUsrDic")]
            static extern int NLPIR_SaveTheUsrDic();

            [DllImport(path, CharSet = CharSet.Ansi, EntryPoint = "NLPIR_DelUsrWord")]
            static extern int NLPIR_DelUsrWord(String sWord);

            [DllImport(path, CharSet = CharSet.Ansi, EntryPoint = "NLPIR_NWI_Start")]
            static extern bool NLPIR_NWI_Start();

            [DllImport(path, CharSet = CharSet.Ansi, EntryPoint = "NLPIR_NWI_Complete")]
            static extern bool NLPIR_NWI_Complete();

            [DllImport(path, CharSet = CharSet.Ansi, EntryPoint = "NLPIR_NWI_AddFile")]
            static extern bool NLPIR_NWI_AddFile(String sText);

            [DllImport(path, CharSet = CharSet.Ansi, EntryPoint = "NLPIR_NWI_AddMem")]
            static extern bool NLPIR_NWI_AddMem(String sText);

            [DllImport(path, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Winapi, EntryPoint = "NLPIR_NWI_GetResult")]
            public static extern IntPtr NLPIR_NWI_GetResult(bool bWeightOut = false);

            [DllImport(path, CharSet = CharSet.Ansi, EntryPoint = "NLPIR_NWI_Result2UserDict")]
            static extern uint NLPIR_NWI_Result2UserDict();
             #endregion

            void readNewsFile(string path, string fileName)
            {
                string strConn = @"Driver={Microsoft Text Driver (*.txt; *.csv)};Dbq=";
                strConn += path;
                strConn += ";Extensions=asc,csv,tab,txt;";

                OdbcConnection objConn = new OdbcConnection(strConn);
                objConn.Open();
                OdbcDataReader reader = null;
                try
                {
                    string strSQL = "select * from " + fileName;
                    OdbcCommand cmd = objConn.CreateCommand();
                    cmd.CommandText = strSQL;
                    reader = cmd.ExecuteReader();
                    int i = 0;
                    while (reader.Read())
                    {

                        newsTitle.Add(reader.GetString(0));
                        //Console.WriteLine(i++);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                    objConn.Close();
                }
            }

            List<string> segParagraph(string str)
            {
                List<string> segList = new List<string>();
                int count = NLPIR_GetParagraphProcessAWordCount(str);//先得到结果的词数
                result_t[] result = new result_t[count];//在客户端申请资源
                NLPIR_ParagraphProcessAW(count, result);//获取结果存到客户的内存中
                byte[] data = System.Text.Encoding.GetEncoding("GB2312").GetBytes(str);
                byte[] temp;
                foreach (result_t r in result)
                {
                    temp = new byte[r.length];
                    for (int j = r.start; j < r.start + r.length; j++)
                        temp[j - r.start] = data[j];
                    string word = Encoding.GetEncoding("GB2312").GetString(temp);
                    if (stopWord.Contains(word)==false)
                    {
                        segList.Add(word);
                    }
                }
                return segList;
            }
            public List<string> BiggestFiveTitle(string s)
            {
                    List<string> r = new List<string>();
                    double simcos = 0;
                    weibo = s;
                    int i = 0;
                    double weiboCount;
                    string highsim = "";
                    List<string> high = new List<string>();
                    weiboSegment = segParagraph(weibo);
                    weiboCount = weiboSegment.Count;

                    Dictionary<string, bool> weiboWordDict = new Dictionary<string, bool>();
                    foreach (String word in weiboSegment)
                    {
                        weiboWordDict[word] = true;
                    }

                    for (int titileiter = 0; titileiter < titleSegment.Count; titileiter++)   //对比每一个
                    {
                        List<string> wordList = titleSegment[titileiter];

                        double matchCount = 0;
                        double rawCount = wordList.Count;
                        if (wordList.Count <= 3)
                            continue;
                        foreach (string word in wordList)
                        {
                            if (weiboWordDict.ContainsKey(word))
                                matchCount++;
                        }
                        /*
                        resultList.Items.Add(new ListViewItem(new string[] { 
                        title,
                        rawCount.ToString(),
                        matchCount.ToString()
                    }));*/
                        if (simcos <
                            matchCount / rawCount)
                        {
                            simcos =
                                matchCount / rawCount;
                            Sentence_Fre temp;
                            temp.fre = simcos;
                            temp.sentence = newsTitle[titileiter];
                            a.Remove(a[0]);
                            a.Add(temp);
                            a.Sort(new Fredesc());
                            high = wordList;
                        }

                        //Console.Write("hello world");
                        //tbResult.AppendText(title + "\t" + rawCount + "\t" + matchCount + "\n");

                    }
                    Console.Write("微博是:" + weibo);
                    for (int iter = 0; iter < 5; iter++)
                    {
                        Console.WriteLine(a[iter].fre.ToString());
                        Console.WriteLine(a[iter].sentence);
                        r.Add(a[iter].sentence);
                    }
                    a.Clear();
                    for (i = 0; i < 5; i++)
                    {
                        Sentence_Fre t;
                        t.fre = 0;
                        t.sentence = "";
                        a.Add(t);
                    }
                    //sw.WriteLine(simcos.ToString());
                return r;
            }
            private void Form1_Load(object sender, EventArgs e)
            {
                if (!NLPIR_Init(@"e:/work/nlpir/", 0))//给出Data文件所在的路径，注意根据实际情况修改。
                {
                   // MessageBox.Show("Init ICTCLAS failed!");
                    Console.Write("Init ICTCLAS failed!");
                    return;
                }
                //System.Console.WriteLine("Init ICTCLAS success!");

            }

            public void  Weibo_Load()
            {
                ;
            }
            private void segNewsFile()
            {
                for (int i=0;i<newsTitle.Count;i++)   //对比每一个
                {
                    List<string> wordList = segParagraph(newsTitle[i]);
                    //Console.WriteLine(i);
                    titleSegment.Add(wordList);
                }
            }
            private void Stopword_Load()
            {
                FileStream fs = new FileStream("stopword.txt", FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs);

                string temp;
                temp = sr.ReadLine();
                while(temp!=null)
                {
                    stopWord.Add(temp);
                    temp = sr.ReadLine();
                }
                sr.Close();
                fs.Close();
            }
            
        }

    class Fredesc : IComparer<Sentence_Fre>
    {
        public int Compare(Sentence_Fre x, Sentence_Fre y)
        {
            return x.fre.CompareTo(y.fre);
        }
    }
}
