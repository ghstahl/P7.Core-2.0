using System.Collections.Generic;
using System.Linq;
using P7.IdentityServer4.Common.Services;

namespace P7.IdentityServer4.Common.Stores
{
    public class InMemoryClientNamespaceValidationStore : IClientNamespaceValidation
    {
        private Dictionary<string, Dictionary<string, bool>> _namespaceRecords;
        private Dictionary<string, Dictionary<string, bool>> NamespaceRecords => _namespaceRecords ??
                                                                                 (_namespaceRecords =
                                                                                     new Dictionary<string, Dictionary<string, bool>>());
        public void AddClientNamespaces(string clientId, string[] namespaces)
        {
            if (!NamespaceRecords.ContainsKey(clientId))
            {

                NamespaceRecords.Add(clientId, new Dictionary<string, bool>());
            }
            var dict = NamespaceRecords[clientId];
            foreach (var item in namespaces)
            {
                dict.Add(item.ToLower(), true);
            }
        }
        public bool ValidateClientNamespace(string clientId, string[] namespaces)
        {
            // brute force here, if any other client contains a private entry from arbitraryScopes, it is denied.
            // basically in production the first one to claim a private scope wins.
            foreach (var dict in NamespaceRecords)
            {
                if (dict.Key == clientId)
                {
                    var result = namespaces.All(key => dict.Value.ContainsKey(key.ToLower()));
                    return result;
                }
            }
            return false;
        }
    }
}