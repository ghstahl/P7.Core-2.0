using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using P7.Core.Settings;
using P7.Core.Startup;

namespace P7.GraphQLCore
{
    public class MutationConfig
    {
        public List<string> OptOut { get; set; }
    }
    public class QueryConfig
    {
        public List<string> OptOut { get; set; }
    }
    public class GraphQLAuthenticationConfig
    {
        public const string WellKnown_SectionName = "graphQLAuthentication";
        public MutationConfig Mutation { get; set; }
        public QueryConfig Query { get; set; }
    }
    public class MyConfigureServicesRegistrant : ConfigureServicesRegistrant
    {
        public override void OnConfigureServices(IServiceCollection services)
        {
            services.Configure<GraphQLAuthenticationConfig>(Configuration.GetSection(GraphQLAuthenticationConfig.WellKnown_SectionName));

        }

        public MyConfigureServicesRegistrant(IConfiguration configuration) : base(configuration)
        {
        }
    }
}