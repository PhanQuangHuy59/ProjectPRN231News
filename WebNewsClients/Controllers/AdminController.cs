using BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;

using System.Text;
using System.Text.Json;
using WebNewsAPIs.Dtos;

namespace WebNewsClients.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private HttpClient _httpClient;

        public AdminController(ILogger<HomeController> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;

        }
        public IActionResult Index()
        {
            return View();
        }
        #region Thêm Admin Và View
        [HttpGet("AdminList.html")]
        public IActionResult AdminList()
        {
            string urlOdataAdminList = "https://localhost:7251/odata/Users?$expand=Role&$filter=Role/Rolename eq 'Admin'";
            var respondMessage = _httpClient.GetAsync(urlOdataAdminList).Result;
            respondMessage.EnsureSuccessStatusCode();
            var listAdmin = respondMessage.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<User>>>().Result.data.ToList();
            ViewBag.ListAdmin = listAdmin;

            return View("AdminList");
        }
        public IActionResult AdminAdd()
        {
            return View("AdminAdd");
        }
        [HttpPost("AddNewAdmin")]
        public IActionResult AddNewAdmin(AddUserDto user)
        {
            if (ModelState.IsValid)
            {
                string urlRegister = "https://localhost:7251/api/Users";
                var request = new HttpRequestMessage(HttpMethod.Post, urlRegister);
                request.Content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");

                var response = _httpClient.SendAsync(request).Result;
                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = response.Content.ReadAsStringAsync().Result;

                    TempData["err"] = $"Không thể tạo tài khoản hãy thử lại \n {errorMessage}";
                    ViewBag.Message = "Thêm Thất bại";
                    return View("UserAdd", user);
                }
                ViewBag.Message = "Thêm Thành Công";
            }
            return View("AdminAdd");
        }
        #endregion
        #region Thêm người dùng và View
        public IActionResult UserList()
        {
            string urlOdataAdminList = "https://localhost:7251/odata/Users?$expand=Role&$filter=Role/Rolename ne 'Admin'";
            var respondMessage = _httpClient.GetAsync(urlOdataAdminList).Result;
            respondMessage.EnsureSuccessStatusCode();
            var listUser = respondMessage.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<User>>>().Result.data.ToList();
            ViewBag.ListUser = listUser;
            return View("UserList");
        }
        
        public IActionResult UserAdd()
        {
            return View("UserAdd");
        }
        [HttpPost("AddNewUser")]
        public IActionResult AddNewUser(AddUserDto user)
        {
            if (ModelState.IsValid)
            {
                string urlRegister = "https://localhost:7251/api/Users";
                var request = new HttpRequestMessage(HttpMethod.Post, urlRegister);
                request.Content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");

                var response = _httpClient.SendAsync(request).Result;
                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = response.Content.ReadAsStringAsync().Result;

                    TempData["err"] = $"Không thể tạo tài khoản hãy thử lại \n {errorMessage}";
                    ViewBag.Message = "Thêm Thất bại";
                    return View("UserAdd", user);
                }
                ViewBag.Message = "Thêm Thành Công";
            }
            return View("UserAdd");
        }
        #endregion
        #region Thêm bài báo và view
        public IActionResult ArticleList()
        {

            string urlOdataAdminList = "https://localhost:7251/odata/Articles?$expand=AuthorNavigation,Categorty,StatusProcessNavigation";
            var respondMessage = _httpClient.GetAsync(urlOdataAdminList).Result;
            respondMessage.EnsureSuccessStatusCode();
            var listUser = respondMessage.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<Article>>>().Result.data.ToList();
            ViewBag.listArticle = listUser;
            return View("ArticleList");
        }
        public IActionResult ArticleAdd()
        {
            string urlOdataAdminList = "https://localhost:7251/odata/Users?$expand=Role&$filter=Role/Rolename eq 'Articles'";
            var respondMessage = _httpClient.GetAsync(urlOdataAdminList).Result;
            respondMessage.EnsureSuccessStatusCode();
            var listAdmin = respondMessage.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<User>>>().Result.data.ToList();
            ViewBag.ListArticle = listAdmin;

            string urlOdataCategory = "https://localhost:7251/odata/CategoriesArticles?$expand=ParentCategory";
            var respond = _httpClient.GetAsync(urlOdataCategory).Result;
            respond.EnsureSuccessStatusCode();
            var listCate = respond.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>().Result.data.ToList();
            ViewBag.ListCategory = listCate;

            string urlGetProcessor = "https://localhost:7251/odata/Users?$expand=Role&$filter=Role/Rolename eq 'Admin'";
            var respondtoProcessor = _httpClient.GetAsync(urlGetProcessor).Result;
            respondtoProcessor.EnsureSuccessStatusCode();
            var listProcessor = respondtoProcessor.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<User>>>().Result.data.ToList();
            ViewBag.ListProcessor = listProcessor;
            return View("ArticleAdd");
        }
        [HttpPost("AddNewArticle")]
        public IActionResult AddNewArticle(AddArticleDto dto)
        {
            string urlOdataAdminList = "https://localhost:7251/odata/Users?$expand=Role&$filter=Role/Rolename eq 'Articles'";
            var respondMessage = _httpClient.GetAsync(urlOdataAdminList).Result;
            respondMessage.EnsureSuccessStatusCode();
            var listAdmin = respondMessage.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<User>>>().Result.data.ToList();
            ViewBag.ListArticle = listAdmin;

            string urlOdataCategory = "https://localhost:7251/odata/CategoriesArticles?$expand=ParentCategory";
            var respond = _httpClient.GetAsync(urlOdataCategory).Result;
            respond.EnsureSuccessStatusCode();
            var listCate = respond.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>().Result.data.ToList();
            ViewBag.ListCategory = listCate;

            string urlGetProcessor = "https://localhost:7251/odata/Users?$expand=Role&$filter=Role/Rolename eq 'Admin'";
            var respondtoProcessor = _httpClient.GetAsync(urlGetProcessor).Result;
            respondtoProcessor.EnsureSuccessStatusCode();
            var listProcessor = respondtoProcessor.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<User>>>().Result.data.ToList();
            ViewBag.ListProcessor = listProcessor;


            if (ModelState.IsValid)
            {
                string urlRegister = "https://localhost:7251/api/Articles/AddNewArticle";
                var request = new HttpRequestMessage(HttpMethod.Post, urlRegister);
                request.Content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");

                var response = _httpClient.SendAsync(request).Result;
                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = response.Content.ReadAsStringAsync().Result;

                    TempData["err"] = $"Không thể tạo bài báo hãy thử lại \n {errorMessage}";
                    ViewBag.Message = "Thêm Thất bại";
                    return View("ArticleAdd", dto);
                }
                ViewBag.Message = "Thêm Thành Công";
            }
            return View("ArticleAdd");
        }
        #endregion
        public IActionResult ArticleCategoryList()
        {
            string urlOdataAdminList = "https://localhost:7251/odata/CategoriesArticles?$expand=ParentCategory";
            var respondMessage = _httpClient.GetAsync(urlOdataAdminList).Result;
            respondMessage.EnsureSuccessStatusCode();
            var listUser = respondMessage.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<CategoriesArticle>>>().Result.data.ToList();
            ViewBag.ListCategory = listUser;
            return View("ArticleCategoryList");
        }
        public IActionResult ArticleCategoryAdd()
        {
            return View("ArticleCategoryAdd");
        }
        public IActionResult EmotionList()
        {
            string urlOdataAdminList = "https://localhost:7251/odata/Emotions";
            var respondMessage = _httpClient.GetAsync(urlOdataAdminList).Result;
            respondMessage.EnsureSuccessStatusCode();
            var listUser = respondMessage.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<Emotion>>>().Result.data.ToList();
            ViewBag.ListEmotion = listUser;
            return View("EmotionList");
        }
        public IActionResult EmotionAdd()
        {
            return View("EmotionAdd");
        }
    }
}
