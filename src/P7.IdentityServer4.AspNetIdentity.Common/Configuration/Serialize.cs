using Newtonsoft.Json;

namespace P7.IdentityServer4.AspNetIdentity.Configuration
{
    public static class Serialize
    {
        public static string ToJson(this OpenidConfiguration self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }
}