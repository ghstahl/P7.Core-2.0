using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace P7.Filters
{
    public class LogFilter3 : ActionFilterAttribute
    {
        /*
        private readonly ILog logger;

        public LogFilter(ILog logger)
        {
            this.logger = logger;
        }
*/
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            Console.WriteLine(actionContext.HttpContext.Request);
            //            this.logger.Log(actionContext.HttpContext.Request);
        }
    }
}