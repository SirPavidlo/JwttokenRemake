using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jwttoken.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        public IActionResult Users()
        {
            return View("UserView");
        }
    }
}
