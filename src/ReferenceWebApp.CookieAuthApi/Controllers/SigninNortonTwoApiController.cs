using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace ReferenceWebApp.CookieAuthApi.Controllers
{
    [Area("Api")]
    [Route("signin-norton-two")]
    public class SigninNortonTwoApiController : Controller
    {
        private DiscoveryCache _discoveryCache;
        private IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private string OriginalPathBase => "/";

        public SigninNortonTwoApiController(
            IConfiguration configuration,
            DiscoveryCache discoveryCache,
            IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _discoveryCache = discoveryCache;
            _httpContextAccessor = httpContextAccessor;
        }

        protected string BuildRedirectUri(string targetPath)
        {
            return (((this.Request.Scheme + "://" + this.Request.Host) + this.OriginalPathBase) + targetPath);
        }

        public async Task<ActionResult> Get()
        {
            if (Request.Query.ContainsKey("code"))
            {
                var doc = await _discoveryCache.GetAsync();
                var clientId = _configuration["Norton-ClientId-Two"];
                var cientSecret = _configuration["Norton-ClientSecret-Two"];

                var client = new IdentityModel.Client.TokenClient(
                    doc.TokenEndpoint,
                    clientId, cientSecret, style:IdentityModel.Client.AuthenticationStyle.PostValues);

                StringValues codeValue;
                Request.Query.TryGetValue("code", out codeValue);
                var code = codeValue[0];
                var redirectUri = BuildRedirectUri("signin-norton-two");
                var token = await client.RequestAuthorizationCodeAsync(code, redirectUri);
                var jsonResult = new JsonResult(token.Json);
                return jsonResult;
            }
            if (Request.Query.ContainsKey("error"))
            {
                var jsonResult = new JsonResult(Request.Query);
                return jsonResult;
            }
            return new NotFoundResult();
        }
    }
}
