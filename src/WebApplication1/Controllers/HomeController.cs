using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using P7.Main.Models;
using P7.SessionContextStore.Core;
using WebApplication1.Models;

namespace WebApplication1.Controllers
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
            return View();
        }

        public async Task<IActionResult> About()
        {
            ViewData["Message"] = "Your application description page.";
            return View();
        }

        public async Task<IActionResult> Contact()
        {
            ViewData["Message"] = "Your contact page.";
            ISessionContext sessionContext = await _sessionContextStore.GetSessionContextAsync(); 
            ViewData["some-data"] = await sessionContext.GetValueAsync<SomeData>("some-data");
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
