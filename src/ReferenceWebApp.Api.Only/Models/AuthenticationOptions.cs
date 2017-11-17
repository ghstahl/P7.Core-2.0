using Newtonsoft.Json;

namespace ReferenceWebApp.Models
{
    public class AuthenticationOptions
    {
        [JsonProperty("jwtBearer")]
        public JwtBearerOptions JwtBearer  { get; set; }
    }
}