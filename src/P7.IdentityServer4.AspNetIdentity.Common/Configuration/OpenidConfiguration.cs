using Newtonsoft.Json;

namespace P7.IdentityServer4.AspNetIdentity.Configuration
{
    public partial class OpenidConfiguration
    {
        [JsonProperty("frontchannel_logout_session_supported")]
        public bool FrontchannelLogoutSessionSupported { get; set; }

        [JsonProperty("check_session_iframe")]
        public string CheckSessionIframe { get; set; }

        [JsonProperty("backchannel_logout_session_supported")]
        public bool BackchannelLogoutSessionSupported { get; set; }

        [JsonProperty("authorization_endpoint")]
        public string AuthorizationEndpoint { get; set; }

        [JsonProperty("backchannel_logout_supported")]
        public bool BackchannelLogoutSupported { get; set; }

        [JsonProperty("code_challenge_methods_supported")]
        public string[] CodeChallengeMethodsSupported { get; set; }

        [JsonProperty("claims_supported")]
        public object[] ClaimsSupported { get; set; }

        [JsonProperty("end_session_endpoint")]
        public string EndSessionEndpoint { get; set; }

        [JsonProperty("introspection_endpoint")]
        public string IntrospectionEndpoint { get; set; }

        [JsonProperty("response_types_supported")]
        public string[] ResponseTypesSupported { get; set; }

        [JsonProperty("grant_types_supported")]
        public string[] GrantTypesSupported { get; set; }

        [JsonProperty("frontchannel_logout_supported")]
        public bool FrontchannelLogoutSupported { get; set; }

        [JsonProperty("id_token_signing_alg_values_supported")]
        public string[] IdTokenSigningAlgValuesSupported { get; set; }

        [JsonProperty("jwks_uri")]
        public string JwksUri { get; set; }

        [JsonProperty("issuer")]
        public string Issuer { get; set; }

        [JsonProperty("response_modes_supported")]
        public string[] ResponseModesSupported { get; set; }

        [JsonProperty("scopes_supported")]
        public string[] ScopesSupported { get; set; }

        [JsonProperty("token_endpoint")]
        public string TokenEndpoint { get; set; }

        [JsonProperty("revocation_endpoint")]
        public string RevocationEndpoint { get; set; }

        [JsonProperty("subject_types_supported")]
        public string[] SubjectTypesSupported { get; set; }

        [JsonProperty("token_endpoint_auth_methods_supported")]
        public string[] TokenEndpointAuthMethodsSupported { get; set; }

        [JsonProperty("userinfo_endpoint")]
        public string UserinfoEndpoint { get; set; }
    }

    public partial class OpenidConfiguration
    {
        public static OpenidConfiguration FromJson(string json) => JsonConvert.DeserializeObject<OpenidConfiguration>(json, Converter.Settings);
    }
}