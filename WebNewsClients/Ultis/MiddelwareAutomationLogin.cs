using BusinessObjects.Models;
using Newtonsoft.Json;
using NuGet.Common;
using WebNewsAPIs.Common;
using WebNewsAPIs.Dtos;
using WebNewsAPIs.Services;

namespace WebNewsClients.Ultis
{
    public class MiddelwareAutomationLogin
    {
        private readonly RequestDelegate _next;
        private readonly IHttpContextAccessor _httpContextAccessor;
        

        public MiddelwareAutomationLogin(RequestDelegate next, IHttpContextAccessor httpContextAccessor)
        {
            _next = next;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Lấy HttpContext từ IHttpContextAccessor
            var httpContext = _httpContextAccessor.HttpContext;
            

           
            if(httpContext.Request.Cookies.ContainsKey(SaveKeySystem.userLogin))
            {
                var jsonSerialUser = httpContext.Request.Cookies[SaveKeySystem.userLogin];
                var userLoginPast = JsonConvert.DeserializeObject<ViewUserDto>(jsonSerialUser);
                if (userLoginPast != null)
                {
                    var userMapper = new User();
                    userMapper.UserId = userLoginPast.UserId;
                    userMapper.Username = userLoginPast.Username;
                    userMapper.Password = userLoginPast.Password;
                    userMapper.DisplayName = userLoginPast.DisplayName;
                    userMapper.Roleid = userLoginPast.RoleId;
                    userMapper.Role = new Role { RoleId = userLoginPast.RoleId, Rolename = userLoginPast.RoleName };

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
                    var jsonSerial = JsonConvert.SerializeObject(userLoginPast);
                    httpContext.Response.Cookies.Append(SaveKeySystem.Authentication, token, cookieOptions);
                    httpContext.Response.Cookies.Append(SaveKeySystem.userLogin, jsonSerial, cookieOptions);
                    // Session
                    httpContext.Session.SetString(SaveKeySystem.userLogin, jsonSerial);
                }
            }
            
           

            await _next(context);
        }
    }
}
