using System.IO;
using Autofac;
using P7.BlogStore.Hugo.Extensions;
using P7.Core.Identity;
using P7.Filters;
using P7.GraphQLCore.Stores;
using P7.IdentityServer4.BiggyStore.Extensions;

namespace WebApplication1
{
    public class MyIdentityServer4BiggyAutofacModule : Module
    {
        private static string TenantId = "02a6f1a2-e183-486d-be92-658cd48d6d94";

        protected override void Load(ContainerBuilder builder)
        {
            var env = P7.Core.Global.HostingEnvironment;
            var dbPath = Path.Combine(env.ContentRootPath, "App_Data/identityserver4");
            Directory.CreateDirectory(dbPath);
            builder.AddIdentityServer4BiggyConfiguration(dbPath);

            dbPath = Path.Combine(env.ContentRootPath, "App_Data/blogstore");
            Directory.CreateDirectory(dbPath);
            builder.AddBlogStoreBiggyConfiguration(dbPath, TenantId);

            /*
            dbPath = Path.Combine(env.ContentRootPath, "App_Data/razorlocationstore");
            Directory.CreateDirectory(dbPath);
            builder.AddRazorLocationStoreBiggyConfiguration(dbPath, TenantId);
            */
            builder.RegisterType<InMemoryGraphQLFieldAuthority>()
                .As<IGraphQLFieldAuthority>()
                .SingleInstance();

            builder.RegisterType<MyPostAuthClaimsProvider>().As<IPostAuthClaimsProvider>().SingleInstance();
            builder.RegisterType<MyAuthApiClaimsProvider>().As<IAuthApiClaimsProvider>().SingleInstance();

        }
    }
}