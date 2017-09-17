using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;

namespace P7.IdentityServer4.Common
{
    public abstract class AbstractApiResourceModel<TSecrets, TScopeModels, TStrings> : IApiResourceModel
        where TSecrets : class
        where TScopeModels : class
        where TStrings : class
    {

        public AbstractApiResourceModel()
        {
        }

        public AbstractApiResourceModel(global::IdentityServer4.Models.ApiResource apiResource)
        {
            ApiSecrets = Serialize(apiResource.ApiSecrets);
            Description = apiResource.Description;
            DisplayName = apiResource.DisplayName;
            Enabled = apiResource.Enabled;
            Name = apiResource.Name;
            Scopes = Serialize(apiResource.Scopes);
            UserClaims = Serialize(apiResource.UserClaims);

        }

        internal abstract TStrings Serialize(ICollection<string> userClaims);
        internal abstract TScopeModels Serialize(ICollection<Scope> scopes);
        public abstract TSecrets Serialize(ICollection<Secret> apiSecrets);

        public async Task<global::IdentityServer4.Models.ApiResource> MakeApiResourceAsync()
        {
            var apiSecrets = await DeserializeApiSecretsAsync(ApiSecrets);
            var scopes = await DeserializeScopesAsync(Scopes);
            var userClaims = await DeserializeUserClaimsAsync(UserClaims);

            var result = new global::IdentityServer4.Models.ApiResource()
            {
                ApiSecrets = apiSecrets,
                Name = Name,
                DisplayName = DisplayName,
                Description = Description,
                Enabled = Enabled,
                Scopes = scopes.ToScopes(),
                UserClaims = userClaims
            };
            return await Task.FromResult(result);
        }

        protected abstract Task<List<string>> DeserializeUserClaimsAsync(TStrings userClaims);

        protected abstract Task<List<ScopeModel>> DeserializeScopesAsync(TScopeModels scopes);

        protected abstract Task<List<Secret>> DeserializeApiSecretsAsync(TSecrets apiSecrets);


        public TSecrets ApiSecrets { get; set; }
        public string Description { get; set; }
        public string DisplayName { get; set; }
        public bool Enabled { get; set; }
        public string Name { get; set; }
        public TScopeModels Scopes { get; set; }
        public TStrings UserClaims { get; set; }

    }
}