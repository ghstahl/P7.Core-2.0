using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Reference.OIDCApp.Data;
using Reference.OIDCApp.Services;

namespace Reference.OIDCApp.Pages.Account
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

    public static class ConfigurationServicesExtension
    {
        public static void RegisterAccountConfigurationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AccountConfig>(configuration.GetSection(AccountConfig.WellKnown_SectionName));
        }
    }

    public class ExternalLoginModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ExternalLoginModel> _logger;
        private IOptions<AccountConfig> _settings;
        public ExternalLoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor,
            IOptions<AccountConfig> settings,
            ILogger<ExternalLoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _settings = settings;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string LoginProvider { get; set; }

        public string ReturnUrl { get; set; }

        public string Prompt { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public IActionResult OnGetAsync()
        {
            return RedirectToPage("./Login");
        }

        public IActionResult OnPost(string provider, string returnUrl = null, string errorUrl = null, string prompt = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl, prompt });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }
        private async Task<Dictionary<string, string>> HarvestOidcDataAsync()
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
        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string prompt = null, string remoteError = null)
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
                return RedirectToPage("./Login");
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToPage("./Login");
            }

            // Sign in the user with this external login provider if the user already has a login.
         /*
          var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor : true);
            if (result.Succeeded)
            {
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
                return LocalRedirect(Url.GetLocalUrl(returnUrl));
            }
            */
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
                return LocalRedirect(Url.GetLocalUrl(returnUrl));
            }
            // paranoid
            var leftoverUser = await _userManager.FindByEmailAsync(displayName);
            if (leftoverUser != null)
            {
                await _userManager.DeleteAsync(leftoverUser); // just using this inMemory userstore as a scratch holding pad
            }
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

                return LocalRedirect(Url.GetLocalUrl(returnUrl));

            }

            return RedirectToPage("./Login");
        }

        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    throw new ApplicationException("Error loading external login information during confirmation.");
                }
                var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                        return LocalRedirect(Url.GetLocalUrl(returnUrl));
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ReturnUrl = returnUrl;
            return Page();
        }
    }
}
