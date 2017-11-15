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


namespace A.OIDC
{

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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            var authenticationBuilder = services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie();
            /*
            var authenticationBuilder = services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(o => o.LoginPath = new PathString("/Account/Login"));
            */
            var configuration = Configuration;

            if (!(string.IsNullOrEmpty(configuration["Google-ClientId"]) ||
                  string.IsNullOrEmpty(configuration["Google-ClientSecret"])))
            {
                authenticationBuilder.AddOpenIdConnect(GoogleDefaults.AuthenticationScheme, GoogleDefaults.DisplayName,
                    o =>
                    {
                     
                        o.CallbackPath = new PathString("/signin-google"); 

                        o.ClientId = configuration["Google-ClientId"];
                        o.ClientSecret = configuration["Google-ClientSecret"];

                        o.Authority = "https://accounts.google.com";
                        o.ResponseType = OpenIdConnectResponseType.Code;
                        o.GetClaimsFromUserInfoEndpoint = true;
                        o.SaveTokens = true;

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

                               // context.Principal = new ClaimsPrincipal(newIdentity);
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
