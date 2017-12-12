using Newtonsoft.Json;

namespace P7.External.SPA.Scheduler
{
    public class ExternalUrlsOptions
    {
        [JsonProperty("urls")]
        public string Urls { get; set; }

        [JsonProperty("urlViewSchema")]
        public string UrlViewSchema { get; set; }
    }

    public static class ExternUrlsOptionConvert
    {
        public static RemoteUrls FromJson(string json) => JsonConvert.DeserializeObject<RemoteUrls>(json, Settings);

        public static string ToJson(RemoteUrls o) => JsonConvert.SerializeObject((object)o, (JsonSerializerSettings)Settings);
        // JsonConverter stuff

        static JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
        };
    }
}