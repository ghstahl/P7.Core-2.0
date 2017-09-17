using System.Collections.Generic;
using System.Linq;
using IdentityServer4.Models;

namespace P7.IdentityServer4.Common
{
    public static class ApiResourceExtensions
    {
        public static ApiResource ToApiResource(this ApiResourceModel model)
        {
            return model.MakeApiResourceAsync().Result;
        }
        public static ApiResource ToApiResource(this FlattenedApiResourceModel model)
        {
            return model.MakeApiResourceAsync().Result;
        }

        public static List<ApiResource> ToApiResources(this List<ApiResourceModel> models)
        {
            var query = from model in models
                let c = model.ToApiResource()
                select c;
            return query.ToList();
        }
        public static List<ApiResource> ToApiResources(this List<FlattenedApiResourceModel> models)
        {
            var query = from model in models
                        let c = model.ToApiResource()
                        select c;
            return query.ToList();
        }
        public static ApiResourceModel ToApiResourceModel(this ApiResource model)
        {
            return new ApiResourceModel(model);
        }
        public static FlattenedApiResourceModel ToFlattenedApiResourceModel(this ApiResource model)
        {
            return new FlattenedApiResourceModel(model);
        }
        public static List<ApiResourceModel> ToApiResourceModels(this ICollection<ApiResource> models)
        {
            var query = from model in models
                let c = model.ToApiResourceModel()
                select c;
            return query.ToList();
        }
        public static List<FlattenedApiResourceModel> ToFlattenedApiResourceModels(this ICollection<ApiResource> models)
        {
            var query = from model in models
                        let c = model.ToFlattenedApiResourceModel()
                        select c;
            return query.ToList();
        }
    }
}