using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace P7.Norton.AspNetIdentity
{
    public static class NortonOpenIdConnectExtensions
    {
        /*
         .AddNortonOpenIdConnect(   Configuration["Norton-ClientId"],
                                    Configuration["Norton-ClientSecret"]);
         */
        public static AuthenticationBuilder AddNortonOpenIdConnect(this AuthenticationBuilder builder,
            string clientId, string clientSecret)
        {

            builder.AddOpenIdConnect(NortonDefaults.AuthenticationScheme, NortonDefaults.DisplayName, o =>
            {
                var openIdConnectOptions = new NortonOpenIdConnectOptions();
                o.CallbackPath = openIdConnectOptions.CallbackPath;

                o.ClientId = clientId;
                o.ClientSecret = clientSecret;

                o.Authority = openIdConnectOptions.Authority;
                o.ResponseType = openIdConnectOptions.ResponseType;
                o.GetClaimsFromUserInfoEndpoint = openIdConnectOptions.GetClaimsFromUserInfoEndpoint;
                o.SaveTokens = openIdConnectOptions.SaveTokens;

                o.Events = new OpenIdConnectEvents()
                {
                    OnRedirectToIdentityProvider = (context) =>
                    {
                        if (context.Request.Path != "/Account/ExternalLogin"
                            && context.Request.Path != "/Manage/LinkLogin")
                        {
                            context.Response.Redirect("/account/login");
                            context.HandleResponse();
                        }

                        return Task.FromResult(0);
                    }
                };
            });
            return builder;
        }
    }

    public class NortonOpenIdConnectOptions : OpenIdConnectOptions
    {
        /// <summary>
        /// Initializes a new <see cref="NortonOpenIdConnectOptions"/>.
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
        }
    }
}
