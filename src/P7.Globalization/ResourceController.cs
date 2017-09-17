using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using P7.Core.Localization;
using P7.Core.Reflection;

namespace P7.Globalization
{
    [Route("ResourceApi/[controller]")]
    public class ResourceController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession Session => _httpContextAccessor.HttpContext.Session;
        private IResourceFetcher _resourceFetcher;
        private ILogger Logger { get; set; }

        public ResourceController(
            IHttpContextAccessor httpContextAccessor,
            IResourceFetcher resourceFetcher,
            ILogger<ResourceController> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _resourceFetcher = resourceFetcher;
            Logger = logger;
        }

        [Route("ByDynamic")]
        [Produces(typeof(object))]
        public async Task<IActionResult> GetResourceSet(string id, string treatment)
        {
            var rqf = Request.HttpContext.Features.Get<IRequestCultureFeature>();
            // Culture contains the information of the requested culture
            var currentCulture = rqf.RequestCulture.Culture;

            // Load Header collection into NameValueCollection object.
            var headers = _httpContextAccessor.HttpContext.Request.Headers;
            if (headers.ContainsKey("X-Culture"))
            {
                var hCulture = headers["X-Culture"];
                CultureInfo hCultureInfo = currentCulture;
                try
                {
                    hCultureInfo = new CultureInfo(hCulture);
                }
                catch (Exception)
                {
                    hCultureInfo = currentCulture;
                }
                currentCulture = hCultureInfo;
            }
            var obj = _resourceFetcher.GetResourceSet(new ResourceQueryHandle()
            {
                Culture = currentCulture.Name,
                Id = id,
                Treatment = treatment
            });

            return Ok(obj);
        }
    }
}
