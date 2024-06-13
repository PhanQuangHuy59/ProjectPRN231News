using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebNewsClients.Dtos;
using WebNewsClients.HttpClients;
using WebNewsClients.Models;

namespace WebNewsClients.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private HttpClient _httpClient;

        public HomeController(ILogger<HomeController> logger, HttpClient httpClient)
        {
            _logger = logger;
           _httpClient = httpClient;
           
        }

        
        public IActionResult Index()
        {
            //Call api của Category
            var responseMessage =  _httpClient.GetAsync($"{BaseUrls.BASE_URL_CategoryArticle}/api/CategoriesArticles/getAllCategory").Result;
            responseMessage.EnsureSuccessStatusCode();
            var listCategories = responseMessage.Content.ReadFromJsonAsync<IEnumerable<ViewCategoriesArticleDto>>()
                .Result;
            //Trả dữ liệu về view
            ViewBag.Category = listCategories;

            return View();
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}