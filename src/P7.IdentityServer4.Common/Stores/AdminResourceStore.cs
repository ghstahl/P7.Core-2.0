using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using P7.Store;

namespace P7.IdentityServer4.Common.Stores
{
    public class AdminResourceStore : IAdminResourceStore
    {
        private readonly IIdentityResourceStore _identityResourceStore;
        private readonly IApiResourceStore _apiResourceStore;

        public AdminResourceStore(IIdentityResourceStore identityResourceStore,
            IApiResourceStore apiResourceStore)
        {
            _identityResourceStore = identityResourceStore;
            _apiResourceStore = apiResourceStore;
        }


        public IIdentityResourceStore IdentityResourceStore
        {
            get { return _identityResourceStore; }
        }

        public IApiResourceStore ApiResourceStore
        {
            get { return _apiResourceStore; }
        }
    }
}
