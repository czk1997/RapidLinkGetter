using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RapidLinkGetter
{
    class BaiduYunInfoGetter
    {
        public static string VerfiyURL= "https://pan.baidu.com/share/verify?channel=chunlei&clienttype=0&web=1&app_id=250528&surl=";
        public static string SurlPattern = "LRURPVSDB\":\"1(.*?)%26";
        public static string GetVerifyLink(String html)
        {
            var groups = Regex.Match(html, SurlPattern).Groups;
            var surl = groups[1].Value;
            return VerfiyURL + surl;
        }

        public static string ValidShareLinkPattern =
            "((?:https?:\\/\\/)?(?:yun|pan|eyun)\\.baidu\\.com\\/(?:s\\/\\w*(((-)?\\w*)*)?|share\\/\\S*\\d\\w*))";
        public static string BrowserUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.111 Safari/537.36";
        
    }
}
