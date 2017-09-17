using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using P7.Core.Utils;


namespace P7.IdentityServer4.Common
{
    public class ResourcesModel :
        AbstractResourcesModel<List<ApiResourceModel>, List<IdentityResourceModel>>
    {
        public ResourcesModel()
            : base()
        {
        }

        public ResourcesModel(global::IdentityServer4.Models.Resources resources) : base(resources)
        {
        }

        public override List<IdentityResourceModel> Serialize(ICollection<IdentityResourceModel> identityResources)
        {
            return identityResources.ToList();
        }

        public override List<ApiResourceModel> Serialize(ICollection<ApiResourceModel> apiResources)
        {
            return apiResources.ToList();
        }

        protected override async Task<List<IdentityResourceModel>> DeserializeIdentityResourcesAsync(List<IdentityResourceModel> obj)
        {
            return obj;
        }

        protected override async Task<List<ApiResourceModel>> DeserializeApiResourcesAsync(List<ApiResourceModel> obj)
        {
            return obj;
        }

        public override bool Equals(object obj)
        {
            var other = obj as ResourcesModel;
            if (other == null)
            {
                return false;
            }
            var result = ApiResources.SafeListEquals(other.ApiResources)
                         && OfflineAccess.SafeEquals(other.OfflineAccess)
                         && IdentityResources.SafeListEquals(other.IdentityResources);
            return result;
        }

        public override int GetHashCode()
        {
            var code = OfflineAccess.GetHashCode();
            foreach (var apiResource in ApiResources)
            {
                code ^= apiResource.GetHashCode();
            }
            foreach (var identityResource in IdentityResources)
            {
                code ^= identityResource.GetHashCode();
            }
            return code;
        }
    }
}
