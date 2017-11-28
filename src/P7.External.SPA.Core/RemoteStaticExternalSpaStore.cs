using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using P7.Core.Utils;

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
            string content = await RemoteJsonFetch.GetRemoteJsonContentAsync(url);
            var spaRecords = FromJson(content);
            return spaRecords;
        }

        public async Task<bool> LoadRemoteDataAsync(string url)
        {
            var result = await GetRemoteDataAsync(url);
            if (result.Spas != null)
            {
                AddRecords(result.Spas);
                return true;
            }
            return false;
        }
    }
}