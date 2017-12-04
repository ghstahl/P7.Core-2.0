using Newtonsoft.Json;
using ZeroFormatter;

namespace P7.External.SPA.Core
{
    [ZeroFormattable]
    public class ExternalSPARecord
    {
        [JsonProperty("renderTemplate")]
        [Index(0)]
        public virtual string RenderTemplate { get; set; }

        [JsonProperty("key")]
        [Index(1)]
        public virtual string Key { get; set; }

        [JsonProperty("requireAuth")]
        [Index(2)]
        public virtual bool RequireAuth { get; set; }

        [JsonProperty("strongLoginRequiredSeconds")]
        [Index(3)]
        public virtual int StrongLoginRequiredSeconds { get; set; }

        [JsonProperty("view")]
        [Index(4)]
        public virtual string View { get; set; }

        [JsonProperty("clientId")]
        [Index(5)]
        public virtual string ClientId { get; set; }

        [JsonProperty("redirectUri")]
        [Index(6)]
        public virtual string RedirectUri { get; set; }
    }
}