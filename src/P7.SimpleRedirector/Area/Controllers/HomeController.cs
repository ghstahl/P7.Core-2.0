using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using P7.SimpleRedirect.Core;

namespace P7.SimpleRedirector.Area.Controllers
{
    [Area("SimpleRedirector")]
    public class HomeController : Controller
    {
        private ILogger Logger { get; set; }
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession Session => _httpContextAccessor.HttpContext.Session;
        private ISimpleRedirectorStore _simpleRedirectorStore;

        public HomeController(IHttpContextAccessor httpContextAccessor,
            ISimpleRedirectorStore simpleRedirectorStore,
            ILogger<HomeController> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _simpleRedirectorStore = simpleRedirectorStore;
            Logger = logger;
        }

        public async Task<ActionResult> Index(string key, string remaining)
        {
            var record = await _simpleRedirectorStore.FetchRedirectRecord(key);
            if (record != null)
            {

                var requestScheme = _httpContextAccessor.HttpContext.Request.Scheme;
                string scheme = (record.Scheme) ?? requestScheme;

                var realUrl = string.Format("{0}://{1}/{2}", scheme, record.BaseUrl, remaining);
                return new RedirectResult(realUrl);
            }
            return new NotFoundResult();
        }
    }
}
