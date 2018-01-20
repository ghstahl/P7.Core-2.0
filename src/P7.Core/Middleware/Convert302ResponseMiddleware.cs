using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace P7.Core.Middleware
{
    public class Convert302ResponseMiddleware
    {
        private readonly RequestDelegate _next;
        public Convert302ResponseMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == 302
                && context.Request.Headers.ContainsKey("X-302to200"))
            {
                context.Response.StatusCode = 200;
            }
        }
    }
}