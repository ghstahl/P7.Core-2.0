using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace P7.External.SPA.Core
{
    public class RemoteStaticExternalSpaStore : InMemoryExternalSpaStore, IRemoteExternalSPAStore
    {
        //"https://rawgit.com/ghstahl/P7/master/src/WebApplication5/external.spa.config.json";
        public static SpaRecords FromJson(string json) => JsonConvert.DeserializeObject<SpaRecords>(json, Settings);
        public static string ToJson(SpaRecords o) => JsonConvert.SerializeObject((object) o, (JsonSerializerSettings) Settings);

        // JsonConverter stuff

        static JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
        };

        public async Task<SpaRecords> GetRemoteDataAsync(string url)
        {
            var accept = "application/json";
            var uri = url;
            var req = (HttpWebRequest)WebRequest.Create(uri);
            req.Accept = accept;
            var content = new MemoryStream();
            SpaRecords spaRecords;
            using (WebResponse response = await req.GetResponseAsync())
            {
                using (Stream responseStream = response.GetResponseStream())
                {

                    // Read the bytes in responseStream and copy them to content.
                    await responseStream.CopyToAsync(content);
                    string result = Encoding.UTF8.GetString(content.ToArray());
                    spaRecords = FromJson(result);
                }
            }
            return spaRecords;
        }

        public async Task LoadRemoteDataAsync(string url)
        {
            var result = await GetRemoteDataAsync(url);
            if (result.Spas != null)
            {
                AddRecords(result.Spas);
            }
        }
    }
}