using AutoMapper;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebNewsAPIs.Dtos;

namespace WebNewsAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserRepository _userRepo;
        private IMapper _mapper;
		private const string TokenScret = "ForTheLoveofFodStoreAndLoadThisSecurelyPhanQuangHuyBaViHaNoiForTheLoveofFodStoreAndLoadThisSecurelyPhanQuangHuyBaViHaNoi";
		private static readonly TimeSpan TokenLife = TimeSpan.FromHours(1);
		public UsersController(IUserRepository userRepo, IMapper mapper)
        {
            _userRepo = userRepo;
            _mapper = mapper;
        }

		[HttpPost("GenerationToken")]
		public string GenerationToken(ViewUserDto user)
		{
			var clams = new List<Claim>()
			{
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim(JwtRegisteredClaimNames.Sub, "phanhuy59@gmail.com"),
				new Claim(JwtRegisteredClaimNames.Email,"phanhuy59@gmail.com" ),
				new Claim("Username", user.Username, ClaimValueTypes.String),
				new Claim("Password", user.Password, ClaimValueTypes.String),
				new Claim(ClaimTypes.Role, user.RoleName,ClaimValueTypes.String)
			};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenScret));
			var keyScret = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

			var Token = new JwtSecurityToken(
				claims: clams,
				expires: DateTime.UtcNow.Add(TokenLife),
				issuer: "https://localhost:7058/",
				audience: "https://localhost:7058/",
				signingCredentials: keyScret
				);


			var jwt = new JwtSecurityTokenHandler().WriteToken(Token);
			return jwt;
		}
	}
}
