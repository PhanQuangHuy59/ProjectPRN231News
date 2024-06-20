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

namespace WebNewsAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserRepository _userRepo;
        private IMapper _mapper;
        private IEmailSender _emailSender;
        private ILogger<UsersController> _logger;

        public UsersController(IUserRepository userRepo, IMapper mapper,
            IEmailSender emailSender, ILogger<UsersController> logger)
        {
            _userRepo = userRepo;
            _mapper = mapper;
            _emailSender = emailSender;
            _logger = logger;
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


            //await _emailSender.SendEmailAsync(userRegister.Username, "Thông Báo tạo tài khoảng thành Công!", WebNewsAPIs.Services.HtmlHelper.GetHtmlForSendMailRegister(userRegister, code));
            return _mapper.Map<AddUserDto>(userRegister);
        }
        [HttpGet("ConfirmEmail")]
        public IActionResult ConfirmEmail(Guid userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }
            var user = _userRepo.GetSingleByCondition(c=> c.UserId.Equals(userId)).Result;
            if (user == null)
            {
                return NotFound($"Khong thể load được thông tini của bạn có id :'{userId}'.");
            }

            var idFromCode = AuthenticationTokent.ConfirmEmail(user, code);
            if (!userId.Equals(idFromCode))
            {
                return StatusCode(404, "Code của bạn không hợp lệ. Hãy Confirm email lại");
            }
            if(!user.IsConfirm)
            {
                _userRepo.UpdateAsync(user);
            }
            // Logic to confirm email
            return Ok("Đã confirm thành công bạn có thể đăng nhập lại.");
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
            var listUserLogin = _userRepo.GetMulti(c => c.Username.ToLower().Equals(userDto.Username.ToLower()), includes).ToList();
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
                return StatusCode(404, "Tài khoản không tồn tại trong hệ thống.");
            }




            return _mapper.Map<ViewUserDto>(userLogin);
        }


    }
}
