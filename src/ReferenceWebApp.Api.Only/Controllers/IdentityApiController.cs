using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

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
}