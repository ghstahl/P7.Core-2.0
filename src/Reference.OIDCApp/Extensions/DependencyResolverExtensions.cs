using Microsoft.AspNetCore.Builder;

namespace Reference.OIDCApp.Services
{
    public static class DependencyResolverExtensions
    {
        public static T GetService<T>(this IApplicationBuilder applicationBuilder) where T : class
        {
            return applicationBuilder.ApplicationServices.GetService(typeof(T)) as T;
        }
    }
}