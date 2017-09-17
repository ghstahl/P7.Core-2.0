using System.Collections.Generic;
using System.Linq;
using IdentityServer4.Models;

namespace P7.IdentityServer4.Common
{
    public static class IdentityResourceExtensions
    {
        public static IdentityResource ToIdentityResource(this IdentityResourceModel model)
        {
            return model.MakeIdentityResourceAsync().Result;
        }
        public static List<IdentityResource> ToIdentityResources(this List<IdentityResourceModel> models)
        {
            var query = from model in models
                let c = ToIdentityResource(model)
                select c;
            return query.ToList();
        }

        public static IdentityResourceModel ToIdentityResourceModel(this IdentityResource model)
        {
            return new IdentityResourceModel(model);
        }
        public static List<IdentityResourceModel> ToIdentityResourceModels(this ICollection<IdentityResource> models)
        {
            var query = from model in models
                let c = model.ToIdentityResourceModel()
                select c;
            return query.ToList();
        }
    }
}