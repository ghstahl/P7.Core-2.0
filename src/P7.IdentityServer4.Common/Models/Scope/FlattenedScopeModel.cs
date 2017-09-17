using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P7.IdentityServer4.Common
{
    public class FlattenedScopeModel :
        AbstractScopeModel<string>
    {
        internal override string Serialize(ICollection<string> userClaims)
        {
            if (userClaims == null)
                return "[]";
            var simpleDocument = new SimpleJsonJsonDocument<List<string>>(userClaims.ToList()).DocumentJson;
            return simpleDocument;
        }

        protected override async Task<List<string>> DeserializeUserClaimsAsync(string obj)
        {
            obj = string.IsNullOrEmpty(obj) ? "[]" : obj;
            var simpleDocument = new SimpleJsonJsonDocument<List<string>>(obj);
            var document = (List<string>)simpleDocument.Document;
            return document;
        }
    }
}