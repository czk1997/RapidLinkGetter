using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CefSharp;

namespace RapidLinkGetter
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            Browser.AddressChanged += OnBrowserAddressChange;
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
                Console.WriteLine(Browser.Address);
            }
        }
        public class CallbackObjectForJs{
    public void showMessage(string msg){//Read Note
        MessageBox.Show(msg);
    }
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
