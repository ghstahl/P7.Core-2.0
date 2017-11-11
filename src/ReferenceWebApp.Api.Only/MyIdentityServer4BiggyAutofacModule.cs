using System.IO;
using Autofac;
using P7.BlogStore.Hugo.Extensions;
using P7.GraphQLCore.Stores;
 

namespace ReferenceWebApp
{
    public class MyIdentityServer4BiggyAutofacModule : Module
    {
        private static string TenantId = "02a6f1a2-e183-486d-be92-658cd48d6d94";

        protected override void Load(ContainerBuilder builder)
        {
            var env = P7.Core.Global.HostingEnvironment;
            string dbPath;
            
            dbPath = Path.Combine(env.ContentRootPath, "App_Data/blogstore");
            Directory.CreateDirectory(dbPath);
            builder.AddBlogStoreBiggyConfiguration(dbPath, TenantId);

            /*
            dbPath = Path.Combine(env.ContentRootPath, "App_Data/razorlocationstore");
            Directory.CreateDirectory(dbPath);
            builder.AddRazorLocationStoreBiggyConfiguration(dbPath, TenantId);
            */


        }
    }
}