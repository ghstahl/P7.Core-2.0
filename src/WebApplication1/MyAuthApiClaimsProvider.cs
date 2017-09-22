using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using P7.Filters;

namespace WebApplication1
{
    class MyAuthApiClaimsProvider : IAuthApiClaimsProvider
    {
        // this gates all apis with not only being authenticated, but have one of the following claims.
        public static string LocalClientIdValue => "local";
        public async Task<List<Claim>> FetchClaimsAsync()
        {
            var claims = new List<Claim>
            {
                new Claim("client_id", MyAuthApiClaimsProvider.LocalClientIdValue),
                new Claim("client_id", "resource-owner-client")
            };
            return claims;
        }
    }
}