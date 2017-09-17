using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace P7.Filters
{
    public class DenyAllActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            context.Result = new UnauthorizedResult();
            base.OnActionExecuting(context);
        }
    }
}