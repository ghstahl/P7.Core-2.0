using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace ReferenceWebApp.InMemory
{
    /*
        "Norton-ClientId": {blah},
        "Norton-ClientSecret": {blah},
  */
    public class NortonOpenIdConnectOptions : OpenIdConnectOptions
    {
        /// <summary>
        /// Initializes a new <see cref="GoogleOptions"/>.
        /// </summary>
        public NortonOpenIdConnectOptions()
        {
            CallbackPath = new PathString("/signin-norton");
            Authority = NortonDefaults.Development.Authority;

            ResponseType = OpenIdConnectResponseType.Code;
            GetClaimsFromUserInfoEndpoint = true;
            SaveTokens = true;

            Events = new OpenIdConnectEvents()
            {
                OnRedirectToIdentityProvider = (context) =>
                {
                    if (context.Request.Path != "/Account/ExternalLogin")
                    {
                        context.Response.Redirect("/Account/login");
                        context.HandleResponse();
                    }

                    return Task.FromResult(0);
                }
            };
            Scope.Add("openid");
            Scope.Add("profile");
            Scope.Add("email");

        }
    }
    public class NortonOpenIdConnectUnsecuredOptions : NortonOpenIdConnectOptions
    {
        /// <summary>
        /// Initializes a new <see cref="NortonOpenIdConnectUnsecuredOptions"/>.
        /// </summary>
        public NortonOpenIdConnectUnsecuredOptions()
        {
            CallbackPath = new PathString("/signin-norton-unsecured");
        }
    }
}