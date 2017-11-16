using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using A.OIDC.Data;
using A.OIDC.Models;
using A.OIDC.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;
 

namespace A.OIDC
{
    public static class SessionExtensions
    {
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) :
                JsonConvert.DeserializeObject<T>(value);
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
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedMemoryCache();
            services.AddSession();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            var authenticationBuilder = services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            });
            var configuration = Configuration;

            if (!(string.IsNullOrEmpty(configuration["Google-ClientId"]) ||
                  string.IsNullOrEmpty(configuration["Google-ClientSecret"])))
            {
                authenticationBuilder.AddOpenIdConnect(GoogleDefaults.AuthenticationScheme, GoogleDefaults.DisplayName,
                    o =>
                    {
                        var openIdConnectOptions = new GoogleOpenIdConnectOptions();
                        o.CallbackPath = openIdConnectOptions.CallbackPath;

                        o.ClientId = configuration["Google-ClientId"];
                        o.ClientSecret = configuration["Google-ClientSecret"];

                        o.Authority = openIdConnectOptions.Authority;
                        o.ResponseType = openIdConnectOptions.ResponseType;
                        o.GetClaimsFromUserInfoEndpoint = openIdConnectOptions.GetClaimsFromUserInfoEndpoint;
                        o.SaveTokens = true;
                       // o.Scope.Add("offline_access"); 
                        o.Events = new OpenIdConnectEvents()
                        {
                            OnRedirectToIdentityProvider = (context) =>
                            {
                                if (context.Request.Path != "/Account/ExternalLogin"
                                    && context.Request.Path != "/Account/ExternalLoginWhatIf"
                                    && context.Request.Path != "/Manage/LinkLogin")
                                {
                                    context.Response.Redirect("/account/login");
                                    context.HandleResponse();
                                }

                                return Task.FromResult(0);
                            },
                            OnTicketReceived = (context) =>
                            {
                                ISession Session = context.HttpContext.Session;
                                var query1 = from item in context.Properties.Items
                                    where item.Key.StartsWith(".Token.")
                                    select item;
                                var query2 = from item in context.Properties.Items
                                    where !item.Key.StartsWith(".Token.")
                                    select item;
                                var ap = new AuthenticationProperties( );
                                foreach (var a in query2.Where(a => a.Key != ".TokenNames"))
                                {
                                    ap.Items.Add(a.Key, a.Value);
                                }
                                context.Properties = ap;

                                var oidc = new Dictionary<string,string>();
                                foreach (var a in query1)
                                {
                                    var keys = a.Key.Split('.');
                                    oidc.Add(keys[2], a.Value);
                                }
                                Session.SetObject(".oidc",oidc);

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
                                return Task.CompletedTask;

                            }
                        };

                    });
            }
            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSession();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
