using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using P7.Core.Utils;


namespace P7.IdentityServer4.Common
{
    public class ScopeModel :
        AbstractScopeModel<List<string>>
    {
        public ScopeModel()
        {
        }

        public ScopeModel(Scope scope) : base(scope)
        {
        }

        internal override List<string> Serialize(ICollection<string> userClaims)
        {
            return userClaims.ToList();
        }

        protected override async Task<List<string>> DeserializeUserClaimsAsync(List<string> obj)
        {
            return obj;
        }

        public override bool Equals(object obj)
        {
            var other = obj as ScopeModel;
            if (other == null)
            {
                return false;
            }

            var result = Description.SafeEquals(other.Description)
                   && DisplayName.SafeEquals(other.DisplayName)
                   && Emphasize.SafeEquals(other.Emphasize)
                   && Name.SafeEquals(other.Name)
                   && Required.SafeEquals(other.Required)
                   && ShowInDiscoveryDocument.SafeEquals(other.ShowInDiscoveryDocument)
                   && UserClaims.SafeListEquals(other.UserClaims);
            return result;
        }

        public override int GetHashCode()
        {
            var code = Name.GetHashCode();
            return code;
        }

    }
}