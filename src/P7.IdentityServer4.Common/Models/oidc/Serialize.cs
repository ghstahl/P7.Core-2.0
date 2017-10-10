using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace P7.IdentityServer4.Common.Models.oidc
{
    public static class Serialize
    {
        public static string ToJson(this OIDCRecord self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }
}
