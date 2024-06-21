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

        
    }
}
