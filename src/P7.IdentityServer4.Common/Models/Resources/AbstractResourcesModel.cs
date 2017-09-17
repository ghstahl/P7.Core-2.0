using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;

namespace P7.IdentityServer4.Common
{
    public abstract class AbstractResourcesModel<TApiResources, TIdentityResource> : IResourcesModel
         where TApiResources : class
         where TIdentityResource : class
    {

        public AbstractResourcesModel()
        {
        }

        public AbstractResourcesModel(global::IdentityServer4.Models.Resources resources)
        {
            ApiResources = Serialize(resources.ApiResources);
            IdentityResources = Serialize(resources.IdentityResources);
            OfflineAccess = resources.OfflineAccess;
        }

        private TIdentityResource Serialize(ICollection<IdentityResource> identityResources)
        {
            var identityResourceModels = identityResources.ToIdentityResourceModels();
            return Serialize(identityResourceModels);
        }

        private TApiResources Serialize(ICollection<ApiResource> apiResources)
        {
            var apiResourceModels = apiResources.ToApiResourceModels();
            return Serialize(apiResourceModels);
        }
        public abstract TIdentityResource Serialize(ICollection<IdentityResourceModel> identityResources);
        public abstract TApiResources Serialize(ICollection<ApiResourceModel> apiResources);

        public async Task<global::IdentityServer4.Models.Resources> MakeResourcesAsync()
        {
            var apiResources = await DeserializeApiResourcesAsync(ApiResources);

            var identityResources = await DeserializeIdentityResourcesAsync(IdentityResources);

            var result = new global::IdentityServer4.Models.Resources()
            {
                ApiResources = apiResources.ToApiResources(),
                IdentityResources = identityResources.ToIdentityResources(),
                OfflineAccess = OfflineAccess
            };
            return await Task.FromResult(result);
        }

        protected abstract Task<List<IdentityResourceModel>> DeserializeIdentityResourcesAsync(TIdentityResource obj);

        protected abstract Task<List<ApiResourceModel>> DeserializeApiResourcesAsync(TApiResources obj);

        /*ICollection<ApiResource>*/
        public TApiResources ApiResources { get; set; }
        /*ICollection<IdentityResource>*/
        public TIdentityResource IdentityResources { get; set; }
        public bool OfflineAccess { get; set; }

    }
}