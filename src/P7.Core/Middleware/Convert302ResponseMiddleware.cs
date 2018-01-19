using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using P7.Core.Deployment;

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
    public class BlueGreenMiddleware
    {
        private readonly RequestDelegate _next;
        private IOptions<DeploymentOptions> _deploymentOptions;
        public BlueGreenMiddleware(RequestDelegate next,
            IOptions<DeploymentOptions> deploymentOptions)
        {
            _next = next;
            _deploymentOptions = deploymentOptions;
        }
        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Cookies.ContainsKey($".bluegreen.{_deploymentOptions.Value.Color}"))
            {
                context.Items.Add(".blueGreenLock", true);
            }

            await _next(context);

            if (context.Items.ContainsKey(".blueGreenLock"))
            {
                context.Response.Cookies.Append($".bluegreen.{_deploymentOptions.Value.Color}", "true",
                    new CookieOptions()
                    {
                        Expires = DateTime.Now.AddMinutes(40)
                    });
            }
            else
            {
                context.Response.Cookies.Delete($".bluegreen.{_deploymentOptions.Value.Color}");
            }
        }
    }
}