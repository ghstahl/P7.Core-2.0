﻿using System.IO;
using Autofac;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using P7.BlogStore.Hugo.Extensions;
using P7.Core;
using P7.Core.Identity;
using P7.Core.Middleware;
using P7.Core.Providers;
using P7.Core.Scheduler;
using P7.Core.Scheduler.Scheduling;
using P7.Core.Scheduler.Stores;
using P7.Filters;
using P7.GraphQLCore;
using P7.GraphQLCore.Stores;
using P7.GraphQLCore.Validators;
using P7.IdentityServer4.Common.Services;
using P7.IdentityServer4.Common.Stores;
using P7.SessionContextStore.Core;
using P7.SimpleRedirect.Core;
using ReferenceWebApp.Scheduler;

namespace ReferenceWebApp
{
    public class AutofacModule : Module
    {
        private static string TenantId = "02a6f1a2-e183-486d-be92-658cd48d6d94";
        protected override void Load(ContainerBuilder builder)
        {
            var env = P7.Core.Global.HostingEnvironment;
            // The generic ILogger<TCategoryName> service was added to the ServiceCollection by ASP.NET Core.
            // It was then registered with Autofac using the Populate method in ConfigureServices.

            builder.RegisterType<InMemoryClientNamespaceValidationStore>()
                .AsSelf()
                .As<IClientNamespaceValidation>()
                .SingleInstance();

            builder.RegisterType<InMemoryPrivateClaimsScopesStore>()
                .AsSelf()
                .As<IPrivateClaimsScopesValidation>()
                .SingleInstance();
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

            builder.RegisterType<MyPostAuthClaimsProvider>().As<IPostAuthClaimsProvider>().SingleInstance();
            builder.RegisterType<MyAuthApiClaimsProvider>().As<IAuthApiClaimsProvider>().SingleInstance();
 
            builder.RegisterType<InMemoryGraphQLFieldAuthority>()
                .As<InMemoryGraphQLFieldAuthority>()
                .As<IGraphQLFieldAuthority>()
                .SingleInstance();

            var dbPath = Path.Combine(env.ContentRootPath, "App_Data/blogstore");
            Directory.CreateDirectory(dbPath);
            builder.AddBlogStoreBiggyConfiguration(dbPath, TenantId);

            builder.RegisterType<QuoteOfTheDayTask>()
                .As<IScheduledTask>()
                .SingleInstance();
            builder.RegisterType<SomeOtherTask>()
                .As<IScheduledTask>()
                .SingleInstance();
            builder.RegisterType<SchedulerHostedService>()
                .As<IHostedService>()
                .SingleInstance();

            builder.RegisterType<QuoteOfTheDataStore>()
                .As<IQuoteOfTheDataStore>()
                .SingleInstance();

            builder.AddInMemoryRemoteSessionContext();

            builder.RegisterType<InMemoryRemoteSessionContextTask>()
                .As<IScheduledTask>()
                .SingleInstance();
        }
    }
}
