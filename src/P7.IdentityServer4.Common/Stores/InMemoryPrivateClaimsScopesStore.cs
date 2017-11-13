using P7.IdentityServer4.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace P7.IdentityServer4.Common.Stores
{
    public class InMemoryPrivateClaimsScopesStore : IPrivateClaimsScopesValidation
    {
        Dictionary<string, Dictionary<string, bool>> _claimRecords;
        Dictionary<string, Dictionary<string, bool>> _scopeRecords;
        Dictionary<string, Dictionary<string, bool>> ClaimRecords
        {
            get
            {
                if (_claimRecords == null)
                {
                    _claimRecords = new Dictionary<string, Dictionary<string, bool>>();
                };
                return _claimRecords;
            }
        }
        Dictionary<string, Dictionary<string, bool>> ScopeRecords
        {
            get
            {
                if (_scopeRecords == null)
                {
                    _scopeRecords = new Dictionary<string, Dictionary<string, bool>>();
                };
                return _scopeRecords;
            }
        }

        public void AddPrivateScopes(string clientId, string[] scopes)
        {
            if (!ScopeRecords.ContainsKey(clientId))
            {
                ScopeRecords.Add(clientId, new Dictionary<string, bool>());
            }
            var clientDict = ScopeRecords[clientId];
            foreach (var item in scopes)
            {
                clientDict.Add(item.ToLower(), true);
            }
        }
        public void AddPrivateClaims(string clientId, string[] claims)
        {
            if (!ClaimRecords.ContainsKey(clientId))
            {
                ClaimRecords.Add(clientId, new Dictionary<string, bool>());
            }
            var clientDict = ClaimRecords[clientId];
            foreach (var item in claims)
            {
                clientDict.Add(item.ToLower(), true);
            }
        }
        public bool ValidatePrivateArbitraryScopes(string clientId, string[] arbitraryScopes)
        {
            // brute force here, if any other client contains a private entry from arbitraryScopes, it is denied.
            // basically in production the first one to claim a private scope wins.
            foreach (var clientDict in ScopeRecords)
            {
                if (clientDict.Key != clientId)
                {
                    if (arbitraryScopes.Any(a => clientDict.Value.ContainsKey(a.ToLower())))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool ValidatePrivateArbitraryClaims(string clientId, string[] arbitraryClaims)
        {
            foreach (var clientDict in ClaimRecords)
            {
                if (clientDict.Key != clientId)
                {
                    if (arbitraryClaims.Any(a => clientDict.Value.ContainsKey(a.ToLower())))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
