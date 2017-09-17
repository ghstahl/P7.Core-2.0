using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using P7.Core.Utils;

namespace P7.IdentityServer4.Common
{
    public class IdentityResourceModel :
        AbstractIdentityResourceModel<List<string>>
    {
        public IdentityResourceModel()
        {
        }

        public IdentityResourceModel(IdentityResource identityResource) : base(identityResource)
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
            var other = obj as IdentityResourceModel;
            if (other == null)
            {
                return false;
            }
 
            var result = UserClaims.SafeListEquals(other.UserClaims)
                         && Description.SafeEquals(other.Description)
                         && DisplayName.SafeEquals(other.DisplayName)
                         && Enabled.SafeEquals(other.Enabled)
                         && Emphasize.SafeEquals(other.Emphasize)
                         && Required.SafeEquals(other.Required)
                         && ShowInDiscoveryDocument.SafeEquals(other.ShowInDiscoveryDocument);
            return result;
        }

        public override int GetHashCode()
        {
            var code = Name.GetHashCode();
            return code;
        }
    }
}
