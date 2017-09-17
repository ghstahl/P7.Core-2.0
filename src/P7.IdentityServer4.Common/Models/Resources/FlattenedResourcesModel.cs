using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;

namespace P7.IdentityServer4.Common
{
    public class FlattenedResourcesModel :
        AbstractResourcesModel<string, string>
    {
        public FlattenedResourcesModel()
            : base()
        {
        }

        public FlattenedResourcesModel(global::IdentityServer4.Models.Resources resources) : base(resources)
        {
        }

        public override string Serialize(ICollection<IdentityResourceModel> identityResources)
        {
            if (identityResources == null)
                return "[]";
            var simpleDocument = new SimpleJsonJsonDocument<List<IdentityResourceModel>>(identityResources.ToList()).DocumentJson;
            return simpleDocument;
        }

        public override string Serialize(ICollection<ApiResourceModel> apiResources)
        {
            if (apiResources == null)
                return "[]";
            var simpleDocument = new SimpleJsonJsonDocument<List<ApiResourceModel>>(apiResources.ToList()).DocumentJson;
            return simpleDocument;
        }

        protected override async Task<List<IdentityResourceModel>> DeserializeIdentityResourcesAsync(string obj)
        {
            obj = string.IsNullOrEmpty(obj) ? "[]" : obj;
            var simpleDocument = new SimpleJsonJsonDocument<List<IdentityResourceModel>>(obj);
            var document = (List<IdentityResourceModel>)simpleDocument.Document;
            return document;
        }

        protected override async Task<List<ApiResourceModel>> DeserializeApiResourcesAsync(string obj)
        {
            obj = string.IsNullOrEmpty(obj) ? "[]" : obj;
            var simpleDocument = new SimpleJsonJsonDocument<List<ApiResourceModel>>(obj);
            var document = (List<ApiResourceModel>)simpleDocument.Document;
            return document;
        }
    }
}