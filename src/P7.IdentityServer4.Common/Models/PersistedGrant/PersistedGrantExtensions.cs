using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;


namespace P7.IdentityServer4.Common
{
    public static class PersistedGrantExtensions
    {
        public static global::IdentityServer4.Models.PersistedGrant ToPersistedGrant(this PersistedGrantModel model)
        {
            var result = model.ToPersistedGrantAsync();
            return result.Result;
        }

        public static async Task<global::IdentityServer4.Models.PersistedGrant> ToPersistedGrantAsync(
            this PersistedGrantModel model)
        {
            var result = await model.MakePersistedGrantAsync();
            return result;
        }

        public static List<global::IdentityServer4.Models.PersistedGrant> ToPersistedGrants(
            this List<PersistedGrantModel> models)
        {
            var result = models.ToPersistedGrantAsync();
            return result.Result;

        }


        public static async Task<List<global::IdentityServer4.Models.PersistedGrant>> ToPersistedGrantAsync(
            this List<PersistedGrantModel> models)
        {
            var queryResults = from item in models
                               let c = item.ToPersistedGrant()
                               select c;
            return queryResults.ToList();
        }

        public static PersistedGrantModel ToPersistedGrantModel(this global::IdentityServer4.Models.PersistedGrant model)
        {
            var result = model.ToPersistedGrantModelAsync();
            return result.Result;
        }
        public static async Task<PersistedGrantModel> ToPersistedGrantModelAsync(this global::IdentityServer4.Models.PersistedGrant model)
        {
            return new PersistedGrantModel(model);
        }

        public static List<PersistedGrantModel> ToPersistedGrantModels(
            this List<global::IdentityServer4.Models.PersistedGrant> models)
        {
            var result = models.ToPersistedGrantModelsAsync();
            return result.Result;

        }


        public static async Task<List<PersistedGrantModel>> ToPersistedGrantModelsAsync(this List<global::IdentityServer4.Models.PersistedGrant> models)
        {
            var queryResults = from item in models
                               let c = item.ToPersistedGrantModel()
                               select c;
            return queryResults.ToList();
        }

    }
}
