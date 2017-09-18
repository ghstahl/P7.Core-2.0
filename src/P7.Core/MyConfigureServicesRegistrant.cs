using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using P7.Core.Settings;
using P7.Core.Startup;

namespace P7.Core
{
    public class MyConfigureServicesRegistrant : ConfigureServicesRegistrant
    {
        public override void OnConfigureServices(IServiceCollection services)
        {
            services.Configure<FiltersConfig>(Configuration.GetSection(FiltersConfig.WellKnown_SectionName));

        }

        public MyConfigureServicesRegistrant(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
