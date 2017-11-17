using Newtonsoft.Json;

namespace ReferenceWebApp.Models
{
    public class JwtBearerOptions
    {
        [JsonProperty("authority")]
        public string Authority { get; set; }

        [JsonProperty("audience")]
        public string Audience { get; set; }

        [JsonProperty("requireHttpsMetadata")]
        public bool RequireHttpsMetadata { get; set; }
        [JsonProperty("saveToken")]
        public bool SaveToken { get; set; }
    }
}