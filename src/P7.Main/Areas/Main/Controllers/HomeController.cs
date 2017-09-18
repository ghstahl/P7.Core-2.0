using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using P7.Main.Models;

namespace P7.Main.Areas.Main.Controllers
{
    [Area("Main")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            var result = HttpContext.User.Claims.Select(
                c => new ClaimType { Type = c.Type, Value = c.Value });
            return View(result);
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }
      
    }
}
