﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.Extensions.FileProviders;
using P7.SessionContextStore.Core;
using ReferenceWebApp.Models;

namespace ReferenceWebApp.Controllers
{
    public class HomeController : Controller
    {
        private ISessionContextStore _sessionContextStore;

        public HomeController(ISessionContextStore sessionContextStore)
        {
            _sessionContextStore = sessionContextStore;
        }

        public IActionResult Index()
        {
            ViewData["scheme"] = Request.Scheme;
            ViewData["host"] = Request.Host.Value;
            return View();
        }

        public async Task<IActionResult> About()
        {
            ViewData["Message"] = "Your application description page.";
            var result = HttpContext.User.Claims.Select(
                c => new ClaimType {Type = c.Type, Value = c.Value});

            if (User.Identity.IsAuthenticated)
            {
                string accessToken = await HttpContext.GetTokenAsync(IdentityConstants.ExternalScheme, "access_token");
                string idToken = await HttpContext.GetTokenAsync(IdentityConstants.ExternalScheme, "id_token");

                // Now you can use them. For more info on when and how to use the 
                // access_token and id_token, see https://auth0.com/docs/tokens
            }

            return View(result);
        }

        public async Task<IActionResult> Contact()
        {
            ViewData["Message"] = "Your contact page.";
            ISessionContext sessionContext = await _sessionContextStore.GetSessionContextAsync();
            ViewData["some-data"] = await sessionContext.GetValueAsync<SomeData>("some-data");
            return View();
        }

        public async Task<IActionResult> Add([FromServices] INodeServices nodeServices,
            [FromServices] IHostingEnvironment env,
            [FromServices] IFileProvider fileProvider)
        {
            var webRoot = env.WebRootPath;
            var file = System.IO.Path.Combine(webRoot, "js\\AddModule.js");

            var num1 = 10;
            var num2 = 20;
            var result = await nodeServices.InvokeAsync<int>(file, num1, num2);
            ViewData["ResultFromNode"] = $"Result of {num1} + {num2} is {result}";
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }

}
