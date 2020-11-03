using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CefSharp.DevTools.CSS;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RapidLinkGetter
{
    /// <summary>
    /// RepidLinkWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RapidLinkWindow : Window
    {
        List<String> PCSLinks = new List<string>();
        List<String> FNList = new List<string>();
        private IProgress<double> progress;
        private String BDIDUID;
        private int finishe = 0;
        public RapidLinkWindow()
        {
            Action<double> bindProgress =
                value => Probar.Value = value;
            progress = new Progress<double>(bindProgress);
            InitializeComponent();
            WriteLogBox("INFO","等待数据返回中……");
            if (System.IO.File.Exists("cookies.txt"))
            {
                SetBaiduID();
            }
        }

        public void SetBaiduID()
        {
            using (StreamReader sr = new StreamReader("cookies.txt"))
            {

                while (sr.Peek() >= 0)
                {
                    var line = sr.ReadLine().Split('\a');
                    var name = line[0];
                    var value = line[1];
                    if (name.Equals("BAIDUID"))
                    {
                        this.BDIDUID = value;
                        break; ;
                    }
                }
            }
        }


        public void setResultList(String list)
        {
            WriteLogBox("INFO", "获取到浏览器返回数据，分析中……");
            JObject j = JObject.Parse(list.Trim());
            if (j["errno"] != null && j["errno"].Value<int>() == 0)
            {
                if (j["list"]?.Value<JArray>() != null)
                {
                    var l = j["list"].Value<JArray>();
                    foreach (var child in l.Children())
                    {
                        PCSLinks.Add(child["dlink"].Value<String>());
                        FNList.Add(child["server_filename"].Value<String>());
                    }
                }
            }
            WriteLogBox("INFO", "共计 " + PCSLinks.Count + " 条数据，开始计算秒传链.");
            CalcRapidLink();
        }

        private void WriteLogBox(String Type, String Content)
        {
            DateTime now = DateTime.Now;
            LogBox.AppendText("[" + Type + "][" + now.ToString("O") + "] " + Content + "\n");

        }

        private void WriteRapidLink(String link)
        {
            ResultBox.AppendText(link + "\n");
        }

        public void CalcRapidLink()
        {
            var i = 0;
            foreach (var t in PCSLinks)
            {
                try
                {
                    GetRapidLinkByPCSLink(i, t);
                }
                catch (Exception e)
                {
                    WriteLogBox("ERROR", "出现错误位于+ " + FNList[i] + " ：\n" + e.ToString());
                }
                i++;
            }
        }

        public async void GetRapidLinkByPCSLink(int i, string link)
        {
            WriteLogBox("INFO", "正在处理第" + (i + 1) + "条数据");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(link);
            request.Method = "GET";
            request.UserAgent = "LogStatistic";
            request.AddRange(0, 262143);
            TryAddCookie(request, new Cookie("BAIDUID", this.BDIDUID) { Domain = ".baidu.com" });
            using (var response = (HttpWebResponse)await request.GetResponseAsync())
            {
                //HttpWebResponse response = (result.AsyncState as HttpWebRequest).EndGetResponse(result) as HttpWebResponse;
                MemoryStream ms = new MemoryStream();
                response.GetResponseStream().CopyTo(ms);
                byte[] data = ms.ToArray();
                ms.Close();
                MD5 md5 = MD5.Create();
                //   byte[] byteOld = Encoding.UTF8.GetBytes(result);
                StringBuilder sb = new StringBuilder();
                byte[] byteNew = md5.ComputeHash(data);
                foreach (byte b in byteNew)
                {
                    // 将字节转换成16进制表示的字符串，
                    sb.Append(b.ToString("x2"));
                }
                // 返回加密的字符串
                StringBuilder rapidLink = new StringBuilder();
                rapidLink.Append(response.Headers["Content-MD5"].ToUpper());
                rapidLink.Append("#");
                rapidLink.Append(sb.ToString().ToUpper());
                rapidLink.Append("#");
                rapidLink.Append(response.Headers["x-bs-file-size"]);
                rapidLink.Append("#");
                string cs = response.Headers["Content-Disposition"];
                string fileName = Regex.Match(cs, "filename=\"(.*?)\"").Groups[1].Value;
                byte[] byteArray = System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(fileName);
                fileName = System.Text.Encoding.UTF8.GetString(byteArray);
                rapidLink.Append(fileName);
                response.Close();
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    ReportProgress(i);
                    WriteRapidLink(rapidLink.ToString());
                }));

            }
        }

        private void ReportProgress(int i)
        {
            Interlocked.Increment(ref finishe);
            double p = (Convert.ToDouble(finishe) / Convert.ToDouble(PCSLinks.Count)) * 100d;
            progress.Report(p);
        }
        public static bool TryAddCookie(WebRequest webRequest, Cookie cookie)
        {
            HttpWebRequest httpRequest = webRequest as HttpWebRequest;
            if (httpRequest == null)
            {
                return false;
            }

            if (httpRequest.CookieContainer == null)
            {
                httpRequest.CookieContainer = new CookieContainer();
            }

            httpRequest.CookieContainer.Add(cookie);
            return true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(ResultBox.Text);
        }
    }
}
