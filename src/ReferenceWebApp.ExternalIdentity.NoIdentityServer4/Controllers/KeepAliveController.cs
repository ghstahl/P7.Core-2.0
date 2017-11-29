using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ReferenceWebApp.Controllers
{
    [Produces("application/json")]
    [Route("keep-alive")]
    public class KeepAliveController : Controller
    {
        // GET: api/KeepAlive
        [HttpGet]
        public async Task<string> GetAsync()
        {
            return "OK";
        }
    }
}
