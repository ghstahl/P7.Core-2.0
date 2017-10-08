using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.AspNetIdentity;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentityServerIdentityBuilderExtensions
    {
        public static IdentityBuilder AddIdentityServer(this IdentityBuilder builder)
        {
            

            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserIdClaimType = JwtClaimTypes.Subject;
                options.ClaimsIdentity.UserNameClaimType = JwtClaimTypes.Name;
                options.ClaimsIdentity.RoleClaimType = JwtClaimTypes.Role;
            });

            builder.Services.Configure<SecurityStampValidatorOptions>(opts =>
            {
                opts.OnRefreshingPrincipal = SecurityStampValidatorCallback.UpdatePrincipal;
            });

            builder.Services.Configure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme, cookie =>
            {
                // we need to disable to allow iframe for authorize requests
                cookie.Cookie.SameSite = AspNetCore.Http.SameSiteMode.None;
            });

            return builder;
        }
    }
}
