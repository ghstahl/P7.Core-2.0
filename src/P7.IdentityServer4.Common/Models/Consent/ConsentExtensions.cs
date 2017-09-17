using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
 

namespace P7.IdentityServer4.Common
{
    public static class ConsentExtensions
    {
        public static global::IdentityServer4.Models.Consent ToConsent(this ConsentModel model)
        {
            var result = model.ToConsentAsync();
            return result.Result;
        }
        public static async Task<global::IdentityServer4.Models.Consent> ToConsentAsync(this ConsentModel model)
        {
            var result = await model.MakeConsentAsync();
            return result;
        }
        public static ConsentModel ToConsentModel(this global::IdentityServer4.Models.Consent model)
        {
            return new ConsentModel(model);
        }
    }
}
