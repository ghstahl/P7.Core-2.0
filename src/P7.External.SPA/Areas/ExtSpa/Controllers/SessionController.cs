using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using P7.Core.Cache;
using P7.Core.Utils;
using P7.External.SPA.Core;

namespace P7.External.SPA.Areas.ExtSpa.Controllers
{
    public class StoreSessionDataRequest
    {
        public string Key { get; set; }
        public string Data { get; set; }
    }


    [Produces("application/json")]
    [Route("api/extSpa/session")]
    public class SessionController : Controller
    {
        private ILogger Logger { get; set; }
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession Session => _httpContextAccessor.HttpContext.Session;
        private IExternalSPAStore _externalSpaStore;

        public SessionController(IHttpContextAccessor httpContextAccessor,
            IExternalSPAStore externalSpaStore,
            ILogger<HomeController> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _externalSpaStore = externalSpaStore;
            Logger = logger;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("store-data")]
        public async Task<IActionResult> StoreData([FromBody] StoreSessionDataRequest request)
        {
            if (request == null)
            {
                return new NotFoundResult();
            }
            if (string.IsNullOrEmpty(request.Key))
            {
                return new NotFoundResult();
            }
            if (!string.IsNullOrEmpty(request.Data) && request.Data.Length > 4096)
            {
                return new NotFoundResult();
            }
            var spa = _externalSpaStore.GetRecord(request.Key);
            if (spa == null)
            {
                return new NotFoundResult();
            }
            var key = $".extSpa.Session.{request.Key}";
            SessionCacheManager<string>
                .Insert(_httpContextAccessor.HttpContext, key, request.Data);
            return new OkResult(); 
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("fetch-data")]
        public async Task<IActionResult> FetchData(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new NotFoundResult();
            }
            var key = $".extSpa.Session.{id}";
            if (Session.IsAvailable)
            {
                var data = SessionCacheManager<string>
                    .Grab(_httpContextAccessor.HttpContext, key);
               
                return new JsonResult(data);
            }
            return new JsonResult(null);
        }
    }
}