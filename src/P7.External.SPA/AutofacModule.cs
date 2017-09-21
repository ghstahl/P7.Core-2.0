using System.Linq;
using System.Reflection;
using Autofac;
using Microsoft.AspNetCore.Mvc.Filters;
using P7.Core.IoC;
using P7.Core.Middleware;
using P7.Core.Reflection;
using P7.Core.Scheduler.Scheduling;
using P7.External.SPA.Filters;
using P7.External.SPA.Scheduler;
using Serilog;
using Module = Autofac.Module;

namespace P7.Filters
{
    public class AutofacModule : Module
    {
        static ILogger logger = Log.ForContext<AutofacModule>();
        protected override void Load(ContainerBuilder builder)
        {
            logger.Information("Hi from P7.Filters Autofac.Load!");
            var assembly = this.GetType().GetTypeInfo().Assembly;

            /*
             * this autofind all types in this assembly, which is probably not what we want.  Lets register them all by hand.
           
            var derivedTypes = TypeHelper<ActionFilterAttribute>.FindDerivedTypes(assembly).ToArray();
            var derivedTypesName = derivedTypes.Select(x => x.GetTypeInfo().Name);
            logger.Information("Found these types: {DerivedTypes}", derivedTypesName);

            builder.RegisterTypes(derivedTypes).SingleInstance();

            */


            builder.RegisterNamedType<AuthActionFilter, ActionFilterAttribute>();
           

            builder.RegisterType<RemoteRazorLocationStoreTask>()
                .As<IScheduledTask>()
                .SingleInstance();
        }
    }
}
