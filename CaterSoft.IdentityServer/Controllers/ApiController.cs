using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CaterSoft.IdentityServer.Controllers
{
    public class ApiController : Controller
    {

        [Authorize]
        public IActionResult Test()
        {
            return Ok("OK");
        }
    }
}
