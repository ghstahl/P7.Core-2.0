using System.Reflection;
using Autofac;
using Microsoft.AspNetCore.Mvc.Filters;
using P7.Core.IoC;
using P7.Core.Scheduler.Scheduling;
using P7.IdentityServer4.AspNetIdentity.Scheduler;
using P7.IdentityServer4.AspNetIdentity.Stores;
using Serilog;
using Module = Autofac.Module;

namespace P7.IdentityServer4.AspNetIdentity
{
    public class AutofacModule : Module
    {
        static ILogger logger = Log.ForContext<AutofacModule>();
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RemoteIdentityServerDiscoveryStore>()
                .As<IIdentityServerDiscoveryStore>()
                .As<IRemoteIdentityServerDiscoveryStore>()
                .SingleInstance();
            builder.RegisterType<IdentityServerDiscoveryTask>()
                .As<IScheduledTask>()
                .SingleInstance();
        }
    }
}
