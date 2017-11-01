using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using P7.Core.Identity;

namespace ReferenceWebApp
{
    class MyPostAuthClaimsProvider : IPostAuthClaimsProvider
    {
        // this seeds all local identities with a claim {client_id:local}
        // this is so that downstream api filters can let identites of this type in.
        // we let in bearer tokens from external systems that we require to have certain claims, in our case client_id.

        public async Task<List<Claim>> FetchClaims(ClaimsPrincipal principal)
        {
            var claims = new List<Claim> { new Claim("client_id", MyAuthApiClaimsProvider.LocalClientIdValue) };
            return claims;
        }
    }
}