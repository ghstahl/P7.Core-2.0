using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using P7.Core.Reflection;
using Serilog;

namespace P7.Core.Startup
{
    public static class ConfigureServicesRegistrantExtensions
    {
         static ILogger logger = Log.ForContext<AutofacModule>();

        public static IServiceCollection AddAllConfigureServicesRegistrants(this IServiceCollection services, IConfiguration configuration)
        {
            bool bCaughtException = false;
            logger.Information("AddAllConfigureServicesRegistrants Enter");
            var types = TypeHelper<ConfigureServicesRegistrant>
                .FindTypesInAssemblies(TypeHelper<ConfigureServicesRegistrant>.IsPublicClassType);
            foreach (var type in types)
            {
                logger.Information("Found:{0}", type);
                var instance = (IConfigureServicesRegistrant) Activator.CreateInstance(type, configuration);
                try
                {
                    instance.OnConfigureServices(services);
                }
                catch (Exception e)
                {
                    bCaughtException = true;
                    logger.Fatal("Failed to call OnConfigureServices on type:{0},{1}", type, e.Message);
                }
            }
            if (bCaughtException)
            {
                throw new Exception("AddAllConfigureServicesRegistrants had one or more exceptions");
            }
            return services;
        }
    }
}
