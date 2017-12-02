using System.Reflection.Metadata;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using P7.External.SPA.Core;
using P7.External.SPA.Models;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using P7.External.SPA.Filters;

namespace P7.External.SPA.Areas.ExtSpa.Controllers
{
    class ViewBagRecord
    {
        public string AuthorizeUrl { get; set; }
        public dynamic SpaRecord { get; set; }
    }
    [Area("ExtSPA")]
    public class HomeController : Controller
    {
        private ILogger Logger { get; set; }
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession Session => _httpContextAccessor.HttpContext.Session;
        private IExternalSPAStore _externalSpaStore;
        private IConfiguration _configuration;
        private DiscoveryCache _discoveryCache;
        public HomeController(IHttpContextAccessor httpContextAccessor,
            IExternalSPAStore externalSpaStore,
            IConfiguration configuration,
            DiscoveryCache discoveryCache,
            ILogger<HomeController> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _externalSpaStore = externalSpaStore;
            _discoveryCache = discoveryCache;
            Logger = logger;
        }
        
        public async Task<IActionResult> Index(string id)
        {
            Logger.LogInformation("Hello from the External SPA Home Index Controller");
            var spa = _externalSpaStore.GetRecord(id);
            var result = HttpContext.User.Claims.Select(
                c => new ClaimType {Type = c.Type, Value = c.Value});
           

            var clientId = _configuration["Norton-ClientId"];
            var cientSecret = _configuration["Norton-ClientSecret"];
            var doc = await _discoveryCache.GetAsync();

            var request = new AuthorizeRequest(doc.AuthorizeEndpoint);
            var url = request.CreateAuthorizeUrl(
                clientId: "test_lifelock/p7core/io",
                responseType: OidcConstants.ResponseTypes.Code,
                prompt:OidcConstants.PromptModes.None,
                redirectUri: "https://p7core.127.0.0.1.xip.io:44311/lifelock/signin-norton",
                scope: "openid profile email");

            var viewBagRecord = new ViewBagRecord {AuthorizeUrl = url, SpaRecord = spa};
            ViewBag.ViewBagRecord = viewBagRecord;

            //	var url = "https://login-int.norton.com/sso/idp/OIDC?prompt=none&response_type=code&scope=openid%20profile%20email&client_id=test_lifelock/p7core/io&redirect_uri=https://p7core.127.0.0.1.xip.io:44311/lifelock/signin-norton";


            //    ViewData[".spaRecord"] = JsonConvert.SerializeObject(spa); 
            // var model = new HtmlString(spa.RenderTemplate);

            return View(spa.View, result);
        }
    }
}
