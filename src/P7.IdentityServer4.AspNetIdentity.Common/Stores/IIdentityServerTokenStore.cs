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
            &handler=arbitrary-claims-service
            &arbitrary-claims={"naguid":"1234abcd","In":"Flames"}
            &username=rat&password=poison
            &arbitrary-scopes=A quick brown fox
*/
        Task<OIDCRecord> FetchArbitraryResourceOwnerTokens(string clientId, 
            string clientSecret, 
            Dictionary<string,string> arbitraryClaims
            ,string userName, string[] arbitraryScopes);

    }
}