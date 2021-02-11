using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace MyNCMusic.Helper
{
    public static class HttpHelper
    {
        /// <summary>
        /// 通用网络请求Get
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static async Task<string> HttpClientGet(string uri)
        {
            var http = new HttpClient();
            http.DefaultRequestHeaders.Add("User-Agent", "MyNCMusic");
            http.DefaultRequestHeaders.Add("withCredentials", "true");
            HttpResponseMessage response = null;
            try
            {
                response = await http.GetAsync(new Uri(uri));
            }
            catch (Exception)
            {
                http.Dispose();
                return null;
            }
            http.Dispose();
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            return await response.Content.ReadAsStringAsync();
        }
    }
    /// <summary>
    /// 封装自维护cookie http
    /// </summary>
    public class Http
    {
        public static CookieContainer cookies;

        /// <summary>
        /// GET方法(自动维护cookie)
        /// </summary>
        public static string Get(string url, string referer = "", int timeout = 2000, Encoding encode = null)
        {
            string dat;
            HttpWebResponse res = null;
            HttpWebRequest req = null;
            try
            {
                req = (HttpWebRequest)WebRequest.Create(url);
                req.CookieContainer = cookies;
                req.AllowAutoRedirect = false;
                req.Timeout = timeout;
                req.Referer = referer;
                req.UserAgent = "Mozilla/5.0 (Windows NT 10.0;%20WOW64; rv:47.0) Gecko/20100101 Firefox/47.0";
                res = (HttpWebResponse)req.GetResponse();
                if (res.StatusCode != HttpStatusCode.OK)
                    return null;
                cookies.Add(res.Cookies);
                dat = new StreamReader(res.GetResponseStream(), encode ?? Encoding.UTF8).ReadToEnd();
                res.Close();
                req.Abort();
            }
            catch
            {
                return null;
            }
            return dat;
        }

        /// <summary>
        /// Post方法(自动维护cookie)
        /// </summary>
        public static string Post(string url, string postdata, CookieContainer cookie = null, string referer = "", int timeout = 2000, Encoding encode = null)
        {
            string html = null;
            HttpWebRequest request;
            HttpWebResponse response;
            if (encode == null) encode = Encoding.UTF8;
            try
            {
                byte[] byteArray = encode.GetBytes(postdata); // 转化
                request = (HttpWebRequest)WebRequest.Create(new Uri(url));
                if (cookie == null) cookie = new CookieContainer();
                request.CookieContainer = cookie;
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; InfoPath.1)";
                request.Method = "POST";
                request.Referer = referer;
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = byteArray.Length;
                request.Timeout = timeout;
                Stream newStream = request.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length);    //写入参数
                newStream.Close();
                response = (HttpWebResponse)request.GetResponse();
                cookie.Add(response.Cookies);
                StreamReader str = new StreamReader(response.GetResponseStream(), encode);
                html = str.ReadToEnd();
            }
            catch
            {
                return "";
            }
            return html;
        }
    }
}
