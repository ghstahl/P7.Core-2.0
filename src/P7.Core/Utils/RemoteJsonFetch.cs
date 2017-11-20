using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace P7.Core.Utils
{
    public class WebRequestInit
    {
        public WebHeaderCollection Headers { get; set; }
        public string Accept { get; set; }
    }
    public class RemoteJsonFetch
    {
        static Serilog.ILogger logger = Log.ForContext<RemoteJsonFetch>();

        public static async Task<string> FetchAsync(string url, WebRequestInit init)
        {
            try
            {
                var uri = url;
                var req = (HttpWebRequest)WebRequest.Create((string)uri);
                req.Accept = init.Accept;
                if (init.Headers != null)
                {

                    var allKeys = init.Headers.AllKeys;
                    foreach (var key in allKeys)
                    {
                        var value = init.Headers.Get(key);
                        req.Headers.Add(key, value);
                    }
                }
                var content = new MemoryStream();
                string result;
                using (WebResponse response = await req.GetResponseAsync())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {

                        // Read the bytes in responseStream and copy them to content.
                        await responseStream.CopyToAsync(content);
                        result = Encoding.UTF8.GetString(content.ToArray());
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                logger.Fatal("Exception Caught:{0}", e.Message);
            }
            return null;
        }

        public static async Task<string> GetRemoteJsonContentAsync(string url)
        {
            var result = await FetchAsync(url, new WebRequestInit() {Accept = "application/json"});
            return result;
        }

        public static async Task<string> GetRemoteJsonContentAsync(string url, string schema)
        {
            try
            {

                string content;

                content = await GetRemoteJsonContentAsync(url);

                var validateResponse =
                    JsonSchemaValidator.Validate(new JsonValidateRequest() {Json = content, Schema = schema});
                if (!validateResponse.Valid)
                {
                    throw new Exception(validateResponse.Errors[0].Message);
                }

                return content;

            }
            catch (Exception e)
            {
                logger.Fatal("Exception Caught:{0}", e.Message);
            }
            return null;
        }
    }
}