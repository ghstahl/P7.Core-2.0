using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using P7.Main.Models;
using P7.SessionContextStore.Core;

namespace P7.Main.Areas.Main.Controllers
{
    [Area("Main")]
    public class HomeController : Controller
    {
        private ISessionContextStore _sessionContextStore;
        public HomeController(ISessionContextStore sessionContextStore)
        {
            _sessionContextStore = sessionContextStore;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> About()
        {
            ViewData["Message"] = "Your application description page.";
            var result = HttpContext.User.Claims.Select(
                c => new ClaimType { Type = c.Type, Value = c.Value });
            return View(result);
        }

        public async Task<IActionResult> Contact()
        {
            ViewData["Message"] = "Your contact page.";
            ISessionContext sessionContext = await _sessionContextStore.GetSessionContextAsync();
            ViewData["some-data"] = await sessionContext.GetValueAsync<SomeData>("some-data");
            return View();
        }
      
    }
}
