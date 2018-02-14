using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ReferenceWebApp.CookieAuthApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Global.HostingEnvironment = env;
            Configuration = configuration;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
            Configuration = builder.Build();
            // Initialize the global configuration static
            GlobalConfigurationRoot.Configuration = Configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            
            services.AddAuthentication(sharedOptions =>
                {
                    sharedOptions.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    // sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.LoginPath = "/Account/LogIn"; ;
                        options.AccessDeniedPath = new PathString("/account/login");
                        options.Cookie.Name = "AUTHCOOKIE";
                        options.ExpireTimeSpan = new TimeSpan(365, 0, 0, 0);
                        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                        options.Cookie.SameSite = SameSiteMode.None;
                        
                    }
                );

            services.AddMemoryCache();
            services.AddSession();
            services.AddMvc();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddDependenciesUsingAutofacModules();

            var serviceProvider = services.BuildServiceProvider(Configuration);

            Global.ServiceProvider = serviceProvider;
            return Global.ServiceProvider;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            var cookiePolicyOptions = new CookiePolicyOptions
            {
                Secure = CookieSecurePolicy.SameAsRequest,
                MinimumSameSitePolicy = SameSiteMode.None
            };

            app.UseCookiePolicy(cookiePolicyOptions);
            app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
