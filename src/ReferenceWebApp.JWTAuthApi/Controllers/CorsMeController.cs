using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ReferenceWebApp.JWTAuthApi.Controllers
{
    class EchoPayload
    {
        public string Message { get; set; }
    }
    [Produces("application/json")]
    [Route("api/cors-me")]
    public class CorsMeController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        [Route("redirect-to")]
        public async Task<IActionResult> GetRedirectToAsync(string url)
        {

            return new RedirectResult(url);
        }

        [HttpOptions]
        [AllowAnonymous]
        [Route("redirect-to")]
        public async Task OptionsRedirectToAsync(string url)
        {
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("echo")]
        public async Task<IActionResult> GetEchoAsync(string message)
        {
            return Json(new EchoPayload() { Message = message });
        }

        [HttpOptions]
        [AllowAnonymous]
        [Route("echo")]
        public async Task OptionsEchoAsync(string message)
        {
        }
    }
}