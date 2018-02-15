using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Reference.OIDCApp.Data;

namespace Reference.OIDCApp.Pages.Account
{
    public class GoogleSilentSigninModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        public string ReturnUrl { get; set; }
        public string Prompt { get; set; }

        public GoogleSilentSigninModel(SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }
        public async Task OnGetAsync(string returnUrl = null, string prompt = null)
        {
            ReturnUrl = returnUrl;
            Prompt = prompt;
        }
    }
}