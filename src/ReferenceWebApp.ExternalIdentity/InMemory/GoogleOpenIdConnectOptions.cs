using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using P7.Filters;

namespace ReferenceWebApp.InMemory
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

    public class GoogleOpenIdConnectOptions : OpenIdConnectOptions
    {
        /// <summary>
        /// Initializes a new <see cref="GoogleOptions"/>.
        /// </summary>
        public GoogleOpenIdConnectOptions()
        {

            CallbackPath = new PathString("/signin-google");
            Authority = "https://accounts.google.com";

            ResponseType = OpenIdConnectResponseType.Code;
            GetClaimsFromUserInfoEndpoint = true;
            SaveTokens = true;

            Events = new OpenIdConnectEvents()
            {
                OnRedirectToIdentityProvider = (context) =>
                {
                    if (context.Request.Path != "/Account/ExternalLogin")
                    {
                        context.Response.Redirect("/account/login");
                        context.HandleResponse();
                    }

                    return Task.FromResult(0);
                }
            };
            Scope.Add("openid");
            Scope.Add("profile");
            Scope.Add("email");

            ClaimActionCollectionMapExtensions.MapJsonKey(ClaimActions, ClaimTypes.NameIdentifier, "id");
            ClaimActionCollectionMapExtensions.MapJsonKey(ClaimActions, ClaimTypes.Name, "displayName");
            ClaimActionCollectionMapExtensions.MapJsonSubKey(ClaimActions, ClaimTypes.GivenName, "name", "givenName");
            ClaimActionCollectionMapExtensions.MapJsonSubKey(ClaimActions, ClaimTypes.Surname, "name", "familyName");
            ClaimActionCollectionMapExtensions.MapJsonKey(ClaimActions, "urn:google:profile", "url");
            ClaimActionCollectionMapExtensions.MapCustomJson(ClaimActions, ClaimTypes.Email, GoogleHelper.GetEmail);
        }
    }
}