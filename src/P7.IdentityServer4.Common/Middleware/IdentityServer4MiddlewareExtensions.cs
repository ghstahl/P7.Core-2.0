using Microsoft.AspNetCore.Builder;

namespace P7.IdentityServer4.Common.Middleware
{
    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class IdentityServer4MiddlewareExtensions
    {
        public static IApplicationBuilder UsePublicRefreshToken(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<PublicRefreshTokenMiddleware>();
        }
    }
}
