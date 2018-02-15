using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using P7.AspNetCore.Identity.InMemory;
using Reference.OIDCApp.Data;
using Reference.OIDCApp.Services;
using Reference.OIDCApp.InMemory;
using Reference.OIDCApp.Middleware;
using Reference.OIDCApp.Pages.Account;

namespace Reference.OIDCApp
{
    public class DeploymentOptions
    {
        public const string WellKnown_SectionName = "deployment";
        [JsonProperty("type")]
        public string Color { get; set; }
        public string Host { get; set; }
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

            services.AddMemoryCache();

            var inMemoryStore = new InMemoryStore<ApplicationUser, ApplicationRole>();

            services.AddSingleton<IUserStore<ApplicationUser>>(provider =>
            {
                return inMemoryStore;
            });
            services.AddSingleton<IUserRoleStore<ApplicationUser>>(provider =>
            {
                return inMemoryStore;
            });
            services.AddSingleton<IRoleStore<ApplicationRole>>(provider =>
            {
                return inMemoryStore;
            });
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddDefaultTokenProviders();

            services.AddAuthentication<ApplicationUser>(Configuration);

            services.RegisterAccountConfigurationServices(Configuration);

            services.AddMvc()
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AuthorizeFolder("/Account/Manage");
                    options.Conventions.AuthorizePage("/Account/Logout");
                }).AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

                });
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });
            services.AddDistributedMemoryCache();
            services.AddSession();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMiddleware<Convert302ResponseMiddleware>();
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            //enable session before MVC
            //=========================  
            var sessionOptions = new SessionOptions
            {
                CookieSecure = CookieSecurePolicy.SameAsRequest
            };
            var deploymentOptions = app.GetService<IOptions<DeploymentOptions>>();
            sessionOptions.Cookie.Name = $".{deploymentOptions.Value.Color}.AspNetCore.Session";
            app.UseSession(sessionOptions);

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
