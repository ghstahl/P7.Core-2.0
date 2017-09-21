using Newtonsoft.Json;

namespace P7.External.SPA.Core
{
    public class SpaRecords
    {
        [JsonProperty("spas")]
        public ExternalSPARecord[] Spas { get; set; }
    }
}