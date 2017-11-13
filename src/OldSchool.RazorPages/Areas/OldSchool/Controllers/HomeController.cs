using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace OldSchool.RazorPages.Areas.OldSchool.Controllers
{
    [Area("OldSchool")]
    public class HomeController : Controller
    {
        private ILogger Logger { get; set; }
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession Session => _httpContextAccessor.HttpContext.Session;
        private IOldSchoolMagicService _oldSchoolMagicService;


        public HomeController(IHttpContextAccessor httpContextAccessor,
            IOldSchoolMagicService oldSchoolMagicService,
            ILogger<HomeController> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _oldSchoolMagicService = oldSchoolMagicService;
            Logger = logger;
        }

        public IActionResult Index(string id)
        {
            Logger.LogInformation("Hello from OldSchool Home Index Controller");
            var utcNow = _oldSchoolMagicService.UtcNow();
            ViewData["UtcNow"] = utcNow;
            return View();
        }
    }
}
