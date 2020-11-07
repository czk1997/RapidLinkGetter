using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

namespace RapidLinkGetter
{
    /// <summary>
    /// WebViewTestPage.xaml 的交互逻辑
    /// </summary>
    public partial class WebViewTestPage : Window
    {
        private BaiduYunCookie baiduYunCookie;

        public WebViewTestPage()
        {
            InitializeComponent();
        }

        private void ParseButton_Click(object sender, RoutedEventArgs e)
        {
            getWebLink(UrlBox.Text);
        }

        private void ParseWebPage(string url)
        {
            if (!CheckLoginStatus())
            {
                //TODO: Finish Login Check
                throw new Exception("Request Login.");
            }

            if (!IsValidURL(url))
            {
                throw new Exception("Invalid URL");
            }
        }
        private bool CheckLoginStatus()
        {
            //TODO: Finish Login Check
            return true;
        }

        private bool IsValidURL(string url)
        {
            Match match = Regex.Match(url, BaiduYunInfoGetter.ValidShareLinkPattern);
            if (!match.Success)
            {
                MessageBox.Show("输入内容非法，请重新输入。");
                return false;
            }
            return true;
        }
        private string RequestWebPage(string url)
        {
            //Create URL Based On the User Input
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
            request.Method = "GET";
            request.UserAgent = BaiduYunInfoGetter.BrowserUserAgent;
            TryAddCookie(request, new Cookie("BAIDUID", this.baiduYunCookie._baiduid) { Domain = ".baidu.com" });
            TryAddCookie(request, new Cookie("PANWEB", baiduYunCookie._pnaweb) { Domain = ".baidu.com" });
            var response =  request.GetResponse();
            var encoding = Encoding.UTF8;
            var reader = new StreamReader(response.GetResponseStream(), encoding);
            string responseText = reader.ReadToEnd();
            
            return responseText;
        }

        private void getWebLink(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.UserAgent =
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.111 Safari/537.36";
            TryAddCookie(request, new Cookie("BAIDUID", baiduYunCookie._baiduid) { Domain = ".baidu.com" });
            TryAddCookie(request, new Cookie("PANWEB", "1") { Domain = ".baidu.com" });
            var response = (HttpWebResponse)request.GetResponse();
            var encoding = Encoding.UTF8;
            var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding);
            string responseText = reader.ReadToEnd();
            parseYunDataFromPage(responseText);
            string surl = getSurl(responseText);
            reader.Close();
            response.Close();
            request = (HttpWebRequest)WebRequest.Create(
              BaiduYunInfoGetter.GetVerifyLink(responseText));
            request.Method = "POST";
            request.UserAgent =
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.111 Safari/537.36";
            request.Referer = "https://pan.baidu.com/disk/home";
            request.ContentType = "application/x-www-form-urlencoded";
            var postData = "pwd=307k";
            byte[] data = Encoding.UTF8.GetBytes(postData);
            request.ContentLength = data.Length;
            var requsetStream = request.GetRequestStream();
            using (Stream reqStream = request.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }
            TryAddCookie(request, new Cookie("BAIDUID", baiduYunCookie._baiduid) { Domain = ".baidu.com" });
            TryAddCookie(request, new Cookie("PANWEB", "1") { Domain = ".baidu.com" });
            response = (HttpWebResponse)request.GetResponse();
            reader = new System.IO.StreamReader(response.GetResponseStream(), encoding);
            responseText = reader.ReadToEnd();
            Console.WriteLine(responseText);
        }

        private YunData parseYunDataFromPage(string html)
        {
            Console.WriteLine(html);
            YunData yunData = new YunData();
            return yunData;
        }

        private string getSurl(string html)
        {
            var groups = Regex.Match(html, "LRURPVSDB\":\"1(.*?)%26").Groups;
            var surl =  groups[1].Value;
            return surl;
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
