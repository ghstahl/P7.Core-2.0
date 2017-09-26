using Autofac;
using Microsoft.Extensions.DependencyInjection;

namespace P7.SessionContextStore.Core
{
    public static class SessionContextStoreExtentensions
    {
        public static ContainerBuilder AddInMemoryRemoteSessionContext(this ContainerBuilder builder)
        {
            builder.RegisterType<InMemoryRemoteSessionContextAccessor>()
                .AsSelf()
                .As<IRemoteSessionContextAccessor>()
                .SingleInstance();
            builder.RegisterType<InMemoryRemoteSessionContext>()
                .As<IRemoteSessionContext>()
                .SingleInstance();
            return builder;
        }
    }
}