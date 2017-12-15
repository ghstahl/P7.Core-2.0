using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using P7.Core.Startup;

namespace P7.IdentityServer4.AspNetIdentity.Configuration
{
    public static class ConfigurationServicesExtension
    {
        public static void RegisterIdentityServer4ConfigurationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<IdentityServerConfig>(configuration.GetSection(IdentityServerConfig.WellKnown_SectionName));
            services.Configure<IdentityServerResourceClientCredentials>(options => configuration.Bind(options));
        }
    }
}