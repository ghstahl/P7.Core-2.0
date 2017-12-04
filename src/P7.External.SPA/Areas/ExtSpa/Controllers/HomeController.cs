using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using P7.External.SPA.Core;
using P7.External.SPA.Models;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using P7.External.SPA.Filters;
using ZeroFormatter;

namespace P7.External.SPA.Areas.ExtSpa.Controllers
{
 
    [ZeroFormattable]
    public class ViewBagRecord
    {
        // Index is key of serialization
        [Index(0)]
        public virtual string AuthorizeEndpoint { get; set; }

        [Index(1)]
        public virtual string AuthorizeUrl { get; set; }

        [Index(2)]
        public virtual ExternalSPARecord SpaRecord { get; set; }
    }


    
    [Area("ExtSPA")]
    public class HomeController : Controller
    {
        private readonly IDistributedCache _cache;
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
            IDistributedCache cache,
            ILogger<HomeController> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _externalSpaStore = externalSpaStore;
            _discoveryCache = discoveryCache;
            _cache = cache;
            Logger = logger;
        }
        
        public async Task<IActionResult> Index(string id)
        {
            Logger.LogInformation("Hello from the External SPA Home Index Controller");
            var spa = _externalSpaStore.GetRecord(id);
            var result = HttpContext.User.Claims.Select(
                c => new ClaimType {Type = c.Type, Value = c.Value});

            var cacheKey = $".extSpaViewBagRecord.{id}";

            ViewBagRecord viewBagRecord = null;
            var value = await _cache.GetAsync(cacheKey);
            if (value != null)
            {
                viewBagRecord = ZeroFormatterSerializer.Deserialize<ViewBagRecord>(value);
            }
            else
            {
                var doc = await _discoveryCache.GetAsync();

                var request = new AuthorizeRequest(doc.AuthorizeEndpoint);
                var url = request.CreateAuthorizeUrl(
                    clientId: spa.ClientId,
                    responseType: OidcConstants.ResponseTypes.Code,
                    prompt: OidcConstants.PromptModes.None,
                    redirectUri: spa.RedirectUri,
                    scope: "openid profile email");

                viewBagRecord = new ViewBagRecord { AuthorizeEndpoint = doc.AuthorizeEndpoint, AuthorizeUrl = url, SpaRecord = spa };
                var val = ZeroFormatterSerializer.Serialize(viewBagRecord);
                var cacheEntryOptions = new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromHours(24));
                _cache.Set(cacheKey, val, cacheEntryOptions);
            }
 
            ViewBag.ViewBagRecord = viewBagRecord;

            //	var url = "https://login-int.norton.com/sso/idp/OIDC?prompt=none&response_type=code&scope=openid%20profile%20email&client_id=test_lifelock/p7core/io&redirect_uri=https://p7core.127.0.0.1.xip.io:44311/lifelock/signin-norton";


            //    ViewData[".spaRecord"] = JsonConvert.SerializeObject(spa); 
            // var model = new HtmlString(spa.RenderTemplate);

            return View(spa.View, result);
        }
    }
}
