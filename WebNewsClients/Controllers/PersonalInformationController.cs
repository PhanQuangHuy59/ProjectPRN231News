using BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics.Eventing.Reader;
using System.Net.Http;
using System.Text;
using WebNewsAPIs.Common;
using WebNewsAPIs.Dtos;
using WebNewsClients.Ultis;

namespace WebNewsClients.Controllers
{
    public class PersonalInformationController : Controller
    {
        private HttpClient _httpClient;
        public PersonalInformationController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("PersonalInformation.html")]
        public IActionResult PersonalInformationOfUser()
        {
            ViewUserDto userLogin =InformationLogin. getUserLogin(HttpContext);
            if (userLogin == null)
            {
                return RedirectToAction("Index","Home");
            }
            string urlRegister = $"https://localhost:7251/odata/Users?$expand=Role&$filter=IsConfirm eq true & UserId eq {userLogin.UserId}";
            var response = _httpClient.GetAsync(urlRegister).Result;
            if(!response.IsSuccessStatusCode)
            {
                TempData["err"] = "Không thể lấy được thông tin của người dùng";
                return RedirectToAction("Index", "Home");
            }
            var userResponse = response.Content.ReadFromJsonAsync<OdataResponse<List<User>>>().Result.data;
            if(userResponse.Count == 0)
            {
                TempData["warning"] = "Không có tài khoản nào phù hợp với phiên đăng nhập của bạn";
                return RedirectToAction("Index", "Home");
            }
            ViewBag.InformationUser = userResponse[0];

            return View();
        }


        public IActionResult PersonalArticleComment()
        {
            //Lay thong tin tai khoan khi nguoi dung dang nhap
            ViewUserDto userLogin = InformationLogin.getUserLogin(HttpContext);
            if (userLogin == null)
            {
                return RedirectToAction("Index", "Home");
            }
            string urlRegister = $"https://localhost:7251/odata/Users?$expand=Role&$filter=IsConfirm eq true & UserId eq {userLogin.UserId}";
            var response = _httpClient.GetAsync(urlRegister).Result;
            if (!response.IsSuccessStatusCode)
            {
                TempData["err"] = "Không thể lấy được thông tin của người dùng";
                return RedirectToAction("Index", "Home");
            }
            var userResponse = response.Content.ReadFromJsonAsync<OdataResponse<List<User>>>().Result.data;
            if (userResponse.Count == 0)
            {
                TempData["warning"] = "Không có tài khoản nào phù hợp với phiên đăng nhập của bạn";
                return RedirectToAction("Index", "Home");
            }

            var urlListComment = $"https://localhost:7251/odata/Comments?$filter=UserId eq {userLogin.UserId}&$expand=Article,User&$orderby=CreateDate desc";
            var responseListComments = _httpClient.GetAsync(urlListComment).Result;
            if (!responseListComments.IsSuccessStatusCode)
            {
                TempData["err"] = "Đã xảy ra lỗi trong quá trình truy cập dữ liệu";
                return View();
            }
            var listComment = responseListComments.Content.ReadFromJsonAsync<OdataResponse<List<Comment>>>().Result.data;


            //Tra ve phia view
            ViewBag.InformationUser = userResponse[0];
            ViewBag.Comments = listComment;
            return View();
        }

        public IActionResult ArticleViewed()
        {

            ViewUserDto userLogin = InformationLogin.getUserLogin(HttpContext);
            if (userLogin == null)
            {
                return RedirectToAction("Index", "Home");
            }
            string urlRegister = $"https://localhost:7251/odata/Users?$expand=Role&$filter=IsConfirm eq true & UserId eq {userLogin.UserId}";
            var response = _httpClient.GetAsync(urlRegister).Result;
            if (!response.IsSuccessStatusCode)
            {
                TempData["err"] = "Không thể lấy được thông tin của người dùng";
                return RedirectToAction("Index", "Home");
            }
            var userResponse = response.Content.ReadFromJsonAsync<OdataResponse<List<User>>>().Result.data;
            if (userResponse.Count == 0)
            {
                TempData["warning"] = "Không có tài khoản nào phù hợp với phiên đăng nhập của bạn";
                return RedirectToAction("Index", "Home");
            }

            //Lấy ra danh sách các bài báo mà người dùng đã xem gần nhất


            var urlListArticleView= $"https://localhost:7251/odata/Views?$expand=Article&filter=UserId eq {userLogin.UserId}&$orderby=ViewDate desc";
            var responseListViewed = _httpClient.GetAsync(urlListArticleView).Result;
            if (!responseListViewed.IsSuccessStatusCode)
            {
                TempData["err"] = "Đã xảy ra lỗi trong quá trình truy cập dữ liệu";
                return View();
            }
            var listArticleViewed = responseListViewed.Content.ReadFromJsonAsync<OdataResponse<List<View>>>().Result.data.ToList();
            var groupViewArticle = listArticleViewed.GroupBy(c => c.ViewDate.Date);
           
           


            //Tra ve phia view
            ViewBag.InformationUser = userResponse[0];
            ViewBag.ArticleViews = groupViewArticle;
            return View();
        }
        public IActionResult ArticleSaved()
        {
            ViewUserDto userLogin = InformationLogin.getUserLogin(HttpContext);
            if (userLogin == null)
            {
                return RedirectToAction("Index", "Home");
            }
            string urlRegister = $"https://localhost:7251/odata/Users?$expand=Role&$filter=IsConfirm eq true & UserId eq {userLogin.UserId}";
            var response = _httpClient.GetAsync(urlRegister).Result;
            if (!response.IsSuccessStatusCode)
            {
                TempData["err"] = "Không thể lấy được thông tin của người dùng";
                return RedirectToAction("Index", "Home");
            }
            var userResponse = response.Content.ReadFromJsonAsync<OdataResponse<List<User>>>().Result.data;
            if (userResponse.Count == 0)
            {
                TempData["warning"] = "Không có tài khoản nào phù hợp với phiên đăng nhập của bạn";
                return RedirectToAction("Index", "Home");
            }

            //Lấy ra danh sách các bài báo mà người dùng đã xem gần nhất


            var urlListArticleView = $"https://localhost:7251/odata/SaveArticles?$expand=Article&filter=UserId eq {userLogin.UserId}&$orderby=SaveDate desc";
            var responseListViewed = _httpClient.GetAsync(urlListArticleView).Result;
            if (!responseListViewed.IsSuccessStatusCode)
            {
                TempData["err"] = "Đã xảy ra lỗi trong quá trình truy cập dữ liệu";
                return View();
            }
            var listArticleViewed = responseListViewed.Content.ReadFromJsonAsync<OdataResponse<List<SaveArticle>>>().Result.data.ToList();
            var groupViewArticle = listArticleViewed.GroupBy(c => c.SaveDate.Date);


            //Tra ve phia view
            ViewBag.InformationUser = userResponse[0];
            ViewBag.ArticleSaves = groupViewArticle;
            return View();
        }




    }
}
