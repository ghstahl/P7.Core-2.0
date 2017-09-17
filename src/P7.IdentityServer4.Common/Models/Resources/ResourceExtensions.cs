using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace P7.IdentityServer4.Common
{
    public static class ResourceExtensions
    {
        public static global::IdentityServer4.Models.Resources ToResources(this ResourcesModel model)
        {
            var result = model.ToResourcesAsync();
            return result.Result;
        }
        public static async Task<global::IdentityServer4.Models.Resources> ToResourcesAsync(this ResourcesModel model)
        {
            var result = await model.MakeResourcesAsync();
            return result;
        }
        public static ResourcesModel ToResourcesModel(this global::IdentityServer4.Models.Resources model)
        {
            return new ResourcesModel(model);
        }
    }
}
