using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace P7.Filters
{
    public class AntiForgeryActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
              
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}