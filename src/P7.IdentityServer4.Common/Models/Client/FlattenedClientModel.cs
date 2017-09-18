using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Models;

namespace P7.IdentityServer4.Common
{
    public class FlattenedClientModel :
        AbstractClientModel<
            string,
            string,
            string,
            string
        >
    {
        public FlattenedClientModel()
            : base()
        {
        }

        public FlattenedClientModel(Client client) : base(client)
        {
        }

        public override string Serialize(Dictionary<string, string> dict)
        {
            if (dict == null)
                return "{}";
            var simpleDocument = new SimpleJsonJsonDocument<Dictionary<string, string>>(dict).DocumentJson;
            return simpleDocument;
        }

        public override string Serialize(List<string> stringList)
        {
            if (stringList == null)
                return "[]";
            var simpleDocument = new SimpleJsonJsonDocument<List<string>>(stringList).DocumentJson;
            return simpleDocument;
        }

        public override async Task<List<string>> DeserializeStringsAsync(string obj)
        {
            obj = string.IsNullOrEmpty(obj) ? "[]" : obj;
            var simpleDocument = new SimpleJsonJsonDocument<List<string>>(obj);
            var document = (List<string>) simpleDocument.Document;
            return await Task.FromResult(document);
        }
        public override async Task<IDictionary<string, string>> DeserializeStringDictionaryAsync(string obj)
        {
            obj = string.IsNullOrEmpty(obj) ? "{}" : obj;
            var simpleDocument = new SimpleJsonJsonDocument<Dictionary<string, string>>(obj);
            var document = (Dictionary<string,string>)simpleDocument.Document;
            return await Task.FromResult(document);
        }

        public override string Serialize(List<Claim> claims)
        {
            var normalized = claims == null ? null : claims.ToClaimTypeRecords();
            if (normalized == null)
                return "[]";
            var simpleDocument = new SimpleJsonJsonDocument<List<ClaimModel>>(normalized).DocumentJson;
            return simpleDocument;
        }

        public override async Task<List<Claim>> DeserializeClaimsAsync(string obj)
        {
            obj = string.IsNullOrEmpty(obj) ? "[]" : obj;
            var simpleDocument = new SimpleJsonJsonDocument<List<ClaimModel>>(obj);
            var document = (List<ClaimModel>) simpleDocument.Document;
            var result = document.ToClaims();
            return await Task.FromResult(result);
        }

        public override string Serialize(List<Secret> secrets)
        {
            if (secrets == null)
                return "[]";
            var simpleDocument = new SimpleJsonJsonDocument<List<Secret>>(secrets).DocumentJson;
            return simpleDocument;
        }

        public override async Task<List<Secret>> DeserializeSecretsAsync(string obj)
        {
            obj = string.IsNullOrEmpty(obj) ? "[]" : obj;
            var simpleDocument = new SimpleJsonJsonDocument<List<Secret>>(obj);
            var document = (List<Secret>) simpleDocument.Document;
            return await Task.FromResult(document);
        }
    }
}