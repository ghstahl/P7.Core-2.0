using Newtonsoft.Json;

namespace P7.External.SPA.Scheduler
{
    public partial class RemoteUrls
    {
        [JsonProperty("urls")]
        public string[] Urls { get; set; }
    }
}