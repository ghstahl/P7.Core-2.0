using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using A.OIDC.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace A.OIDC.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> About()
        {
            ViewData["Message"] = "Your application description page.";
            if (User.Identity.IsAuthenticated)
            {
                var dd = HttpContext.RequestServices.GetRequiredService<IAuthenticationService>();
                var kk = dd.GetTokenAsync(HttpContext, "access_token");

                string accessToken = await HttpContext.GetTokenAsync("access_token");
                accessToken = await HttpContext.GetTokenAsync(".Token.access_token");
                accessToken = await HttpContext.GetTokenAsync("Token.access_token");


                string idToken = await HttpContext.GetTokenAsync("id_token");
            }
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
