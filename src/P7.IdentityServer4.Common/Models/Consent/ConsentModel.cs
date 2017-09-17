using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;

namespace P7.IdentityServer4.Common
{
    public class ConsentModel :
        AbstractConsentModel<List<string>>
    {
        public ConsentModel()
            : base()
        {
        }

        public ConsentModel(global::IdentityServer4.Models.Consent consent) : base(consent)
        {
        }

        public override List<string> Serialize(List<string> scopes)
        {
            return scopes;
        }

        public override List<string> DeserializeScopes(List<string> obj)
        {
            return obj;
        }
    }
}
