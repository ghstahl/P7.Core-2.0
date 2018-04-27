using System.Collections.Generic;
using System.Threading.Tasks;
using P7.IdentityServer4.Common.Models.oidc;

namespace P7.IdentityServer4.AspNetIdentity.Stores
{
    public interface IIdentityServerTokenStore
    {
        /*
         * 
            https://localhost:44311/connect/token POST
            grant_type=password&scope=arbitrary offline_access
            &client_id=resource-owner-client
            &client_secret=secret
            &handler=arbitrary_claims_service
            &arbitrary_claims={"naguid":"1234abcd","In":"Flames"}
            &username=rat&password=poison
            &arbitrary_scopes=A quick brown fox
*/
        Task<OIDCRecord> FetchArbitraryResourceOwnerTokens(string clientId, 
            string clientSecret, 
            Dictionary<string,string> arbitraryClaims
            ,string userName, string[] arbitraryScopes);

    }
}