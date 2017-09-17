using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;

namespace P7.IdentityServer4.Common
{
    public abstract class AbstractIdentityResourceModel<TStrings> : IIdentityResourceModel
        where TStrings : class
    {

        public AbstractIdentityResourceModel()
        {
        }

        public AbstractIdentityResourceModel(global::IdentityServer4.Models.IdentityResource identityResource)
        {
            Description = identityResource.Description;
            DisplayName = identityResource.DisplayName;
            Emphasize = identityResource.Emphasize;
            Enabled = identityResource.Enabled;
            Name = identityResource.Name;
            Required = identityResource.Required;
            ShowInDiscoveryDocument = identityResource.ShowInDiscoveryDocument;
            UserClaims = Serialize(identityResource.UserClaims);

        }

        internal abstract TStrings Serialize(ICollection<string> userClaims);

        public async Task<IdentityResource> MakeIdentityResourceAsync()
        {

            var userClaims = await DeserializeUserClaimsAsync(UserClaims);

            var result = new global::IdentityServer4.Models.IdentityResource()
            {
                Name = Name,
                DisplayName = DisplayName,
                Description = Description,
                Enabled = Enabled,
                UserClaims = userClaims,
                Emphasize = Emphasize,
                Required = Required,
                ShowInDiscoveryDocument = ShowInDiscoveryDocument
            };
            return await Task.FromResult(result);
        }
       

        protected abstract Task<List<string>> DeserializeUserClaimsAsync(TStrings userClaims);


        public string Description { get; set; }
        public string DisplayName { get; set; }
        public bool Emphasize { get; set; }
        public bool Enabled { get; set; }
        public string Name { get; set; }
        public bool Required { get; set; }
        public bool ShowInDiscoveryDocument { get; set; }
        public TStrings UserClaims { get; set; }

    }
}