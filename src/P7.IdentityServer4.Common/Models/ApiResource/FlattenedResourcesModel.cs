using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;

namespace P7.IdentityServer4.Common
{
    public class FlattenedApiResourceModel :
        AbstractApiResourceModel<string, string, string>
    {
        public FlattenedApiResourceModel() { }
        public FlattenedApiResourceModel(ApiResource apiResource) : base(apiResource)
        {
        }

        internal override string Serialize(ICollection<string> userClaims)
        {
            if (userClaims == null)
                return "[]";
            var simpleDocument = new SimpleJsonJsonDocument<List<string>>(userClaims.ToList()).DocumentJson;
            return simpleDocument;
        }

        internal override string Serialize(ICollection<Scope> scopes)
        {
            if (scopes == null)
                return "[]";
            var simpleDocument = new SimpleJsonJsonDocument<List<Scope>>(scopes.ToList()).DocumentJson;
            return simpleDocument;
        }

        public override string Serialize(ICollection<Secret> apiSecrets)
        {
            if (apiSecrets == null)
                return "[]";
            var simpleDocument = new SimpleJsonJsonDocument<List<Secret>>(apiSecrets.ToList()).DocumentJson;
            return simpleDocument;
        }

        protected override async Task<List<string>> DeserializeUserClaimsAsync(string obj)
        {
            obj = string.IsNullOrEmpty(obj) ? "[]" : obj;
            var simpleDocument = new SimpleJsonJsonDocument<List<string>>(obj);
            var document = (List<string>) simpleDocument.Document;
            return document;
        }

        protected override async Task<List<ScopeModel>> DeserializeScopesAsync(string obj)
        {
            obj = string.IsNullOrEmpty(obj) ? "[]" : obj;
            var simpleDocument = new SimpleJsonJsonDocument<List<ScopeModel>>(obj);
            var document = (List<ScopeModel>) simpleDocument.Document;
            return document;
        }

        protected override async Task<List<Secret>> DeserializeApiSecretsAsync(string obj)
        {
            obj = string.IsNullOrEmpty(obj) ? "[]" : obj;
            var simpleDocument = new SimpleJsonJsonDocument<List<Secret>>(obj);
            var document = (List<Secret>) simpleDocument.Document;
            return document;
        }
    }
}