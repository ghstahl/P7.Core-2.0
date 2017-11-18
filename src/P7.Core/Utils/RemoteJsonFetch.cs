using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace P7.Core.Utils
{
    public class RemoteJsonFetch
    {
        static Serilog.ILogger logger = Log.ForContext<RemoteJsonFetch>();

        public static async Task<string> GetRemoteJsonContentAsync(string url)
        {
            try
            {
                var accept = "application/json";
                var uri = url;
                var req = (HttpWebRequest) WebRequest.Create((string) uri);
                req.Accept = accept;
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