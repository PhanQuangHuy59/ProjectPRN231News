using AutoMapper;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
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
        private IMapper _mapper;
       
        public UserController(HttpClient httpClient, IMapper mapper) 
        {
            _httpClient = httpClient;
            _mapper = mapper;
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
        public IActionResult ResetPassword(string? email)
        {
            if (email == null || string.IsNullOrEmpty(email.Trim()))
            {
                TempData["error"] = "Hãy kiểm tra email của bạn không được để tr để resetpassword";
                ViewBag.email = email;
                return View();
            }
            string urlCheckUser = $"https://localhost:7251/odata/Users?$top=1&$expand=Role&$filter=Username eq '{email}'";
            var response = _httpClient.GetAsync(urlCheckUser).Result;
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = response.Content.ReadAsStringAsync().Result;
                TempData["err"] = $"Hệ thống đã xảy ra lỗi :\n {errorMessage}";
                ViewBag.email = email;
                return View();
            }
            var userResponse = response.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<User>>>().Result.data.ToList();
            if (userResponse.Count == 0)
            {
                TempData["err"] = $"Trong hệ thống không có tài khoản nào có email này.";
                ViewBag.email = email;
                return View();
            }
            var user = userResponse[0];

            string urlSendmailResetPass = "https://localhost:7251/api/Users/SendMailResetPassword";
            var request = new HttpRequestMessage(HttpMethod.Post, urlSendmailResetPass);
            request.Content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");

            var responseCodeSendMail = _httpClient.SendAsync(request).Result;
            if (!responseCodeSendMail.IsSuccessStatusCode)
            {
                TempData["err"] = responseCodeSendMail.Content.ReadAsStringAsync().Result;
                ViewBag.email = email;
                return View();
            }
            TempData["success"] = "Hãy kiểm tra email của bạn để resetpassword";
            ViewBag.email = email;
            return View();
        }
        [HttpGet]
        public IActionResult ConfirmResetPassword(string userId, string code)
        {
            if (userId == null || code == null)
            {
                TempData["err"] = "Thông tin truyền bị bỏ trống";
                return RedirectToAction("Error400", "Home");
            }
           
            string urlCheckUser = $"https://localhost:7251/odata/Users?$top=1&$expand=Role&$filter=UserId eq {Guid.Parse(userId)}";
            var response = _httpClient.GetAsync(urlCheckUser).Result;

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = response.Content.ReadAsStringAsync().Result;
                TempData["err"] = $"Hệ thống đã xảy ra lỗi :\n {errorMessage}";
                return RedirectToAction("Error500","Home");
            }
            var userResponse = response.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<User>>>().Result.data.ToList();
            if (userResponse.Count == 0)
            {
                TempData["err"] = $"Thông tin của người dùng cung cấp không chính xác trong link.";
                return RedirectToAction("Error400", "Home");
            }
            var user = userResponse[0];

            var idFromCode = AuthenticationTokent.ConfirmEmail(user, code);
            if (!userId.Equals(idFromCode.ToString()))
            {
                TempData["err"] = $"\"đường link Code của bạn không hợp lệ. Hãy Confirm email lại\".";
                return RedirectToAction("Error400", "Home");
            }
            ViewBag.userId = userId;    
            ViewBag.code = code;

            return View();
        }

        [HttpPost]
        public IActionResult ConfirmResetPassword(string userId, string code, string newPassword, string confirmPassword)
        {
            if (userId == null || code == null)
            {
                TempData["err"] = "Thông tin truyền bị bỏ trống";
                return View();
            }

            string urlCheckUser = $"https://localhost:7251/odata/Users?$top=1&$expand=Role&$filter=UserId eq {Guid.Parse(userId)}";
            var response = _httpClient.GetAsync(urlCheckUser).Result;

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = response.Content.ReadAsStringAsync().Result;
                TempData["err"] = $"Hệ thống đã xảy ra lỗi :\n {errorMessage}";
                return  View(); ;
            }
            var userResponse = response.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<User>>>().Result.data.ToList();
            if (userResponse.Count == 0)
            {
                TempData["err"] = $"Thông tin của người dùng cung cấp không chính xác trong link.";
                return  View(); ;
            }
            var user = userResponse[0];

            var idFromCode = AuthenticationTokent.ConfirmEmail(user, code);
            if (!userId.Equals(idFromCode.ToString()))
            {
                TempData["err"] = $"\"Code của bạn không hợp lệ. Hãy Confirm email lại\".";
                return View(); 
            }

            VerifyInformation verify = new VerifyInformation();
            var ok = verify.IsValidPassword(newPassword, user.Username);
            if(ok != "Ok")
            {
                TempData["err"] = ok;
                return View();
            }


            // Call Api to reset password 
            string urlUpdatePassword = $"https://localhost:7251/api/Users/{user.UserId}";
            var requestResetPassword = new HttpRequestMessage(HttpMethod.Put, urlUpdatePassword);
            user.Password = newPassword;
            var userUdpate = _mapper.Map<UpdateUserDto>(user);
            requestResetPassword.Content = new StringContent(JsonSerializer.Serialize(userUdpate), Encoding.UTF8, "application/json");

            var responseCodeResetPassword = _httpClient.SendAsync(requestResetPassword).Result;
            if (!responseCodeResetPassword.IsSuccessStatusCode)
            {
                TempData["err"] ="Đã xảy ra lỗi : " +  responseCodeResetPassword.Content.ReadAsStringAsync().Result;
                return View();
            }
            TempData["success"] = "Dã cập nhật thành công password";

            return View();
        }
        
        public IActionResult ConfirmAccount(string email)
        {
            if (email == null || string.IsNullOrEmpty(email.Trim()))
            {
                TempData["error"] = "Hãy kiểm tra email của bạn không được để tr để resetpassword";
                ViewBag.email = email;
                return View();
            }
            string urlCheckUser = $"https://localhost:7251/odata/Users?$top=1&$expand=Role&$filter=Username eq '{email}'";
            var response = _httpClient.GetAsync(urlCheckUser).Result;
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = response.Content.ReadAsStringAsync().Result;
                TempData["err"] = $"Hệ thống đã xảy ra lỗi :\n {errorMessage}";
                ViewBag.email = email;
                return View();
            }
            var userResponse = response.Content.ReadFromJsonAsync<OdataResponse<IEnumerable<User>>>().Result.data.ToList();
            if (userResponse.Count == 0)
            {
                TempData["err"] = $"Trong hệ thống không có tài khoản nào có email này.";
                ViewBag.email = email;
                return View();
            }
            var user = userResponse[0];
            string urlConfirmAccount = $"https://localhost:7251/api/Users/SendMailConfirmAccount";
            var requestConfirmAccount= new HttpRequestMessage(HttpMethod.Get, urlConfirmAccount);
            requestConfirmAccount.Content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");
            var responseCodeConfirmAccount = _httpClient.SendAsync(requestConfirmAccount).Result;
            if (!responseCodeConfirmAccount.IsSuccessStatusCode)
            {
                TempData["err"] = "Đã xảy ra lỗi : " + responseCodeConfirmAccount.Content.ReadAsStringAsync().Result;
                return View();
            }
            ViewBag.email = email;
            TempData["success"] = "Hãy kiểm tra email của bạn";

            return View();
        }
        public IActionResult NotificationConfirmAccount(Guid userId, string code)
        {

            
            string urlConfirmAccount = $"https://localhost:7251/api/Users/ConfirmEmail?userId={userId}&code={code}";
            var requestConfirmAccount = new HttpRequestMessage(HttpMethod.Get, urlConfirmAccount);
            var responseCodeConfirmAccount = _httpClient.SendAsync(requestConfirmAccount).Result;
            if (!responseCodeConfirmAccount.IsSuccessStatusCode)
            {
                TempData["err"] = "Đã xảy ra lỗi : " + responseCodeConfirmAccount.Content.ReadAsStringAsync().Result;
                return View("ConfirmAccount");
            }
            TempData["success"] = responseCodeConfirmAccount.Content.ReadAsStringAsync().Result;


            return View("ConfirmAccount");
        }

        public IActionResult Logout()
        {
            return RedirectToAction("Index","Home");   
        }

    }
    public class ResetPassword
    {
        public string Email { get; set; }
    }
}
