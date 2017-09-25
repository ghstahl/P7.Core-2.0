using Microsoft.Extensions.DependencyInjection;

namespace P7.SessionContextStore.Core
{
    public static class SessionContextStoreExtentensions
    {
        public static IServiceCollection AddInMemoryRemoteSessionContext(this IServiceCollection services)
        {
            var obj = new InMemoryRemoteSessionContext();
            services.AddSingleton<IInMemoryRemoteSessionContext>(provider => obj);
            services.AddSingleton<IRemoteSessionContext>(provider => obj);
            return services;
        }
    }
}