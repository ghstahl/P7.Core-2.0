using Microsoft.Extensions.DependencyInjection;

namespace P7.Core.Startup
{
    interface IConfigureServicesRegistrant
    {
        void OnConfigureServices(IServiceCollection serviceCollection);
    }
}