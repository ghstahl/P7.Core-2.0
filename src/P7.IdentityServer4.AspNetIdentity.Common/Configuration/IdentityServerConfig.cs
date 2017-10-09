using Newtonsoft.Json;

namespace P7.IdentityServer4.AspNetIdentity.Configuration
{
    public class IdentityServerConfig
    {
        public const string WellKnown_SectionName = "identityServer";
        [JsonProperty("discovery")]
        public string Discovery { get; set; }
    }
}
