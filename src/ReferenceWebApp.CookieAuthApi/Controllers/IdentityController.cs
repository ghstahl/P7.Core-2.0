using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ReferenceWebApp.CookieAuthApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Identity")]
    public class IdentityController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        [Route("sign-in")]
        public async Task SignIn(string username, string password)
        {

            var properties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddYears(1),
                
                AllowRefresh = true
            };

            var claims = new[] {new Claim("name", username), new Claim(ClaimTypes.Role, "User")};
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity),
                    properties);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("sign-out")]
        public async Task Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("check-authentication")]
        public async Task<bool> CheckAuthentication()
        {
            return User.Identity.IsAuthenticated;
        }
    }
}