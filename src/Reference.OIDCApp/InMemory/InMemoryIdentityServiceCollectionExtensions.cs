using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

namespace Reference.OIDCApp.InMemory
{
    public static class InMemoryIdentityServiceCollectionExtensions
    {
        public static IServiceCollection AddAuthentication<TUser>(this IServiceCollection services, IConfiguration configuration)
            where TUser : class => services.AddAuthentication<TUser>(configuration, null);

        public static IServiceCollection AddAuthentication<TUser>(this IServiceCollection services, IConfiguration configuration, Action<IdentityOptions> setupAction)
            where TUser : class
        {
           
            // Services used by identity
            var authenticationBuilder = services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            });
            /*
                .AddCookie(IdentityConstants.ApplicationScheme, o =>
                {
                    o.LoginPath = new PathString("/Account/Login");
                    o.Events = new CookieAuthenticationEvents
                    {
                        OnValidatePrincipal = SecurityStampValidator.ValidatePrincipalAsync
                    };
                })
                .AddCookie(IdentityConstants.ExternalScheme, o =>
                {
                    o.Cookie.Name = IdentityConstants.ExternalScheme;
                    o.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                })
                .AddCookie(IdentityConstants.TwoFactorRememberMeScheme,
                    o => o.Cookie.Name = IdentityConstants.TwoFactorRememberMeScheme)
                .AddCookie(IdentityConstants.TwoFactorUserIdScheme, o =>
                {
                    o.Cookie.Name = IdentityConstants.TwoFactorUserIdScheme;
                    o.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                });
           */     
            if (!(string.IsNullOrEmpty(configuration["Google-ClientId"]) ||
                  string.IsNullOrEmpty(configuration["Google-ClientSecret"])))
            {
                authenticationBuilder.AddOpenIdConnect(GoogleDefaults.AuthenticationScheme, GoogleDefaults.DisplayName,
                    o =>
                    {
                        var openIdConnectOptions = new GoogleOpenIdConnectOptions();
                        o.CallbackPath = configuration["oauth2:google:callbackPath"];

                        o.ClientId = configuration["Google-ClientId"];
                        o.ClientSecret = configuration["Google-ClientSecret"];

                        o.Authority = configuration["oauth2:google:authority"];
                        o.ResponseType = openIdConnectOptions.ResponseType;
                        o.GetClaimsFromUserInfoEndpoint = openIdConnectOptions.GetClaimsFromUserInfoEndpoint;
                        o.SaveTokens = openIdConnectOptions.SaveTokens;

                        o.Events = new OpenIdConnectEvents()
                        {
                            OnMessageReceived = (context) =>
                            {
                                if (context.ProtocolMessage.Error != null)
                                {
                                    context.Response.Redirect($"/account/OIDCIFrameResult?error={context.ProtocolMessage.Error}");
                                    context.HandleResponse();
                                }
                                return Task.FromResult(0);

                            },
                            OnAuthenticationFailed = (context) =>
                            {
                                return Task.FromResult(0);
                            },
                            OnRedirectToIdentityProvider = (context) =>
                            {
                                if (context.Request.Path != "/Account/ExternalLogin"
                                    && context.Request.Path != "/Account/ExternalLoginWhatIf"
                                    && context.Request.Path != "/Account/PreFlightOIDCAuthorize"
                                    && context.Request.Path != "/Manage/LinkLogin")
                                {
                                    context.Response.Redirect("/account/login");
                                    context.HandleResponse();
                                }

                                var query = from item in context.Request.Query
                                    where string.Compare(item.Key, "prompt", true) == 0
                                    select item.Value;
                                if (query.Any())
                                {
                                    var prompt = query.FirstOrDefault();
                                    context.ProtocolMessage.Prompt = prompt;
                                }
                               
                                return Task.FromResult(0);
                            },
                            OnTicketReceived = (context) =>
                            {
 
                                ClaimsIdentity identity = (ClaimsIdentity)context.Principal.Identity;
                                var query = from claim in context.Principal.Claims
                                    where claim.Type == ClaimTypes.Name || claim.Type == "name"
                                    select claim;
                                var nameClaim = query.FirstOrDefault();
                                var nameIdentifier = identity.FindFirst(ClaimTypes.NameIdentifier);


                                var claimsToKeep =
                                    new List<Claim>
                                    {
                                        nameClaim,
                                        nameIdentifier,
                                        new Claim("DisplayName", nameClaim.Value),
                                        new Claim("UserId", nameIdentifier.Value)
                                    };

                                var newIdentity = new ClaimsIdentity(claimsToKeep, identity.AuthenticationType);

                                context.Principal = new ClaimsPrincipal(newIdentity);
//                                context.Response.Redirect($"/account/SuccessJson");
  //                              context.HandleResponse();
  //                              return Task.FromResult(0);
                                return Task.CompletedTask;

                            },
                            OnUserInformationReceived = (context) =>
                            {
                                return Task.FromResult(0);

                            }

                        };

                    });
            }


