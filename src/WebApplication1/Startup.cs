using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using GraphQL.Language.AST;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using P7.Core;
using P7.Core.FileProviders;
using P7.Core.Identity;
using P7.Core.IoC;
using P7.Core.Middleware;
using P7.Core.Scheduler.Scheduling;
using P7.Core.Startup;
using P7.Core.TagHelpers;
using P7.Filters;
using P7.GraphQLCore.Stores;
using P7.Razor.FileProvider;
using P7.RazorProvider.Store.Core;
using P7.RazorProvider.Store.Core.Interfaces;
using P7.TwitterAuthentication;
using Serilog;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1
{
    // this gates all apis with not only being authenticated, but have one of the following claims.
    class MyAuthApiClaimsProvider : IAuthApiClaimsProvider
    {
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

    // this seeds all local identities with a claim {client_id:local}
    // this is so that downstream api filters can let identites of this type in.
    // we let in bearer tokens from external systems that we require to have certain claims, in our case client_id.
    class MyPostAuthClaimsProvider : IPostAuthClaimsProvider
    {
        public async Task<List<Claim>> FetchClaims(ClaimsPrincipal principal)
        {
            var claims = new List<Claim> { new Claim("client_id", MyAuthApiClaimsProvider.LocalClientIdValue) };
            return claims;
        }
    }

    public class Startup
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            _hostingEnvironment = env;
            P7.Core.Global.HostingEnvironment = _hostingEnvironment;
            Configuration = configuration;

            var appDataPath = Path.Combine(env.ContentRootPath, "App_Data");

            var RollingPath = Path.Combine(env.ContentRootPath, "logs/myapp-{Date}.txt");
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.RollingFile(RollingPath)
                .WriteTo.LiterateConsole()
                .CreateLogger();
            Log.Information("Ah, there you are!");

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings-filters.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings-filters-graphql.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }


            builder.AddEnvironmentVariables();
            Configuration = builder.Build();

            // Initialize the global configuration static
            GlobalConfigurationRoot.Configuration = Configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.TryAddSingleton(typeof(IStringLocalizerFactory), typeof(ResourceManagerStringLocalizerFactory));
            services.AddLocalization();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            services
                .AddScoped
                <Microsoft.AspNetCore.Identity.IUserClaimsPrincipalFactory<ApplicationUser>,
                    AppClaimsPrincipalFactory<ApplicationUser>>();

            services.AddAntiforgery(opts => opts.HeaderName = "X-XSRF-Token");
            services.AddMvc(opts =>
            {
                opts.Filters.AddService(typeof(AngularAntiforgeryCookieResultFilter));
            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            });
            services.AddTransient<AngularAntiforgeryCookieResultFilter>();

            var razorLocationStore = new RemoteRazorLocationStore();
            services.AddSingleton<IRemoteRazorLocationStore>(razorLocationStore);
            services.AddSingleton<IRazorLocationStore>(razorLocationStore);
            services.AddSingleton<RemoteRazorLocationStore>(razorLocationStore);
            services.Configure<RazorViewEngineOptions>(opts =>
                opts.FileProviders.Add(
                    new RazorFileProvider(razorLocationStore)
                )
            );

            services.AddAuthorization();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            services.AddLogging();
            services.AddWebEncoders();
            services.AddCors();

            services.AddDistributedMemoryCache();
            services.AddSession();

            services.AddTransient<ClaimsPrincipal>(
                s => s.GetService<IHttpContextAccessor>().HttpContext.User);

            // If you don't want the cookie to be automatically authenticated and assigned to HttpContext.User, 
            // remove the CookieAuthenticationDefaults.AuthenticationScheme parameter passed to AddAuthentication.
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => {
                    options.LoginPath = "/Account/LogIn";
                    options.LogoutPath = "/Account/LogOff";
                })
                .AddP7Twitter(options =>
                {
                    options.ConsumerKey = "uWkHwFNbklXgsLHYzLfRXcThw";
                    options.ConsumerSecret = "2kyg9WdUiJuU2HeWYJEuvwzaJWoweLadTgG3i0oHI5FeNjD5Iv";
                });

            services.AddAllConfigureServicesRegistrants(Configuration);
            services.AddDependenciesUsingAutofacModules();

            services.AddScheduler((sender, args) =>
            {
                Console.Write(args.Exception.Message);
                args.SetObserved();
            });


            var serviceProvider = services.BuildServiceProvider(Configuration);

            P7.Core.Global.ServiceProvider = serviceProvider;

            return serviceProvider;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            IApplicationLifetime appLifetime)
        {
           // LoadRazorProviderData();
            LoadGraphQLAuthority();

            var supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("en-US"),
                new CultureInfo("en-AU"),
                new CultureInfo("en-GB"),
                new CultureInfo("es-ES"),
                new CultureInfo("ja-JP"),
                new CultureInfo("fr-FR"),
                new CultureInfo("zh"),
                new CultureInfo("zh-CN")
            };
            var options = new RequestLocalizationOptions
            {
                //     RequestCultureProviders = new List<IRequestCultureProvider>(),
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            };
            app.UseRequestLocalization(options);

            ConfigureTagHelperBase(env);

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            // Add Serilog to the logging pipeline
            loggerFactory.AddSerilog();
            // Ensure any buffered events are sent at shutdown
            appLifetime.ApplicationStopped.Register(Log.CloseAndFlush);


           
            app.AddAllConfigureRegistrants();


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
            var contentTypeProvider = new FileExtensionContentTypeProvider();

            contentTypeProvider.Mappings.Add(".tag", "text/plain");

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new CbvPhysicalFileProvider(env.WebRootPath),
                RequestPath = new PathString("/cb-v"),
                ServeUnknownFileTypes = true,
                ContentTypeProvider = contentTypeProvider
            });

            var root = env.ContentRootFileProvider;
            var rewriteOptions = new MutableRewriteOptions()
                .AddIISUrlRewrite(root, "IISUrlRewrite.config");
            P7.Core.Global.ArbitraryObjects.Add("rewrite-optons", (object)rewriteOptions);
            app.UseP7Rewriter((RewriteOptions)P7.Core.Global.ArbitraryObjects["rewrite-optons"]);

            app.UseSession();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute("areaRoute", "{area:exists}/{controller=Admin}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
        private async Task LoadGraphQLAuthority()
        {
            var graphQLFieldAuthority = P7.Core.Global.ServiceProvider.GetServices<IGraphQLFieldAuthority>().FirstOrDefault();

            await graphQLFieldAuthority.AddClaimsAsync(OperationType.Mutation, "/blog", new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier,""),
                new Claim("client_id","resource-owner-client"),
            });
        }
        private async Task LoadRazorProviderData()
        {
            // "https://rawgit.com/ghstahl/P7/master/src/p7.external.spa/Areas/ExtSpa/views.json"

            var store = P7.Core.Global.ServiceProvider.GetServices<RemoteRazorLocationStore>().FirstOrDefault();
            store.LoadRemoteDataAsync("https://rawgit.com/ghstahl/P7/master/src/p7.external.spa/Areas/ExtSpa/views.json").GetAwaiter().GetResult();

            /*
            var now = DateTime.UtcNow;
            await store.InsertAsync(new RazorLocation()
            {
                Location = "/Areas/ExtSPA/Views/Home/Index.cshtml",
                Content =
                    "@using P7.External.SPA.Models \n@model SectionValue \n<div id=\"spaSection\">\n@Model.Value\n</div>",
                LastModified = now,
                LastRequested = now
            });
            */
        }
        private static void ConfigureTagHelperBase(IHostingEnvironment env)
        {
            var version = typeof(Startup).GetTypeInfo()
                .Assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                .InformationalVersion;
            if (env.EnvironmentName == "Development")
            {
                version += "." + Guid.NewGuid().ToString().GetHashCode();
            }
            P7TagHelperBase.Version = version;
        }
    }
}