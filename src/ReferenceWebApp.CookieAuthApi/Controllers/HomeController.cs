using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ReferenceWebApp.CookieAuthApi.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        // GET api/values
        [HttpGet]
        public async Task<string> Get()
        {
            return $"Howdy! {Request.Scheme}://{Request.Host.Value}";
        }
    }
}
