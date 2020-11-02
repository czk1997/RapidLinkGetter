using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Window2.xaml 的交互逻辑
    /// </summary>
    public partial class Window2 : Window
    {
        List<String> PCSLinks = new List<string>();
        private String BDIDUID;
        public Window2()
        {
            InitializeComponent();
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
            JObject j = JObject.Parse(list.Trim());
            if (j["errno"] != null && j["errno"].Value<int>() == 0)
            {
                if (j["list"]?.Value<JArray>() != null)
                {
                    var l = j["list"].Value<JArray>();
                    foreach (var child in l.Children())
                    {

                        PCSLinks.Add(child["dlink"].Value<String>());
                    }
                }
            }

            GetRapidLinkByPCSLink(PCSLinks[0]);
        }

        public string GetRapidLinkByPCSLink(string link)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(link);
            request.Method = "GET";
            request.UserAgent = "LogStatistic";
            request.AddRange(0,262143);
            TryAddCookie(request, new Cookie("BAIDUID", this.BDIDUID){Domain = ".baidu.com"});
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
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
            Console.WriteLine(sb.ToString());
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
            Console.WriteLine(fileName);
            rapidLink.Append(fileName);
            response.Close();
            return sb.ToString();
            
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
    }
}
