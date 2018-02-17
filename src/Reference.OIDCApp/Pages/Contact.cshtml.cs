using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Reference.OIDCApp.Pages
{
    public class ContactModel : PageModel
    {
        public string Message { get; set; }

        public void OnGet()
        {
            var xTest = HttpContext.Session.GetString("x-test");
            Message = $"Your contact page. {xTest}";
        }
    }
}
