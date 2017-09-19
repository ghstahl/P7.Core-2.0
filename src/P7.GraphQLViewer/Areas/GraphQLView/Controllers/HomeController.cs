using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace P7.GraphQLViewer.Areas.GraphQLView.Controllers
{
    [Area("GraphQLView")]
    public class HomeController : Controller
    {
        private ILogger Logger { get; set; }
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession Session => _httpContextAccessor.HttpContext.Session;



        public HomeController(IHttpContextAccessor httpContextAccessor,
            ILogger<HomeController> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            Logger = logger;
        }

        public IActionResult Index()
        {
            Logger.LogInformation("Hello from the GraphQL Home Index Controller");

            return View();
        }

   
    }
}
