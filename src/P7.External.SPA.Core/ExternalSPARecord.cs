using Newtonsoft.Json;

namespace P7.External.SPA.Core
{
    public class ExternalSPARecord
    {
        [JsonProperty("renderTemplate")]
        public string RenderTemplate { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("requireAuth")]
        public bool RequireAuth { get; set; }

        [JsonProperty("view")]
        public string View { get; set; }
    }
}