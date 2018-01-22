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
using P7.Core.Cache;
using P7.Core.Utils;
using P7.External.SPA.Filters;
using ZeroFormatter;

namespace P7.External.SPA.Areas.ExtSpa.Controllers
{
    public class FrontChannelRecord
    {
        public string KeepAliveUri { get; set; }
        public string LogoutUri { get; set; }
    }
    [ZeroFormattable]
    public class MySpaRecord
    {    
        [Index(0)]
        public virtual string Key { get; set; } 
   
        [Index(1)]
        public virtual string ClientId { get; set; }

        [Index(2)]
        public virtual string RedirectUri { get; set; }
        [Index(3)]
        public virtual string CacheBustHash { get; set; }
    }
    [ZeroFormattable]
    public class ViewBagRecord
    {
        // Index is key of serialization
        [Index(0)]
        public virtual string AuthorizeEndpoint { get; set; }

        [Index(1)]
        public virtual string AuthorizeUrl { get; set; }

        [Index(2)]
        public virtual MySpaRecord SpaRecord { get; set; }

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
        private const string _loadedSpasKey = ".loadedSpas";
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
            if (spa == null)
            {
                return new NotFoundResult();
            }

            var loadedSpas = SessionCacheManager<Dictionary<string, ExternalSPARecord>>.Grab(_httpContextAccessor.HttpContext,
                                 _loadedSpasKey) ?? new Dictionary<string, ExternalSPARecord>();

            
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

                var request = new RequestUrl(doc.AuthorizeEndpoint);
                var url = request.CreateAuthorizeUrl(
                    clientId: spa.ClientId,
                    responseType: OidcConstants.ResponseTypes.Code,
                    prompt: OidcConstants.PromptModes.None,
                    redirectUri: spa.RedirectUri,
                    scope: "openid profile email");
                var mySpaRecord = new MySpaRecord()
                {
                    ClientId = spa.ClientId,
                    Key = spa.Key,
                    RedirectUri = spa.RedirectUri,
                    CacheBustHash = spa.CacheBustHash
                };
                viewBagRecord = new ViewBagRecord { AuthorizeEndpoint = doc.AuthorizeEndpoint,
                    AuthorizeUrl = url, SpaRecord = mySpaRecord
                };
                var val = ZeroFormatterSerializer.Serialize(viewBagRecord);
                var cacheEntryOptions = new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));
                _cache.Set(cacheKey, val, cacheEntryOptions);
            }
 
            ViewBag.ViewBagRecord = viewBagRecord;
            if (!loadedSpas.ContainsKey(id))
            {
                loadedSpas.Add(id, spa);
                SessionCacheManager<Dictionary<string, ExternalSPARecord>>
                    .Insert(_httpContextAccessor.HttpContext, _loadedSpasKey, loadedSpas);
            }
            var key = $".extSpa.Session.{viewBagRecord.SpaRecord.Key}";
            var customData = SessionCacheManager<string>
                .Grab(_httpContextAccessor.HttpContext, key);
           
            ViewBag.CacheBustHash = viewBagRecord.SpaRecord.CacheBustHash;
            ViewBag.CustomData = customData;
            return View(spa.View, result);
        }
    }
}
