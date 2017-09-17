using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace P7.Core.Identity
{
    public class AppClaimsPrincipalFactory<T> : UserClaimsPrincipalFactory<T, IdentityRole> where T : class
    {
        public readonly IPostAuthClaimsProvider _postAuthClaimsProvider;

        public AppClaimsPrincipalFactory(
            UserManager<T> userManager
            , RoleManager<IdentityRole> roleManager
            , IOptions<IdentityOptions> optionsAccessor
            , IPostAuthClaimsProvider postAuthClaimsProvider)
            : base(userManager, roleManager, optionsAccessor)
        {
            _postAuthClaimsProvider = postAuthClaimsProvider;
        }

        public async override Task<ClaimsPrincipal> CreateAsync(T user)
        {
            var principal = await base.CreateAsync(user);
            var claims = await _postAuthClaimsProvider.FetchClaims(principal);
            if (claims != null)
            {
                ((ClaimsIdentity)principal.Identity).AddClaims(claims);
            }
            return principal;
        }
    }
}