﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using P7.Core.Cache;
using P7.Core.Deployment;
using P7.Core.Startup;
using P7.Core.Utils;
using P7.External.SPA.Areas.ExtSpa.Controllers;
using P7.External.SPA.Core;
using P7.GraphQLCore;
using P7.GraphQLCore.Stores;
using ReferenceWebApp.InMemory;
using ReferenceWebApp.Models;
using ReferenceWebApp.Services;

namespace ReferenceWebApp.Controllers
{
     public class PreFlightOIDCOptions
    {
        public string Provider { get; set; }
      
    }
    public class ClaimHandle
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
    public class AccountConfig
    {
        public const string WellKnown_SectionName = "account";
        public List<ClaimHandle> PostLoginClaims { get; set; }
    }
    
    public static class ConfigurationServicesExtension
    {
        public static void RegisterAccountConfigurationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AccountConfig>(configuration.GetSection(AccountConfig.WellKnown_SectionName));
        }
    }

    [Authorize]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private IOptions<AccountConfig> _settings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IOptions<DeploymentOptions> _deploymentOptions;
        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            IHttpContextAccessor httpContextAccessor,
            IOptions<AccountConfig> settings,
            IOptions<DeploymentOptions> deploymentOptions,
            ILogger<AccountController> logger,
            IConfiguration config)
        {
            _settings = settings;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _deploymentOptions = deploymentOptions;
            _httpContextAccessor = httpContextAccessor;
            _config = config;
        }

        [TempData]
        public string ErrorMessage { get; set; }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["IsHttps"] = _httpContextAccessor.HttpContext.Request.IsHttps;
            ViewData["ReturnUrl"] = returnUrl;

            return View("Login.bulma");
        }

        public async Task LogoutSpa(string id)
        {
            _logger.LogInformation($"LogoutSpa({id}).");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var loadedSpas = SessionCacheManager<Dictionary<string, ExternalSPARecord>>
                                 .Grab(_httpContextAccessor.HttpContext, ".loadedSpas") ?? new Dictionary<string, ExternalSPARecord>();
            var query = from item in loadedSpas
                let c = new FrontChannelRecord { LogoutUri = item.Value.LogoutUri }
                select c;
            var frontChannelRecords = query.ToList();
            ViewBag.logoutRecords = frontChannelRecords;

            await _signInManager.SignOutAsync();
            _httpContextAccessor.HttpContext.Session.Clear();
          //  _httpContextAccessor.HttpContext.DeleteBlueGreenApplicationCookie(_deploymentOptions);
             
            _logger.LogInformation("User logged out.");
            var url = Url.Action(nameof(HomeController.Index), "Home");
            ViewBag.RedirectUrl = url;
            return View();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginWhatIf(string provider, string returnUrl = null)
        {
            var result = InternalExternalLogin(provider, returnUrl);
            return (result);
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> iFrameSilentSignin(string provider = null, string returnUrl = null,
            string errorUrl = null, string prompt = null)
        {
            return View(new iFrameSilentSigninModel()
            {
                Provider = provider == null ? "Google" : provider,
                ReturnUrl = returnUrl,
                ErrorUrl = errorUrl,
                Prompt = prompt
            });
        }
        

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> OIDCIFrameResult(string error = null)
        {
            var model = new OIDCIFrameResultModel()
            {
                Error = error,

            };
            if (string.IsNullOrEmpty(model.Error))
            {
                model.OIDC = await HarvestOidcDataAsync();
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> SuccessJson()
        {
            var oidc = await HarvestOidcDataAsync();
            return new JsonResult(oidc);
        }
        [HttpGet]
        public async Task<IActionResult> ErrorJson(string error)
        {
            var payload = new ErrorPayload() { Error = error };
            return new JsonResult(payload);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> iFrameProxy()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Blank()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task ImportantData()
        {

        }
        [HttpPost]
        [AllowAnonymous]
        public async Task UnimportantData()
        {

        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PreFlightOIDCAuthorize([FromBody]PreFlightOIDCOptions options)
        {
            var result = InternalExternalLogin(options.Provider);
            return (result);
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLogin(string provider, string returnUrl = null, string errorUrl = null, string prompt = null)
        {
            var result = InternalExternalLogin(provider, returnUrl);
            return (result);
        }

        private IActionResult InternalExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            var challeng = Challenge(properties, provider);
            return challeng;
        }

        async Task<Dictionary<string, string>> HarvestOidcDataAsync()
        {
            var at = await HttpContext.GetTokenAsync(IdentityConstants.ExternalScheme, "access_token");
            var idt = await HttpContext.GetTokenAsync(IdentityConstants.ExternalScheme, "id_token");
            var rt = await HttpContext.GetTokenAsync(IdentityConstants.ExternalScheme, "refresh_token");
            var tt = await HttpContext.GetTokenAsync(IdentityConstants.ExternalScheme, "token_type");
            var ea = await HttpContext.GetTokenAsync(IdentityConstants.ExternalScheme, "expires_at");
          
            var oidc = new Dictionary<string, string>
            {
                {"access_token", at},
                {"id_token", idt},
                {"refresh_token", rt},
                {"token_type", tt},
                {"expires_at", ea}
            };
            return oidc;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            string currentNameIdClaimValue = null;
            if (User.Identity.IsAuthenticated)
            {
                // we will only create a new user if the user here is actually new.
                var qName = from claim in User.Claims
                    where claim.Type == ".nameIdentifier"
                            select claim;
                var nc = qName.FirstOrDefault();
                currentNameIdClaimValue = nc?.Value;
            }

            var oidc = await HarvestOidcDataAsync();

            var session = _httpContextAccessor.HttpContext.Session;
            

            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToAction(nameof(Login));
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var query = from claim in info.Principal.Claims
                where claim.Type == "DisplayName"
                select claim;
            var queryNameId = from claim in info.Principal.Claims
                where claim.Type == ClaimTypes.NameIdentifier
                select claim;
            var nameClaim = query.FirstOrDefault();
            var displayName = nameClaim.Value;
            var nameIdClaim = queryNameId.FirstOrDefault();

            if (!string.IsNullOrEmpty(currentNameIdClaimValue) &&
                (currentNameIdClaimValue != nameIdClaim.Value))
            {
                session.Clear();
            }
           
            if (currentNameIdClaimValue == nameIdClaim.Value)
            {
                session.SetObject(".identity.oidc", oidc);
                session.SetObject(".identity.strongLoginUtc", DateTimeOffset.UtcNow);
                // this is a re login from the same user, so don't do anything;
                return RedirectToLocal(returnUrl);
            }
            
            // paranoid
            var leftoverUser = await _userManager.FindByEmailAsync(displayName);
            if (leftoverUser != null)
            {
                await _userManager.DeleteAsync(leftoverUser); // just using this inMemory userstore as a scratch holding pad
            }
            // paranoid end

            var user = new ApplicationUser { UserName = nameIdClaim.Value, Email = displayName };
            // SHA256 is disposable by inheritance.  
            using (var sha256 = SHA256.Create())
            {
                // Send a sample text to hash.  
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(nameIdClaim.Value));
                // Get the hashed string.  
                var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                SessionCacheManager<string>.Insert(_httpContextAccessor.HttpContext, ".identity.userHash", hash);
            }
            var result = await _userManager.CreateAsync(user);
            var newUser = await _userManager.FindByIdAsync(user.Id);

            var cQuery = from claim in _settings.Value.PostLoginClaims
                let c = new Claim(claim.Name, claim.Value)
                select c;
            var eClaims = cQuery.ToList();
            eClaims.Add(new Claim("custom-name", displayName));
            eClaims.Add(new Claim(".nameIdentifier", nameIdClaim.Value));// normalized id.
            await _userManager.AddClaimsAsync(newUser, eClaims);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                await _userManager.DeleteAsync(user); // just using this inMemory userstore as a scratch holding pad
                _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                session.SetObject(".identity.oidc", oidc);
                session.SetObject(".identity.strongLoginUtc", DateTimeOffset.UtcNow);
          //      _httpContextAccessor.HttpContext.DropBlueGreenApplicationCookie(_deploymentOptions);

                return RedirectToLocal(returnUrl);

            }
            return RedirectToAction(nameof(Login));
        }

 

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion


        [HttpGet]
        [AllowAnonymous]
        public async Task<string> CheckAuthenticatedSession()
        {
            if (User.Identity.IsAuthenticated)
            {
                return "Horray, you are authenticated";
            }
            return "Oh My, Not authenticated!";
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task EstablishAuthenticatedSession(string accessToken)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            var webHeaderCollection = new WebHeaderCollection
            {
                {"Authorization", $"Bearer {accessToken}"}
            };

            var byteJwt = await RemoteFetch.FetchAsync(
                NortonDefaults.Development.UserInformationEndpoint,
                new WebRequestInit()
                {
                    Headers = webHeaderCollection
                });
 
            var jwt =  Encoding.UTF8.GetString(byteJwt);
            var handler = new JwtSecurityTokenHandler();
            var tokenS = handler.ReadToken(jwt) as JwtSecurityToken;
            var queryNameId = from claim in tokenS.Claims
                where claim.Type == "sub"
                select claim;
           
         
            var nameIdClaim = queryNameId.FirstOrDefault();


            var user = new ApplicationUser { UserName = nameIdClaim.Value, Email = nameIdClaim.Value };

            try
            {
                var result = await _userManager.CreateAsync(user);
                await _signInManager.SignInAsync(user, isPersistent: false);
                await _userManager.DeleteAsync(user); // just using this inMemory userstore as a scratch holding pad

            }
            catch (Exception e)
            {
                
            }


        }
    }

    [Area("Api")]
    [Route("api/[controller]")]
    public class IdentityApiController : Controller
    {
        public async Task<ActionResult> Get()
        {
            var jsonResult = new JsonResult(User.Claims.Select(c=>new
            {
                c.Type,c.Value
            }));
            return jsonResult;
        }
    }


    [Area("Api")]
    [Route("api/[controller]")]
    public class RedirectorApiController : Controller
    {
        public async Task<ActionResult> Get(string url)
        {
            return new RedirectResult(url);
        }
    }

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

                var client = new TokenClient(
                    doc.TokenEndpoint,
                    clientId, cientSecret,style:AuthenticationStyle.PostValues);
                
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
