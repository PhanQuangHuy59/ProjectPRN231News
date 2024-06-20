using AutoMapper;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using WebNewsAPIs.Common;
using WebNewsAPIs.Dtos;
using WebNewsAPIs.Services;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace WebNewsClients.Controllers
{
    public class UserController : Controller
    {
        private HttpClient _httpClient;
       
        public UserController(HttpClient httpClient) 
        {
            _httpClient = httpClient;
          
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            AddUserDto loginUserDto = new AddUserDto();
            return View(loginUserDto);
        }
        [HttpPost]
        public IActionResult LoginPost(AddUserDto user)
        {
            TempData.Clear();
            if (!ModelState.IsValid)
            {
                var errorMessages = new List<string>();
                foreach (var err in ModelState.Keys)
                {
                    var state = ModelState[err];
                    if (state != null && state.Errors.Count > 0)
                    {
                        errorMessages.Add($"{err}: {state.Errors[0].ErrorMessage}");
                    }
                }
                ModelState.AddModelError(string.Empty, string.Join("\n", errorMessages));
                TempData["err"] = "Dữ liệu có vẻ không hợp lệ";
                return View("Login", user);
            }
            string urlRegister = "https://localhost:7251/api/Users/Login";
            var request = new HttpRequestMessage(HttpMethod.Get, urlRegister);
            request.Content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");
            var response = _httpClient.SendAsync(request).Result;

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = response.Content.ReadAsStringAsync().Result;
                
                TempData["err"] = "Đăng nhập thất bại. " + errorMessage;
                return View("Login", user);
            }
            var userLogin =  response.Content.ReadFromJsonAsync<ViewUserDto>().Result;
            if (userLogin != null)
            {
                var userMapper = new User();
                userMapper.UserId = userLogin.UserId;
                userMapper.Username = userLogin.Username;
                userMapper.Password = userLogin.Password;
                userMapper.Roleid = userLogin.RoleId;
                userMapper.Role = new Role { RoleId = userLogin.RoleId , Rolename = userLogin.RoleName};

                string token = AuthenticationTokent.GenerationToken(userMapper);

                var cookieOptions = new CookieOptions
                {
                    // Set the cookie properties 
                    Path = "/",
                    Expires = DateTime.UtcNow.AddDays(1),
                    Secure = true, // Use "false" if not using HTTPS 
                    HttpOnly = true,
                    SameSite = (Microsoft.AspNetCore.Http.SameSiteMode)SameSiteMode.Strict
                };
                HttpContext.Response.Cookies.Append(SaveKeySystem.Authentication, token, cookieOptions);

                // Session
                var jsonSerial = JsonConvert.SerializeObject(userLogin);
                HttpContext.Session.SetString(SaveKeySystem.userLogin, jsonSerial);


                return RedirectToAction("Index", "Home");
            }
            return View(user);
            
        }


        [HttpGet]
        public IActionResult Register()
        {
            AddUserDto addUserDto = new AddUserDto();
            return View("Register",addUserDto);
        }


        [HttpPost("Register")]
        public IActionResult RegisterPost(AddUserDto user)
        {
            TempData.Clear();
            if (!ModelState.IsValid)
            {
                var errorMessages = new List<string>();
                foreach (var err in ModelState.Keys)
                {
                    var state = ModelState[err];
                    if(state != null && state.Errors.Count > 0)
                    {
                        errorMessages.Add($"{err}: {state.Errors[0].ErrorMessage}");
                    }
                }
                ModelState.AddModelError(string.Empty, string.Join("\n", errorMessages));
                TempData["err"] = "Dữ liệu có vẻ không hợp lệ";
                return View("Register", user);
            }

            VerifyInformation verify = new VerifyInformation();
            string message = verify.IsValidPassword(user.Password, user.Username);
            if (message != "Ok")
            {
                ModelState.AddModelError(nameof(user.Password), message);
                TempData["err"] = "Mật khẩu chưa thỏa mãn hệ thống";
                return View("Register", user);

            }
            string urlRegister = "https://localhost:7251/api/Users";
            var request = new HttpRequestMessage(HttpMethod.Post, urlRegister);
            request.Content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");

            var response = _httpClient.SendAsync(request).Result;

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = response.Content.ReadAsStringAsync().Result;
              
                
                TempData["err"] = $"Không thể tạo tài khoản hãy thử lại \n {errorMessage}";
                return View("Register", user);
            }

            TempData["success"] = "Đã tạo tài khoản thành công Hãy Login vào tài khoản";

            return View("Login",user);
            //return RedirectToAction("Index", "Home");
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
