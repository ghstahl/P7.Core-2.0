using System.Reflection.Metadata;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using P7.External.SPA.Core;
using P7.External.SPA.Models;
using System.Linq;

namespace P7.External.SPA.Areas.ExtSpa.Controllers
{
    [Area("ExtSPA")]
    public class HomeController : Controller
    {
        private ILogger Logger { get; set; }
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession Session => _httpContextAccessor.HttpContext.Session;
        private IExternalSPAStore _externalSpaStore;


        public HomeController(IHttpContextAccessor httpContextAccessor,
            IExternalSPAStore externalSpaStore,
            ILogger<HomeController> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _externalSpaStore = externalSpaStore;
            Logger = logger;
        }

        public IActionResult Index(string id)
        {
            Logger.LogInformation("Hello from the External SPA Home Index Controller");
            var spa = _externalSpaStore.GetRecord(id);
            var result = HttpContext.User.Claims.Select(
                c => new ClaimType {Type = c.Type, Value = c.Value});

           // var model = new HtmlString(spa.RenderTemplate);

            return View(spa.View, result);
        }
    }
}
