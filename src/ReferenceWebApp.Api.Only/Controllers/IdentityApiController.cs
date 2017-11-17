using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ReferenceWebApp.Controllers
{
    [Area("Api")]
    [Route("api/[controller]")]
    public class IdentityApiController : Controller
    {
        public async Task<ActionResult> Get()
        {
            var jsonResult = new JsonResult(Enumerable.Select(User.Claims, c => new
            {
                c.Type,
                c.Value
            }));
            return jsonResult;
        }
    }

    public class SomeData
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
    [Area("Api2")]
    [Route("api2/[controller]/[action]")]
    public class OpenApiController : Controller
    {
        [HttpGet]
        [ActionName("Testing")]
        public async Task<ActionResult> FetchTesting()
        {
            var jsonResult = new JsonResult(new List<string>(){"a","B"});
            return jsonResult;
        }
        [HttpGet]
        [ActionName("Testing2")]
        public async Task<string> FetchTesting2()
        {
            var json = JsonConvert.SerializeObject(new List<string>() {"a", "B"});
            return json;
        }
        [HttpPost]
        [ActionName("Create")]
        public IActionResult Create([FromBody]SomeData value)
        {
            var jsonResult = new JsonResult(value);
            return jsonResult;
        }
 
    }
}