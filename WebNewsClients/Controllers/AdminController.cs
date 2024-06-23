using Microsoft.AspNetCore.Mvc;

namespace WebNewsClients.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AdminList()
        {
            return View("AdminList");
        }
        public IActionResult AdminAdd()
        {
            return View("AdminAdd");
        }
        public IActionResult UserList()
        {
            return View("UserList");
        }
        public IActionResult UserAdd()
        {
            return View("UserAdd");
        }
        public IActionResult ArticleList()
        {
            return View("ArticleList");
        }
        public IActionResult ArticleAdd()
        {
            return View("ArticleAdd");
        }
    }
}
