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


        public async Task<IActionResult> Index()
        {
            ////$"{BaseUrls.BASE_URL_CategoryArticle}/api/CategoriesArticles/getAllCategory"
            ////Call api của Category
            //string urlOdataAllCategory = "https://localhost:8080/odata/CategoriesArticles?$expand=ParentCategory,InverseParentCategory&$filter=ParentCategory eq null & orderby=OrderLevel";

            //var responseMessage =  _httpClient.GetAsync(urlOdataAllCategory).Result;
            //responseMessage.EnsureSuccessStatusCode();
            //var listCategories = responseMessage.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>()
            //    .Result.data;

            //Call api của Category Root
            string urlOdataAllCategory = "https://localhost:8080/odata/CategoriesArticles?$expand=ParentCategory,InverseParentCategory&orderby=OrderLevel";

            var responseMessage = await _httpClient.GetAsync(urlOdataAllCategory);
            responseMessage.EnsureSuccessStatusCode();
            var listCategories1 = await responseMessage.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>();
            var listCategories = listCategories1.data;
            //



            //Call Api lastest new 
            string urlOdataLastestNew = "https://localhost:8080/odata/Articles?$expand=Categorty,AuthorNavigation,Comments & $top=20&orderby=CreatedDate desc&$filter=IsPublish eq true and StatusProcess eq 3";
            var responseMessageLastestNew = await _httpClient.GetAsync(urlOdataLastestNew);
            responseMessageLastestNew.EnsureSuccessStatusCode();
            var listLastestNews1 = await responseMessageLastestNew.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<Article>>>();
            var listLastestNews = listLastestNews1.data;
            // slide 
            string urlGetAllArticleOfRoot = "https://localhost:8080/api/Articles/GetArticleOfRootCategory";
            var responseMessageAllArticleOfRoot = await _httpClient.GetAsync(urlGetAllArticleOfRoot);
            responseMessageLastestNew.EnsureSuccessStatusCode();
            var slide = await responseMessageAllArticleOfRoot.Content.ReadFromJsonAsync<Dictionary<string, List<ViewArticleDto>>>();

            //lấy top 20 bài article có view nhiều nhất  &$filter=IsPublish eq true and StatusProcess eq 3 and month(CreatedDate) eq month(now()) and year(CreatedDate) eq year(CreatedDate)"
            string urlGetAllArticleHaveViewHighest = "https://localhost:8080/odata/Articles?$expand=Categorty,AuthorNavigation,Comments&$top=10&orderby=ViewArticles desc,CreatedDate desc";
            var responseMessageAllArticleHighest = await _httpClient.GetAsync(urlGetAllArticleHaveViewHighest);
            responseMessageAllArticleHighest.EnsureSuccessStatusCode();
            var mostPopular1 = await responseMessageAllArticleHighest.Content.ReadFromJsonAsync<OdataResponse<List<Article>>>();
            var mostPopular = mostPopular1.data;

            string urlListArticleContactHighest = "https://localhost:8080/api/Articles/GetTopArticleContactHighest?take=20";
			var listArticleContactHighest = await _httpClient.GetFromJsonAsync<List<ViewArticleDto>>(urlListArticleContactHighest);
			

			//Trả dữ liệu về view
			ViewBag.Category = listCategories;
            ViewBag.LastestNews = listLastestNews;
            ViewBag.Slide = slide;
            ViewBag.HighestContact = listArticleContactHighest;
            ViewBag.MostPopular = mostPopular;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


        public async Task<IActionResult> Error400()
        {
            //Call api của Category
            string urlOdataAllCategory = "https://localhost:8080/odata/CategoriesArticles?$expand=ParentCategory,InverseParentCategory&$filter=ParentCategory eq null & orderby=OrderLevel";

            var responseMessage =await _httpClient.GetAsync(urlOdataAllCategory);
            responseMessage.EnsureSuccessStatusCode();
            var listCategories1 = await responseMessage.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>();
            var listCategories = listCategories1.data;

            ViewBag.Category = listCategories;

            string message = TempData["message"] as string;
            ViewBag.message = message;
            return View();
        }

        public async Task<IActionResult> Error500()
        {
            //Call api của Category
            string urlOdataAllCategory = "https://localhost:8080/odata/CategoriesArticles?$expand=ParentCategory,InverseParentCategory&$filter=ParentCategory eq null & orderby=OrderLevel";

            var responseMessage =await _httpClient.GetAsync(urlOdataAllCategory);
            responseMessage.EnsureSuccessStatusCode();
            var listCategories1 = await responseMessage.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>();
            var listCategories = listCategories1.data;    


            ViewBag.Category = listCategories;

            return View();
        }
    }
}