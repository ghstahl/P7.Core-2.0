using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace P7.Core.Startup
{
    public abstract class ConfigureServicesRegistrant: IConfigureServicesRegistrant
    {
        public IConfiguration Configuration { get; private set; }
        protected ConfigureServicesRegistrant( )
        {
        }
        protected ConfigureServicesRegistrant(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public abstract void OnConfigureServices(IServiceCollection serviceCollection);
    }
}
