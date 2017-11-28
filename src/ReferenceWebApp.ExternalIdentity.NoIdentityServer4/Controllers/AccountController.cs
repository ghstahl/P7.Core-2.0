using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using P7.Core.Startup;
using P7.Core.Utils;
using P7.GraphQLCore;
using ReferenceWebApp.InMemory;
using ReferenceWebApp.Models;
using ReferenceWebApp.Services;

namespace ReferenceWebApp.Controllers
{
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
    public class MyAccountConfigureServicesRegistrant : ConfigureServicesRegistrant
    {
        public override void OnConfigureServices(IServiceCollection services)
        {
            services.Configure<AccountConfig>(Configuration.GetSection(AccountConfig.WellKnown_SectionName));

        }

        public MyAccountConfigureServicesRegistrant(IConfiguration configuration) : base(configuration)
        {
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
        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            IHttpContextAccessor httpContextAccessor,
            IOptions<AccountConfig> settings,
            ILogger<AccountController> logger,
            IConfiguration config)
        {
            _settings = settings;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
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

 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLogin(string provider, string returnUrl = null)
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
            var oidc = await HarvestOidcDataAsync();

            var session = _httpContextAccessor.HttpContext.Session;
            session.SetObject(".identity.oidc", oidc);

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

            

            // paranoid
            var leftoverUser = await _userManager.FindByEmailAsync(displayName);
            if (leftoverUser != null)
            {
                await _userManager.DeleteAsync(leftoverUser); // just using this inMemory userstore as a scratch holding pad
            }
            // paranoid end

            var user = new ApplicationUser { UserName = nameIdClaim.Value, Email = displayName };
            var result = await _userManager.CreateAsync(user);
            var newUser = await _userManager.FindByIdAsync(user.Id);

            var cQuery = from claim in _settings.Value.PostLoginClaims
                        let c = new Claim(claim.Name, claim.Value)
                        select c;
            var eClaims = cQuery.ToList();
            eClaims.Add(new Claim("custom-name", displayName));
            await _userManager.AddClaimsAsync(newUser, eClaims);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                await _userManager.DeleteAsync(user); // just using this inMemory userstore as a scratch holding pad
                _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                session.SetObject(".identity.strongLoginUtc", DateTimeOffset.UtcNow);
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

            var jwt = await RemoteJsonFetch.FetchAsync(
                NortonDefaults.Development.UserInformationEndpoint,
                new WebRequestInit()
                {
                    Headers = webHeaderCollection
                });

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
            var jsonResult = new JsonResult(User.Claims.Select(c => new
            {
                c.Type,
                c.Value
            }));
            return jsonResult;
        }
 
    }
}
