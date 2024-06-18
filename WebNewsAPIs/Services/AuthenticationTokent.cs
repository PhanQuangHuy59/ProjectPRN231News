using BusinessObjects.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebNewsAPIs.Dtos;

namespace WebNewsAPIs.Services
{
    public static class AuthenticationTokent
    {
        private static string TokenScret = "ForTheLoveofFodStoreAndLoadThisSecurelyPhanQuangHuyBaViHaNoiForTheLoveofFodStoreAndLoadThisSecurelyPhanQuangHuyBaViHaNoi";
        private static readonly TimeSpan TokenLife = TimeSpan.FromDays(1);
        public static string GenerationToken(ViewUserDto user)
        {
            var clams = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, "phanhuy59@gmail.com"),
                new Claim(JwtRegisteredClaimNames.Email,"phanhuy59@gmail.com" ),
                new Claim("Username", user.Username, ClaimValueTypes.String),
                new Claim("Password", user.Password, ClaimValueTypes.String),
                new Claim("Password", user.UserId.ToString(), ClaimValueTypes.String),
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

        public static Dictionary<string,string> GetPropertyInTokenJwt(string token)
        {
           var handler = new JwtSecurityTokenHandler();

            // validate the token and extract the claims
            var tokenValidationParams = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenScret))
            };

            try
            {
                var principal = handler.ValidateToken(token, tokenValidationParams, out SecurityToken validatedToken);
                var claims = principal.Claims;
                // iterate through the claims and extract the attributes you need
                Dictionary<string, string> claimInToken = new Dictionary<string, string>();

                foreach (var claim in claims)
                {
                    claimInToken.Add(claim.Type, claim.Value);
                }
                return claimInToken;
            }
            catch (SecurityTokenException ex)
            {
                Console.WriteLine("Invalid token: " + ex.Message);
            }
            return null;
        }

        public static string GetRoleInTokenJwt(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            // validate the token and extract the claims
            var tokenValidationParams = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenScret))
            };

            try
            {
                var principal = handler.ValidateToken(token, tokenValidationParams, out SecurityToken validatedToken);
                var claims = principal.Claims;
                var roleClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
                if (roleClaim != null)
                {
                    string role = roleClaim.Value;
                    return role;
                }
                else
                {
                    return null;
                }
            }
            catch (SecurityTokenException ex)
            {
                Console.WriteLine("Invalid token: " + ex.Message);
            }
            return null;
        }


    }
}
