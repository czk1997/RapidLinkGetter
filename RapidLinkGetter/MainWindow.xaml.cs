using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CefSharp;
using CefSharp.OffScreen;
using Newtonsoft.Json.Linq;

namespace RapidLinkGetter
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private ProxyObject proxyObject;
        private RapidLinkWindow rlw;
        public MainWindow()
        {
            CefSettings settings = new CefSettings();
            CefSharpSettings.LegacyJavascriptBindingEnabled = true;
            Cef.Initialize(settings);
            InitializeComponent();
            if (!IsLogined())
            {
                Login();
            }
            else
            {
                setCookie();
            }
            rlw = new RapidLinkWindow();
            proxyObject = new ProxyObject(rlw);
            this.Closing += MainWindow_Closing;
            Chromium.JavascriptObjectRepository.Register("boundAsync", proxyObject, isAsync: true, options: BindingOptions.DefaultBinder);
            Chromium.FrameLoadEnd += ChromiumOnFrameLoadEnd;
            Chromium.AddressChanged += OnBrowserAddressChange;
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
           rlw.Close();
        }

        private void ChromiumOnFrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
           InjectJS();
        }
        private void setCookie()
        {
            var cookieManager = Cef.GetGlobalCookieManager();
            using (StreamReader sr = new StreamReader("cookies.txt"))
            {
                while (sr.Peek() >= 0)
                {
                    var line = sr.ReadLine().Split('\a');
                    var name = line[0];
                    var value = line[1];

                    cookieManager.SetCookie("http://pan.baidu.com", new Cookie()
                    {
                        Name = name,
                        Value = value
                    });
                }
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Login();
        }

        private void Login()
        {
            Chromium.Address = "https://pan.baidu.com";
        }
       
        private Boolean IsLogined()
        {
            //TODO: Add More Verfication Here
            if (System.IO.File.Exists("cookies.txt"))
            {
                LoginStatus.Text = "已登录";
                LoginStatus.Foreground = new SolidColorBrush(Colors.Green);
                using (StreamReader sr = new StreamReader("cookies.txt"))
                {
                    string line = sr.ReadLine();

                }

                return true;
            }
            return false;
        }

        private void URLTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ParseButton_Click_1(object sender, RoutedEventArgs e)
        {
            
           // Chromium.ShowDevTools();
            string url = URLTextBox.Text;
            string pattern =
                "((?:https?:\\/\\/)?(?:yun|pan|eyun)\\.baidu\\.com\\/(?:s\\/\\w*(((-)?\\w*)*)?|share\\/\\S*\\d\\w*))";
            Match match = Regex.Match(url, pattern);
            if (!match.Success)
            {
                MessageBox.Show("输入内容非法，请重新输入。");
                return;
            }
            Chromium.Address = url;
       
        }
        private async void InjectJS()
        {
            using (StreamReader sr = new StreamReader("inject.js"))
            {
                var task = Chromium.GetMainFrame().EvaluateScriptAsync(
                    sr.ReadToEnd()
                    ).ContinueWith(t => { });
            }
        }

   
        private async void getDownLoadURL()
        {

            var myScript = @"(function () {
    getDownloadLink();
})();";
            var task = Chromium.GetMainFrame().EvaluateScriptAsync(
                myScript
                   ).ContinueWith(t => { });
        }

        private void GetLinkButton_Click(object sender, RoutedEventArgs e)
        {
            getDownLoadURL();
            rlw.ShowDialog();
            rlw.Close();
            rlw =new RapidLinkWindow();
            proxyObject.SetInstance(rlw);
        }
       
        private void OnBrowserAddressChange(object sender, DependencyPropertyChangedEventArgs e)
        {
            String NewURL = (String)e.NewValue;
            if (NewURL.Contains("pan.baidu.com/disk/home"))
            {
                var visitor = new CookieMonster(all_cookies =>
                {
                    var sb = new StringBuilder();
                    foreach (var nameValue in all_cookies)
                        sb.AppendLine(nameValue.Item1 + "\x07" + nameValue.Item2);
                    using (StreamWriter sw = new StreamWriter("cookies.txt"))
                    {
                        sw.Write(sb);
                    }
                });
                Cef.GetGlobalCookieManager().VisitAllCookies(visitor);
            }
            else
            {
                Console.WriteLine(Chromium.Address);
            }

            IsLogined();
        }
      
        class CookieMonster : ICookieVisitor
        {
            readonly List<Tuple<string, string>> cookies = new List<Tuple<string, string>>();
            readonly Action<IEnumerable<Tuple<string, string>>> useAllCookies;

            public CookieMonster(Action<IEnumerable<Tuple<string, string>>> useAllCookies)
            {
                this.useAllCookies = useAllCookies;
            }

            public void Dispose()
            {

            }

            public bool Visit(Cookie cookie, int count, int total, ref bool deleteCookie)
            {
                cookies.Add(new Tuple<string, string>(cookie.Name, cookie.Value));
                if (count == total - 1)
                    useAllCookies(cookies);
                return true;
            }
           
        }
    }
}
