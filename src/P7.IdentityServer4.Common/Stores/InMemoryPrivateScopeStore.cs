using P7.IdentityServer4.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace P7.IdentityServer4.Common.Stores
{
    public class InMemoryPrivateScopeStore : IPrivateScopeValidation
    {
        public bool ValidateArbitraryScopes(string clientId, string[] arbitraryScopes)
        {
            // brute force here, if any other client contains a private entry from arbitraryScopes, it is denied.
            // basically in production the first one to claim a private scope wins.
            foreach (var clientDict in Records)
            {
                if (clientDict.Key != clientId)
                {
                    if (arbitraryScopes.Any(arbScope => clientDict.Value.ContainsKey(arbScope)))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        Dictionary<string, Dictionary<string, bool>> _records;
        Dictionary<string, Dictionary<string, bool>> Records
        {
            get
            {
                if (_records == null)
                {
                    _records = new Dictionary<string, Dictionary<string, bool>>();
                };
                return _records;
            }
        }
        public void AddPrivateScopes(string clientId, string[] scopes)
        {
            if (!Records.ContainsKey(clientId))
            {
                Records.Add(clientId, new Dictionary<string, bool>());
            }
            var clientDict = Records[clientId];
            foreach(var scope in scopes)
            {
                clientDict.Add(scope, true);
            }
        }
    }
}
