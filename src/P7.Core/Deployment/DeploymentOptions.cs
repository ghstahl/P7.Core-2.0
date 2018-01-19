using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace P7.Core.Deployment
{
    public class DeploymentOptions
    {
        public const string WellKnown_SectionName = "deployment";
        [JsonProperty("type")]
        public string Color { get; set; }
    }

    public static class DeploymentExtensions
    {
        public static IServiceCollection RegisterDeploymentConfigurationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DeploymentOptions>(configuration.GetSection(DeploymentOptions.WellKnown_SectionName));
            return services;
        }
    }
}
