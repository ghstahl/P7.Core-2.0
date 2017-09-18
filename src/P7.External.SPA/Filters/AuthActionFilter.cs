using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using P7.External.SPA.Core;

namespace P7.External.SPA.Filters
{
    public class SpaRecord
    {
        public string Key { get; set; }
        public string Section { get; set; }
        public bool RequiresAuth { get; set; }
    }
    public class AuthActionFilter : ActionFilterAttribute
    {
        public static string Area { get; set; }
        public static string Controller { get; set; }
        public static string Action { get; set; }
        private static IConfiguration _configuration;
        private static IExternalSPAStore _externalSPAStore;
        public static Dictionary<string, SpaRecord> Spas = new Dictionary<string, SpaRecord>();
        public AuthActionFilter(IConfiguration configuration, IExternalSPAStore externalSpaStore)
        {
            _externalSPAStore = externalSpaStore;
            _configuration = configuration;
            Area = _configuration["Filters:Configuration:AuthActionFilter:Area"];
            Controller = _configuration["Filters:Configuration:AuthActionFilter:Controller"];
            Action = _configuration["Filters:Configuration:AuthActionFilter:Action"];
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                object key;
                if (!context.ActionArguments.TryGetValue("id", out key))
                {
                    context.Result = new UnauthorizedResult();
                }
                else
                {
                    var sKey = key as string;
                    var spa = _externalSPAStore.GetRecord(sKey);
                    if(spa == null)
                    {
                        context.Result = new UnauthorizedResult();
                    }
                    else
                    {
                        if (spa.RequireAuth)
                        {
                            context.Result = new RedirectToActionResult(Action, Controller,
                                new { area = Area, returnUrl = context.HttpContext.Items["original-path"] });
                        }
                    }
                }
            }
            else
            {
                var result = from claim in context.HttpContext.User.Claims
                             where claim.Type == ClaimTypes.NameIdentifier
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