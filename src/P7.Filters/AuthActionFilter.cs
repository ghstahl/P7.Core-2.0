using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

namespace P7.Filters
{
    public class AuthActionFilter : ActionFilterAttribute
    {
        public static string Area { get; set; }
        public static string Controller { get; set; }
        public static string Action { get; set; }
        private static IConfiguration _configuration;
        public AuthActionFilter(IConfiguration configuration)
        {
            _configuration = configuration;
            Area = _configuration["Filters:Configuration:AuthActionFilter:Area"];
            Controller = _configuration["Filters:Configuration:AuthActionFilter:Controller"];
            Action = _configuration["Filters:Configuration:AuthActionFilter:Action"];
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
//                context.Result =  new ChallengeResult();
                context.Result = new RedirectToActionResult(Action,Controller, new { area = Area,returnUrl=context.HttpContext.Request.Path });
            }
            else
            {
                var result = from claim in context.HttpContext.User.Claims
                    where claim.Type == ClaimTypes.NameIdentifier || claim.Type == "name"
                    select claim;
                if (!result.Any())
                {
                    context.Result = new UnauthorizedResult();
                }
            }

            base.OnActionExecuting(context);
        }
    }
}