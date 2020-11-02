using System;
using System.Collections.Generic;
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
        public MainWindow()
        {
     
            proxyObject = new ProxyObject();
            CefSettings settings = new CefSettings();
            CefSharpSettings.LegacyJavascriptBindingEnabled = true;
            Cef.Initialize(settings);
            InitializeComponent();
            setCookie();
            if (!IsLogined())
            {
                Login();
            }
            Chromium.JavascriptObjectRepository.Register("boundAsync", proxyObject, isAsync: true, options: BindingOptions.DefaultBinder);
            Chromium.FrameLoadEnd += ChromiumOnFrameLoadEnd;
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
            Window1 form2 = new Window1();
            form2.ShowDialog();
            if (IsLogined())
            {
                MessageBox.Show("登陆成功！");
            }
            else
            {
                MessageBox.Show("不明原因登录失败");
            }
            setCookie();
           

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

        public void showMessage(String msg)
        {
            Window2 window2 = new Window2();
            window2.setResultList(msg);
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
        public void setResultList(String list)
        {
            JObject j = new JObject(list);
            if (j["errno"] != null && j["errno"].Value<int>() == 0)
            {
                MessageBox.Show("OK");
            }
            {

            }
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
        }
    }
}

