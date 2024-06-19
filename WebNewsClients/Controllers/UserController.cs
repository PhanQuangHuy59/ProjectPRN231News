using Microsoft.AspNetCore.Mvc;

namespace WebNewsClients.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult LoginPost(string username, string password)
        {


            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost("Register")]
        public IActionResult RegisterPost()
        {


            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost("ResetPassPost")]
        public IActionResult ResetPasswordPost()
        {
            return View();
        }
        public IActionResult Logout()
        {
            return RedirectToAction("Index","Home");   
        }

    }
}
