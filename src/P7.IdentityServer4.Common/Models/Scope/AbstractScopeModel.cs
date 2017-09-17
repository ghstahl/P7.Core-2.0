using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;

namespace P7.IdentityServer4.Common
{
    public abstract class AbstractScopeModel<TStrings> : IScopeModel
        where TStrings : class
    {

        public AbstractScopeModel()
        {
        }

        public AbstractScopeModel(global::IdentityServer4.Models.Scope scope)
        {
            Description = scope.Description;
            DisplayName = scope.DisplayName;
            Emphasize = scope.Emphasize;
            Name = scope.Name;
            Required = scope.Required;
            ShowInDiscoveryDocument = scope.ShowInDiscoveryDocument;
            UserClaims = Serialize(scope.UserClaims);
        }

        internal abstract TStrings Serialize(ICollection<string> userClaims);

        public async Task<Scope> MakeScopeAsync()
        {

            var userClaims = await DeserializeUserClaimsAsync(UserClaims);

            var result = new global::IdentityServer4.Models.Scope()
            {
                Name = Name,
                DisplayName = DisplayName,
                Description = Description,
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
        public string Name { get; set; }
        public bool Required { get; set; }
        public bool ShowInDiscoveryDocument { get; set; }
        public TStrings UserClaims { get; set; }
    
    }
}