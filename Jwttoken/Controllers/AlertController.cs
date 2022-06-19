using Microsoft.AspNetCore.Mvc;

namespace Jwttoken.Controllers
{
    public class AlertController : Controller
    {
        public IActionResult Index()
        {
            TempData["WrongPassword"] = 0;
            return View();
            
        }
    }
}
