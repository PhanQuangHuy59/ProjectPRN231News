using AutoMapper;
using BCrypt.Net;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebNewsAPIs.Dtos;
using BusinessObjects.Models;
using WebNewsAPIs.Services;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using Microsoft.AspNetCore.Components;

namespace WebNewsAPIs.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserRepository _userRepo;
        private IArticleRepository _articleRepo;
        private IViewRepository _viewRepo;
        private IMapper _mapper;
        private IEmailSender _emailSender;
        private ILogger<UsersController> _logger;

        public UsersController(IUserRepository userRepo, IMapper mapper,
            IEmailSender emailSender, ILogger<UsersController> logger,
            IArticleRepository articleRepo, IViewRepository viewRepo)
        {
            _userRepo = userRepo;
            _mapper = mapper;
            _emailSender = emailSender;
            _logger = logger;
            _articleRepo = articleRepo;
            _viewRepo = viewRepo;
        }
        [EnableQuery]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> Get()
        {
            if (_userRepo == null || _mapper == null || _emailSender == null)
            {
                return StatusCode(500, "Hệ thống api đang bảo trì.");
            }
            var listUser = _userRepo.GetAll();
            return Ok(listUser.AsQueryable());
        }


        [HttpPost]
        public async Task<ActionResult<AddUserDto>> Post(AddUserDto addUser)
        {
            if (_userRepo == null || _mapper == null || _emailSender == null)
            {
                return StatusCode(500, "Hệ thống api đang bảo trì.");
            }
            if (addUser == null)
            {
                return BadRequest();
            }
            var checkExistAccount = _userRepo.GetSingleByCondition(c => c.Username.ToLower() == addUser.Username.ToLower()).Result;
            if (checkExistAccount != null)
            {
                return StatusCode(405, "Tài khoản đã tồn tại xin hãy tạo tài khoản mới");
            }
            var userRegister = _mapper.Map<User>(addUser);
            userRegister.Password = BCrypt.Net.BCrypt.HashPassword(userRegister.Password);
            userRegister.Createddate = DateTime.Now;
            if (userRegister.Roleid.ToString() == "00000000-0000-0000-0000-000000000000")
            {
                userRegister.Roleid = Guid.Parse("179d557c-d9cc-4504-b869-a4319792f631");
            }
            try
            {
                userRegister = _userRepo.AddAsync(userRegister).Result;
            }
            catch (Exception ex)
            {
                _logger.LogError("User Register Fail");
                return BadRequest(ex.Message);
            }
            // lay user moi login vao kem them role
            var includes = new string[]
                {
                    "Role"
                };

            var code = AuthenticationTokent.GeneraterCodeTokent(userRegister);
            var url = Url.Action("ConfirmEmail", "Users", values: new { userId = userRegister.UserId, code = code }
            , protocol: Request.Scheme);


            await _emailSender.SendEmailAsync(userRegister.Username, "Thông Báo tạo tài khoảng thành Công!", WebNewsAPIs.Services.HtmlHelper.GetHtmlForSendMailRegister(userRegister, code));
            return _mapper.Map<AddUserDto>(userRegister);
        }
        [HttpGet("ConfirmEmail")]
        public IActionResult ConfirmEmail(Guid userId, string code)
        {
            if (userId == null || code == null)
            {
                return BadRequest();
            }
            var user = _userRepo.GetSingleByCondition(c => c.UserId.Equals(userId)).Result;
            if (user == null)
            {
                return NotFound($"Khong thể load được thông tin của bạn có id :'{userId}'.");
            }

            var idFromCode = AuthenticationTokent.ConfirmEmail(user, code);
            if (!userId.Equals(idFromCode))
            {
                return StatusCode(404, "Code của bạn không hợp lệ. Hãy Confirm email lại");
            }
            if (!user.IsConfirm)
            {
                user.IsConfirm = true;
                _userRepo.UpdateAsync(user);
            }
            // Logic to confirm email
            return Ok("Đã confirm thành công bạn có thể đăng nhập lại.");
        }

        [HttpGet("SendMailConfirmAccount")]
        public async Task<IActionResult> SendMailConfirmAccountAsync(User user)
        {
            try
            {
                var code = AuthenticationTokent.GeneraterCodeTokent(user);
                var url = $"https://localhost:7069/User/NotificationConfirmAccount?userId={user.UserId}&code={code}";

                await _emailSender.SendEmailAsync(user.Username, "Thông Báo confirm account!", $"<h3>Confirm Account</h3>\r\n                    <a href=\"{url}\">CLick To ConfirmAccount</a>");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Gửi email không thành công xin hãy làm lại");
            }

            return Ok();
        }


        [HttpPost("SendMailResetPassword")]
        public async Task<IActionResult> SendMailResetPasswordAsync(User user)
        {
            try
            {
                var code = AuthenticationTokent.GeneraterCodeTokent(user);
                var url = $"https://localhost:7069/User/ConfirmResetPassword?userId={user.UserId}&code={code}";
                await _emailSender.SendEmailAsync(user.Username, "Thông Báo reset password!", $"<h3>Password Reset</h3>\r\n                    <a href=\"{url}\">CLick To ResetPassword</a>");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Gửi email không thành công xin hãy làm lại");
            }

            return Ok();
        }


        [HttpPut("{userId}")]
        public async Task<IActionResult> Update(Guid userId, UpdateUserDto user)
        {
            if (user == null)
            {
                return BadRequest();
            }
            if (!userId.Equals(user.UserId))
            {
                return NotFound();
            }
            var userCheck = _userRepo.GetSingleByCondition(c => c.UserId.Equals(userId)).Result;

            try
            {
                userCheck.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                await _userRepo.UpdateAsync(userCheck);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Update không thành công!");
            }

            return Ok();
        }
        //Update display name 
        [HttpPut("UpdateDisplayName")]
        public async Task<ActionResult<ViewUserDto>> UpdateDisplayName(Guid? userId, string displayName)
        {
            string[] includes = new string[]
            {
                nameof(BusinessObjects.Models.User.Role)
            };
            if (userId == null || string.IsNullOrEmpty(displayName))
            {
                return BadRequest();
            }

            var userCheck = _userRepo.GetSingleByCondition(c => c.UserId.Equals(userId), includes).Result;

            if (userCheck == null)
            {
                return StatusCode(404, "Không thể tìm thấy người dùng để thay đổi Display Name");
            }
            userCheck.DisplayName = displayName;

            try
            {
                await _userRepo.UpdateAsync(userCheck);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Update không thành công!");
            }
            var responseToView = _mapper.Map<ViewUserDto>(userCheck);
            return Ok(responseToView);
        }
        [HttpPut("UpdateInformationBasic")]
        public async Task<ActionResult<ViewUserDto>> UpdateÌnormationBasic(Guid? userId, string? phoneNumber, DateTime? dateOfBirth, string? gioiTinh, string? address)
        {
            string[] includes = new string[]
            {
                nameof(BusinessObjects.Models.User.Role)
            };
            string[] gender = { "Nam", "Nữ", "Khác" };
            if (userId == null)
            {
                return BadRequest();
            }

            var userCheck = _userRepo.GetSingleByCondition(c => c.UserId.Equals(userId), includes).Result;

            if (userCheck == null)
            {
                return StatusCode(404, "Không thể tìm thấy người dùng để Update");
            }
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                userCheck.PhoneNumber = phoneNumber;
            }
            if (!string.IsNullOrEmpty(gioiTinh) && gender.Contains(gioiTinh))
            {
                userCheck.Gender = gioiTinh;
            }
            if (dateOfBirth != null)
            {
                userCheck.DateOfBirth = dateOfBirth;
            }
            userCheck.Address = address;

            try
            {
                await _userRepo.UpdateAsync(userCheck);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Update không thành công!");
            }
            var responseToView = _mapper.Map<ViewUserDto>(userCheck);
            return Ok(responseToView);
        }


        [HttpPut("ChangePassword")]
        public async Task<ActionResult<ViewUserDto>> ChangePassword(Guid? userId, string? oldPassword, string? newPassword)
        {
            VerifyInformation verify = new VerifyInformation();


            string[] includes = new string[]
            {
                nameof(BusinessObjects.Models.User.Role)
            };
            if (userId == null || oldPassword == null || newPassword == null)
            {
                return BadRequest();
            }
            var userCheck = _userRepo.GetSingleByCondition(c => c.UserId.Equals(userId), includes).Result;
            if (userCheck == null)
            {
                return StatusCode(404, "Khong tìm thấy người dùng nào hợp lệ");
            }

            if (!BCrypt.Net.BCrypt.Verify(oldPassword, userCheck.Password))
            {
                return StatusCode(405, "Mật khẩu cũ không trùng khớp");
            }
            string ok = verify.IsValidPassword(newPassword, userCheck.Username);
            if (ok != "Ok")
            {
                return StatusCode(406, ok);
            }

            userCheck.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);

            try
            {
                await _userRepo.UpdateAsync(userCheck);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Thay đổi mật khẩu thành công!");
            }
            var responseToView = _mapper.Map<ViewUserDto>(userCheck);
            return Ok(responseToView);
        }

        [HttpPut("ChangeAvata")]
        public async Task<ActionResult<ViewUserDto>> ChangePassword(Guid? userId, string? urlImage)
        {

            string[] includes = new string[]
            {
                nameof(BusinessObjects.Models.User.Role)
            };
            if (userId == null || urlImage == null)
            {
                return BadRequest();
            }
            var userCheck = _userRepo.GetSingleByCondition(c => c.UserId.Equals(userId), includes).Result;
            if (userCheck == null)
            {
                return StatusCode(404, "Khong tìm thấy người dùng nào hợp lệ");
            }

            userCheck.Image = urlImage;

            try
            {
                await _userRepo.UpdateAsync(userCheck);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Thay đổi mật khẩu thành công!");
            }
            var responseToView = _mapper.Map<ViewUserDto>(userCheck);
            return Ok(responseToView);
        }

        [HttpPost("AddArticleToViewUser")]
        public async Task<ActionResult<ViewUserDto>> AddArticleToViewUser(Guid? userId, Guid? articleId)
        {



            string[] includes = new string[]
            {
                nameof(BusinessObjects.Models.User.Role)
            };

            var userCheck = _userRepo.GetSingleByCondition(c => c.UserId.Equals(userId)).Result;
            var articleCheck = _articleRepo.GetSingleByCondition(c => c.ArticleId.Equals(articleId)).Result;
            var viewCheck = _viewRepo.GetSingleByCondition(c => c.ArticleId.Equals(articleId) && c.UserId.Equals(userId)).Result;
            if (viewCheck != null)
            {
                return Ok();
            }

            var listArticleViewOfUser = _viewRepo.GetMulti(c => c.UserId.Equals(userId.Value)).OrderBy(c => c.ViewDate).ToList();
            if (listArticleViewOfUser.Count > 100)
            {
                var listViewDelete = listArticleViewOfUser.Skip(99);
                _viewRepo.DeleteListView(listViewDelete);
            }

            var viewAdd = new View
            {
                ArticleId = articleId.Value,
                Article = articleCheck,
                UserId = userId.Value,
                User = userCheck,
                ViewDate = DateTime.Now
            };

            try
            {
                await _viewRepo.AddAsync(viewAdd);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Thêm Không thanh Cong Cho View of user");
            }

            return Ok();
        }


        [HttpGet("Login")]
        public async Task<ActionResult<ViewUserDto>> Login(AddUserDto userDto)
        {
            if (_userRepo == null || _mapper == null || _emailSender == null)
            {
                return StatusCode(500, "Hệ thống api đang bảo trì.");
            }
            if (userDto == null)
            {
                return BadRequest();
            }
            string[] includes = new string[]
            {
                nameof(BusinessObjects.Models.User.Role)
            };
            var listUserLogin = _userRepo.GetMulti(c => c.Username.ToLower().Equals(userDto.Username.ToLower()) && c.IsConfirm == true, includes).ToList();
            if (listUserLogin == null || listUserLogin.Count == 0)
            {
                return StatusCode(404, "Tài khoản không tồn tại trong hệ thống.");
            }
            User userLogin = null;
            foreach (var user in listUserLogin)
            {
                var checkPass = BCrypt.Net.BCrypt.Verify(userDto.Password, user.Password);
                if (checkPass == true)
                {
                    userLogin = user;
                    break;
                }
            }
            if (userLogin == null)
            {
                return StatusCode(404, "Mật khẩu của bạn bị sai hãy nhập lại mật khẩu");
            }




            return _mapper.Map<ViewUserDto>(userLogin);
        }



    }
}
