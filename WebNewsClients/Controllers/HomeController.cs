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

        
        public  IActionResult Index()
        {
            ////$"{BaseUrls.BASE_URL_CategoryArticle}/api/CategoriesArticles/getAllCategory"
            ////Call api của Category
            //string urlOdataAllCategory = "https://localhost:7251/odata/CategoriesArticles?$expand=ParentCategory,InverseParentCategory&$filter=ParentCategory eq null & orderby=OrderLevel";

            //var responseMessage =  _httpClient.GetAsync(urlOdataAllCategory).Result;
            //responseMessage.EnsureSuccessStatusCode();
            //var listCategories = responseMessage.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>()
            //    .Result.data;

            //Call api của Category Root
            string urlOdataAllCategory = "https://localhost:7251/odata/CategoriesArticles?$expand=ParentCategory,InverseParentCategory&orderby=OrderLevel";

            var responseMessage = _httpClient.GetAsync(urlOdataAllCategory).Result;
            responseMessage.EnsureSuccessStatusCode();
            var listCategories = responseMessage.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>()
                .Result.data;
            //
           


            //Call Api lastest new 
            string urlOdataLastestNew = "https://localhost:7251/odata/Articles?$expand=Categorty,AuthorNavigation&$top=20&orderby=CreatedDate desc&$filter=IsPublish eq true and StatusProcess eq 3";
            var responseMessageLastestNew = _httpClient.GetAsync(urlOdataLastestNew).Result;
            responseMessageLastestNew.EnsureSuccessStatusCode();
            var listLastestNews = responseMessageLastestNew.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<Article>>>()
                .Result.data;
            // slide 
            string urlGetAllArticleOfRoot = "https://localhost:7251/api/Articles/GetArticleOfRootCategory";
			var responseMessageAllArticleOfRoot = _httpClient.GetAsync(urlGetAllArticleOfRoot).Result;
			responseMessageLastestNew.EnsureSuccessStatusCode();
            var slide =  responseMessageAllArticleOfRoot.Content.ReadFromJsonAsync<Dictionary<string, List<ViewArticleDto>>>().Result;

            //lấy top 20 bài article có view nhiều nhất
            string urlGetAllArticleHaveViewHighest = "https://localhost:7251/odata/Articles?$expand=Categorty,AuthorNavigation,Comments&$top=10&orderby=ViewArticles desc,CreatedDate desc&$filter=IsPublish eq true and StatusProcess eq 3 and month(CreatedDate) eq month(now()) and year(CreatedDate) eq year(CreatedDate)";
			var responseMessageAllArticleHighest = _httpClient.GetAsync(urlGetAllArticleHaveViewHighest).Result;
			responseMessageAllArticleHighest.EnsureSuccessStatusCode();
			var mostPopular = responseMessageAllArticleHighest.Content.ReadFromJsonAsync<OdataResponse<List<Article>>>().Result.data;
			//Trả dữ liệu về view
			ViewBag.Category = listCategories;
            ViewBag.LastestNews = listLastestNews;
            ViewBag.Slide = slide;
            ViewBag.MostPopular = mostPopular;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

      
        public IActionResult Error400()
        {
            //Call api của Category
            string urlOdataAllCategory = "https://localhost:7251/odata/CategoriesArticles?$expand=ParentCategory,InverseParentCategory&$filter=ParentCategory eq null & orderby=OrderLevel";

            var responseMessage = _httpClient.GetAsync(urlOdataAllCategory).Result;
            responseMessage.EnsureSuccessStatusCode();
            var listCategories = responseMessage.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>()
                .Result.data;
            ViewBag.Category = listCategories;

            string message = TempData["message"] as string;
            ViewBag.message = message;
            return View();
        }
        
        public IActionResult Error500()
        {
            //Call api của Category
            string urlOdataAllCategory = "https://localhost:7251/odata/CategoriesArticles?$expand=ParentCategory,InverseParentCategory&$filter=ParentCategory eq null & orderby=OrderLevel";

            var responseMessage = _httpClient.GetAsync(urlOdataAllCategory).Result;
            responseMessage.EnsureSuccessStatusCode();
            var listCategories = responseMessage.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>()
                .Result.data;
            ViewBag.Category = listCategories;

            string message = TempData["message"] as string;
            ViewBag.message = message;
            return View();
        }
    }
}