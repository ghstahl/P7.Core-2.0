using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Remotion.Linq.Clauses;

namespace P7.Filters
{
    public interface IAuthApiClaimsProvider
    {
        Task<List<Claim>> FetchClaimsAsync();
    }

    public class AuthApiActionFilter : ActionFilterAttribute
    {
        private static IConfiguration _configuration;
        private IAuthApiClaimsProvider _authApiClaimsProvider;
        private List<Claim> _allowedClaims;

        private List<Claim> AllowedClaims
        {
            get
            {
                if (_authApiClaimsProvider == null)
                    return null;

                return _allowedClaims;
            }
        }

        public AuthApiActionFilter(IConfiguration configuration, IAuthApiClaimsProvider authApiClaimsProvider)
        {
            _configuration = configuration;
            _authApiClaimsProvider = authApiClaimsProvider;
        }

        public AuthApiActionFilter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        static bool ClaimsEqual(Claim one, Claim two)
        {
            return one.Type == two.Type && one.Value == two.Value;
        }

        public override async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            bool unauthorized = false;
            // do something before the action executes
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                unauthorized = true;
            }
            else
            {
                if (_authApiClaimsProvider != null)
                {
                    if (_allowedClaims == null)
                    {
                        _allowedClaims = await _authApiClaimsProvider.FetchClaimsAsync();
                    }
                    if (AllowedClaims.Any())
                    {
                        var identityClaims = context.HttpContext.User.Claims.ToList();
                        /*
                        var query = from allowedClaim in AllowedClaims
                                    from identityClaim in identityClaims
                                    let c1 = new SimpleClaimHandle() { Type = allowedClaim.Type, Value = allowedClaim.Value }
                                    let c2 = new SimpleClaimHandle() { Type = identityClaim.Type, Value = identityClaim.Value }
                                    where c1.Equals(c2)
                                    select c1;
                        var qb = query.Count();
*/
                        var allowedCheck =
                            identityClaims.Where(a => AllowedClaims.Any(x => x.Type == a.Type && x.Value == a.Value));
                        if (!allowedCheck.Any())
                        {
                            context.Result = new UnauthorizedResult();
                            unauthorized = true;
                        }
                    }
                }
            }
            if (!unauthorized)
            {
                await next();
                // do something after the action executes
            }
        }
    }
}