            if (!(string.IsNullOrEmpty(configuration["Norton-ClientId"]) ||
                  string.IsNullOrEmpty(configuration["Norton-ClientSecret"])))
            {
                authenticationBuilder.AddOpenIdConnect(NortonDefaults.AuthenticationScheme, NortonDefaults.DisplayName,
                    o =>
                    {
                        var openIdConnectOptions = new NortonOpenIdConnectOptions();
                        o.CallbackPath = configuration["oauth2:norton:callbackPath"];

                        o.ClientId = configuration["Norton-ClientId"];
                        o.ClientSecret = configuration["Norton-ClientSecret"];

                        o.Authority = configuration["oauth2:norton:authority"]; 
                        o.ResponseType = openIdConnectOptions.ResponseType;
                        o.GetClaimsFromUserInfoEndpoint = openIdConnectOptions.GetClaimsFromUserInfoEndpoint;
                        o.SaveTokens = openIdConnectOptions.SaveTokens;
                        o.Scope.Add("offline_access");
                        o.Events = new OpenIdConnectEvents()
                        {
                            OnRedirectToIdentityProvider = (context) =>
                            {

                                var acrValues = context.ProtocolMessage.AcrValues;
                                context.ProtocolMessage.Scope += " open_web_session";
                                if (context.HttpContext.User.Identity.IsAuthenticated)
                                {
                                    // assuming a relogin trigger, so we will make the user relogin on the IDP
                                    context.ProtocolMessage.Prompt = "login";
                                }
                              //  
                                if (context.Request.Path != "/Account/ExternalLogin"
                                    && context.Request.Path != "/Account/ExternalLoginWhatIf"
                                    && context.Request.Path != "/Account/PreFlightOIDCAuthorize"
                                    && context.Request.Path != "/Manage/LinkLogin")
                                {
                                    context.Response.Redirect("/account/login");
                                    context.HandleResponse();
                                }
                                if (context.Request.Path == "/Account/PreFlightOIDCAuthorize")
                                {
                                    context.ProtocolMessage.Prompt = "none";
                                }
                                
                                return Task.FromResult(0);
                            },
                            OnTicketReceived = (context) =>
                            {
 
                                ClaimsIdentity identity = (ClaimsIdentity)context.Principal.Identity;
                                var givenName = identity.FindFirst(ClaimTypes.GivenName);
                                var familyName = identity.FindFirst(ClaimTypes.Surname);
                                var nameIdentifier = identity.FindFirst(ClaimTypes.NameIdentifier);
                                var userId = identity.FindFirst("UserId");

                             
                                var claimsToKeep = new List<Claim> { givenName, familyName, nameIdentifier, userId };
                                claimsToKeep.Add(new Claim("DisplayName",$"{givenName.Value} {familyName.Value}"));
                                var newIdentity = new ClaimsIdentity(claimsToKeep, identity.AuthenticationType);

                                context.Principal = new ClaimsPrincipal(newIdentity);
                                return Task.CompletedTask;
                            }
                        }; 

               
                    });
            }

            /*
            // Hosting doesn't add IHttpContextAccessor by default
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Identity services
            services.TryAddScoped<IUserValidator<TUser>, UserValidator<TUser>>();
            services.TryAddScoped<IPasswordValidator<TUser>, PasswordValidator<TUser>>();
            services.TryAddScoped<IPasswordHasher<TUser>, PasswordHasher<TUser>>();
            services.TryAddScoped<ILookupNormalizer, UpperInvariantLookupNormalizer>();

            // No interface for the error describer so we can add errors without rev'ing the interface
            services.TryAddScoped<IdentityErrorDescriber>();
            services.TryAddScoped<ISecurityStampValidator, SecurityStampValidator<TUser>>();
            services.TryAddScoped<IUserClaimsPrincipalFactory<TUser>, UserClaimsPrincipalFactory<TUser>>();
            services.TryAddScoped<UserManager<TUser>, AspNetUserManager<TUser>>();
            services.TryAddScoped<SignInManager<TUser>, SignInManager<TUser>>();

            if (setupAction != null)
            {
                services.Configure(setupAction);
            }

            return new IdentityBuilder(typeof(TUser), services);
            */
            return services;
        }
    }
}
