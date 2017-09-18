using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace P7.IdentityServer4.Common.Stores
{
    public class DefaultResourcesStore : IResourceStore
    {
        private readonly IIdentityResourceStore _identityResourceStore;
        private readonly IApiResourceStore _apiResourceStore;

        public DefaultResourcesStore(IIdentityResourceStore identityResourceStore,
            IApiResourceStore apiResourceStore)
        {
            _identityResourceStore = identityResourceStore;
            _apiResourceStore = apiResourceStore;
        }

        async Task<IEnumerable<ApiResource>> FetchAllApiResourceAsync()
        {
            List< ApiResource > apiResources = new List<ApiResource>();
            byte[] pagingState = null;
            do
            {
                var page = await _apiResourceStore.PageAsync(100, pagingState);
                apiResources.AddRange(page);
                pagingState = page.PagingState;
            } while (pagingState != null);
            return apiResources;
        }
        async Task<IEnumerable<IdentityResource>> FetchAllIdentityResourceAsync()
        {
            List<IdentityResource> finalList = new List<IdentityResource>();
            byte[] pagingState = null;
            do
            {
                var page = await _identityResourceStore.PageAsync(100, pagingState);
                finalList.AddRange(page);
                pagingState = page.PagingState;
            } while (pagingState != null);
            return finalList;
        }
        public async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> names)
        {
            if (names == null)
                throw new ArgumentNullException(nameof(names));
            var identityResources = await FetchAllIdentityResourceAsync();
            var identity = from i in identityResources
                           where names.Contains(i.Name)
                           select i;

            return identity;
        }

        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> names)
        {
            if (names == null) throw new ArgumentNullException(nameof(names));
            var apiResources = await FetchAllApiResourceAsync();
            var api = from a in apiResources
                      from s in a.Scopes
                      where names.Contains(s.Name)
                      select a;

            return api;
        }

        public async Task<ApiResource> FindApiResourceAsync(string name)
        {
            var apiResources = await FetchAllApiResourceAsync();
            var api = from a in apiResources
                      where a.Name == name
                      select a;
            return api.FirstOrDefault();
        }

        public async Task<Resources> GetAllResourcesAsync()
        {
            var identityResources = await FetchAllIdentityResourceAsync();
            var apiResources = await FetchAllApiResourceAsync();
            var result = new Resources(identityResources, apiResources);
            return result;
        }

    }
}
