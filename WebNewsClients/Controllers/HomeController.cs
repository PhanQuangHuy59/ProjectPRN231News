using BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebNewsAPIs.Dtos;
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
            //$"{BaseUrls.BASE_URL_CategoryArticle}/api/CategoriesArticles/getAllCategory"
            //Call api của Category
            string urlOdataAllCategory = "https://localhost:7251/odata/CategoriesArticles?$expand=ParentCategory,InverseParentCategory&$filter=ParentCategory eq null & orderby=OrderLevel";

            var responseMessage =  _httpClient.GetAsync(urlOdataAllCategory).Result;
            responseMessage.EnsureSuccessStatusCode();
            var listCategories = responseMessage.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>()
                .Result.data;
            //Call Api lastest new 
            string urlOdataLastestNew = "https://localhost:7251/odata/Articles?$expand=Categorty,AuthorNavigation&$top=20&orderby=CreatedDate desc&$filter=IsPublish eq true and StatusProcess eq 3";
            var responseMessageLastestNew = _httpClient.GetAsync(urlOdataLastestNew).Result;
            responseMessageLastestNew.EnsureSuccessStatusCode();
            var listLastestNews = responseMessageLastestNew.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<Article>>>()
                .Result.data;
            // slide 
            Dictionary<String, IEnumerable<Article>> slide = new Dictionary<String, IEnumerable<Article>>();
			var groups = listLastestNews.GroupBy(c => c.Categorty.CategoryName);
            foreach(var group in groups)
            {
                var listArticle = new List<Article>();
               foreach(var article in group)
               {
                    listArticle.Add(article);
               }
				slide.Add(group.Key, listArticle.Take(4));
			}
              

            //Trả dữ liệu về view
            ViewBag.Category = listCategories;
            ViewBag.LastestNews = listLastestNews;
            ViewBag.Slide = slide;

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