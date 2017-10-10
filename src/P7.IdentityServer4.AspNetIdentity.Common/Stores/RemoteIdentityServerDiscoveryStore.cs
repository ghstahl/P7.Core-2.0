using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using P7.IdentityServer4.AspNetIdentity.Configuration;

namespace P7.IdentityServer4.AspNetIdentity.Stores
{
    public class RemoteIdentityServerDiscoveryStore : IRemoteIdentityServerDiscoveryStore
    {
        // "https://localhost:44311/.well-known/openid-configuration";
        public static OpenidConfiguration FromJson(string json) 
            => JsonConvert.DeserializeObject<OpenidConfiguration>(json, Converter.Settings);

        private OpenidConfiguration OpenidConfiguration { get; set; }
        public async Task<OpenidConfiguration> GetRemoteDataAsync(string url)
        {
            var accept = "application/json";
            var uri = url;
            var req = (HttpWebRequest)WebRequest.Create(uri);
            req.Accept = accept;
            var content = new MemoryStream();
            OpenidConfiguration record;
            using (WebResponse response = await req.GetResponseAsync())
            {
                using (Stream responseStream = response.GetResponseStream())
                {

                    // Read the bytes in responseStream and copy them to content.
                    await responseStream.CopyToAsync(content);
                    string result = Encoding.UTF8.GetString(content.ToArray());
                    record = FromJson(result);
                }
            }
            return record;
        }

        public async Task<bool> LoadRemoteDataAsync(string url)
        {
            var result = await GetRemoteDataAsync(url);
            if (result != null)
            {
                OpenidConfiguration = result;
                return true;
            }
            return false;
        }

        public OpenidConfiguration GetOpenidConfiguration()
        {
            return OpenidConfiguration;
        }
    }
}