using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Common
    {
        private readonly string filePath;

        public static async Task<string> RespondHeaderText(string url)
        {
            //sdfdfdf
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(url);
            var filetype = response.Content.Headers.ContentType.MediaType;
            return filetype;
            //using (var client = new WebClient())
            //{
            //    var contentType = client.ResponseHeaders["Content-Type"];
            //    Console.WriteLine(contentType);
            //    client.DownloadFile(new Uri(url), Directory.GetCurrentDirectory() + "\\test1.pdf");
            //    client.DownloadFileAsync("https://goopenva.org/courseware/related-resource/8226/view", Directory.GetCurrentDirectory() + "\\test.pdf");
            //    Console.WriteLine("Done");
            //}
            //string longUrl = "";
            //using (var handler = new HttpClientHandler())
            //{
            //    handler.AllowAutoRedirect = false;
            //    using (var client = new HttpClient(handler))
            //    {
            //        var response = await client.GetAsync(url);
            //        longUrl = response.Headers.Location.ToString();
            //    }
            //}
            //return longUrl;
        }
        public static string DownloadHtml(string url, Dictionary<dynamic, dynamic> headers = null, NetworkCredential credentials = null, bool utf8 = true, int timeout_ms = 0)
        {
            if (timeout_ms == 0)
                timeout_ms = 100000;

            string html = "";
            ServicePointManager.SecurityProtocol = /*SecurityProtocolType.Ssl3 |*/ SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            HttpWebRequest client = (HttpWebRequest)WebRequest.Create(url);
            client.CookieContainer = new CookieContainer();
            client.UserAgent = "Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:38.0) Gecko/20100101 Firefox/38.0"; // "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2623.112 Safari/537.36";
            client.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            client.Credentials = credentials;
            client.Method = "GET";
            client.AllowAutoRedirect = true;
            if (timeout_ms > 0)
            {
                client.Timeout = timeout_ms;
                client.ReadWriteTimeout = timeout_ms;
            }
            client.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            WebHeaderCollection headcoll = client.Headers;
            headcoll.Add(HttpRequestHeader.AcceptLanguage, "en-SG");
            if (headers != null)
            {
                if (headers.Count() > 0)
                {
                    foreach (var item in headers)
                        client.Headers.Add(item.Key, item.Value);
                }
            }

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)client.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream);
                        html = reader.ReadToEnd();

                        //if (response.CharacterSet == "UTF-8")
                        //{
                        //    //byte[] bytes = Encoding.UTF8.GetBytes(html);
                        //    //html = Encoding.Default.GetString(bytes);
                        //}
                    }
                }
            }
            catch (Exception e)
            {
                //Common.LogError(e, MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name.ToString() + " url:" + url);
                Console.WriteLine(url + " " + e.Message);
            }

            return html;
        }

        public static async Task<string> DownloadHtmlAsync(string url,  Dictionary<string, string> headers = null, NetworkCredential credentials = null, bool utf8 = true, int timeout_ms = 0)
        {
            Random rand = new Random();
            if (timeout_ms == 0) timeout_ms = 100000;

            using (HttpClientHandler handler = new HttpClientHandler())
            {
                handler.AllowAutoRedirect = true;
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                if (credentials != null)
                    handler.Credentials = credentials;

                using (HttpClient client = new HttpClient(handler))
                {
                    client.Timeout = TimeSpan.FromMilliseconds(timeout_ms);                   
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/html"));
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/xhtml+xml"));
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/xml", 0.9));
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("*/*", 0.8));
                    client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-SG");

                    if (headers != null && headers.Count > 0)
                    {
                        foreach (var item in headers)
                        {
                            client.DefaultRequestHeaders.TryAddWithoutValidation(item.Key, item.Value);
                        }
                    }

                    try
                    {
                        HttpResponseMessage response = await client.GetAsync(url);
                        response.EnsureSuccessStatusCode();
                        return await response.Content.ReadAsStringAsync();
                    }
                    catch (HttpRequestException e)
                    {
                        return $"Error: {e.Message}";
                    }
                }
            }
        }

        public static void FileDownload(string url, string fName)
        {
           
            string filePath = Path.Combine(Directory.GetCurrentDirectory()) + "\\DownloadFiles\\" + fName;
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(url, filePath);
                }                
            }
            catch (Exception ex)
            {
                Console.WriteLine( $"An error occurred: {ex.Message}");
            }
        }

        public static string strClean(string str)
        {
            string result = Regex.Replace(str, @"\t|\n|\r", "");
            result = Regex.Replace(result, @"\s+", " ");
            result = Regex.Replace(result, "<!--.*?-->", string.Empty);
            result = WebUtility.HtmlDecode(result);
            result = WebUtility.UrlDecode(result);
            result = Regex.Replace(result, "’", "'");
            result = result.Trim();
            return result;
        }
        //aaa

    }
}
