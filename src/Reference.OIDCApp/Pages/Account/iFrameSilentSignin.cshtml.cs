using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Reference.OIDCApp.Data;

namespace Reference.OIDCApp.Pages.Account
{
    public class iFrameSilentSigninModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        public string ReturnUrl { get; set; }
        public string ErrorUrl { get; set; }
        public string Prompt { get; set; }
        public string Provider { get; set; }
        public iFrameSilentSigninModel(SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }
        public async Task OnGetAsync(string provider = null, string returnUrl = null, string errorUrl = null, string prompt = null)
        {
            Provider = provider == null?"Google": provider;
            ReturnUrl = returnUrl;
            ErrorUrl = errorUrl;
            Prompt = prompt;
        }
    }
}