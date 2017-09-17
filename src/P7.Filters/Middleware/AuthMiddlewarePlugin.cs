using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using P7.Core.Middleware;

namespace P7.Filters.Middleware
{
    public class AuthMiddlewarePlugin : MiddlewarePlugin
    {
        public static string Area { get; set; }
        public static string Controller { get; set; }
        public static string Action { get; set; }
        private static IConfigurationRoot _configurationRoot;

        public AuthMiddlewarePlugin(IConfigurationRoot configurationRoot)
        {
            _configurationRoot = configurationRoot;
            Area = _configurationRoot["Filters:Configuration:AuthActionFilter:Area"];
            Controller = _configurationRoot["Filters:Configuration:AuthActionFilter:Controller"];
            Action = _configurationRoot["Filters:Configuration:AuthActionFilter:Action"];
        }

        private static string _redirectPath = null;

        private static string RedirectPath
            => _redirectPath ?? (_redirectPath = string.Format("/{0}/{1}/{2}", Area, Controller, Action));

        public override bool Invoke(HttpContext httpContext)
        {
            if (httpContext.User.Identity.IsAuthenticated)
                return true;
            httpContext.Response.Redirect(RedirectPath);
            return false;
        }
    }
}
