using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using P7.Core.Startup;

namespace P7.IdentityServer4.AspNetIdentity.Configuration
{
    public class MyConfigureServicesRegistrant : ConfigureServicesRegistrant
    {
        public override void OnConfigureServices(IServiceCollection services)
        {
            services.Configure<IdentityServerConfig>(Configuration.GetSection(IdentityServerConfig.WellKnown_SectionName));
            services.Configure<IdentityServerResourceClientCredentials>(options => Configuration.Bind(options));
        }

        public MyConfigureServicesRegistrant(IConfiguration configuration) : base(configuration)
        {
        }
    }
}