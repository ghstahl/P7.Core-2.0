using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace OldSchool.RazorPages
{
    [Route("api/old-school/[controller]")]
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