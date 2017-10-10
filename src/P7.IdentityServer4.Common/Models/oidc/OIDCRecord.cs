using Newtonsoft.Json;

namespace P7.IdentityServer4.Common.Models.oidc
{
    public partial class OIDCRecord
    {
        public static OIDCRecord FromJson(string json) => JsonConvert.DeserializeObject<OIDCRecord>(json, Converter.Settings);
    }

    public partial class OIDCRecord
    {
        [JsonProperty("expires_in")]
        public long ExpiresIn { get; set; }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }
    }
}