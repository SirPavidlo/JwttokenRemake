using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jwttoken.Controllers
{
    //[Authorize(Roles ="admin")]
    [Authorize(Policy = "Admin")]
    public class AdminController : Controller
    {
        public IActionResult Admins()
        {
            //админ
            return View("AdminView");
        }
    }
}
