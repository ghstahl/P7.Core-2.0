using System;
using System.IO;
using Autofac;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using P7.BlogStore.Hugo.Extensions;
using P7.Core;
using P7.Core.Identity;
using P7.Core.Middleware;
using P7.Core.Providers;
using P7.External.SPA.Core;
using P7.Filters;
using P7.GraphQLCore.Stores;
using P7.SimpleRedirect.Core;

namespace WebApplication1
{
    public class AutofacModule : Module
    {
        private static string TenantId = "02a6f1a2-e183-486d-be92-658cd48d6d94";
        protected override void Load(ContainerBuilder builder)
        {
            var env = P7.Core.Global.HostingEnvironment;
            // The generic ILogger<TCategoryName> service was added to the ServiceCollection by ASP.NET Core.
            // It was then registered with Autofac using the Populate method in ConfigureServices.

            builder.Register(c => new InMemorySimpleRedirectStore())
                .As<ISimpleRedirectorStore>()
                .SingleInstance();

            builder.RegisterType<LocalSettingsGlobalPathAuthorizeStore>()
                .As<IGlobalPathAuthorizeStore>()
                .SingleInstance();
            builder.RegisterType<LocalSettingsOptOutOptInAuthorizeStore>()
                .As<IOptOutOptInAuthorizeStore>()
                .SingleInstance();

            builder.RegisterType<OptOutOptInFilterProvider>()
                .As<IFilterProvider>()
                .SingleInstance();

            // register the global configuration root
            builder.RegisterType<GlobalConfigurationRoot>()
                .As<IConfiguration>()
                .SingleInstance();

            // build external InMemoryStore
            var remoteStaticExternalSpaStore = new RemoteStaticExternalSpaStore(
                "https://rawgit.com/ghstahl/P7/master/src/WebApplication5/external.spa.config.json");
            var records = remoteStaticExternalSpaStore.GetRemoteDataAsync().GetAwaiter().GetResult();
            foreach (var spa in records.Spas)
            {
                remoteStaticExternalSpaStore.AddRecord(spa);
            }
            builder.RegisterType<MyPostAuthClaimsProvider>().As<IPostAuthClaimsProvider>().SingleInstance();
            builder.RegisterType<MyAuthApiClaimsProvider>().As<IAuthApiClaimsProvider>().SingleInstance();

            /*
            remoteStaticExternalSpaStore.AddRecord(new ExternalSPARecord()
            {
                Key = "Support",
                RequireAuth = false,
                RenderTemplate = "<div access_token={%{user.access_token}%}>Well Hello Support</div>"
            });
            remoteStaticExternalSpaStore.AddRecord(new ExternalSPARecord()
            {
                Key = "admin",
                RequireAuth = true,
                RenderTemplate = "<div access_token={%{user.access_token}%}>Well Hello Admin</div>"
            });
            */

            builder.Register(c => remoteStaticExternalSpaStore)
                .As<IExternalSPAStore>()
                .SingleInstance();

            builder.RegisterType<InMemoryGraphQLFieldAuthority>()
                .As<IGraphQLFieldAuthority>()
                .SingleInstance();

            var dbPath = Path.Combine(env.ContentRootPath, "App_Data/blogstore");
            Directory.CreateDirectory(dbPath);
            builder.AddBlogStoreBiggyConfiguration(dbPath, TenantId);

        }
    }
}
