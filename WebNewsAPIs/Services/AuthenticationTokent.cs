using BusinessObjects.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.Eventing.Reader;
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
        public static string GenerationToken(User user)
        {

            var context = new FinalProjectPRN231Context();
            var roleOfUser = context.Roles.FirstOrDefault(c => c.RoleId.Equals(user.Roleid));
            if (roleOfUser == null)
            {
                throw new Exception("Role not found");
            }
            var clams = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, "phanhuy59@gmail.com"),
                new Claim(JwtRegisteredClaimNames.Email,"phanhuy59@gmail.com" ),
                new Claim(nameof(ViewUserDto.UserId), user.UserId.ToString(), ClaimValueTypes.String),
                new Claim(nameof(ViewUserDto.Username), user.Username, ClaimValueTypes.String),
                new Claim(nameof(ViewUserDto.DisplayName), user.DisplayName, ClaimValueTypes.String),
                new Claim(ClaimTypes.Role,roleOfUser.Rolename,ClaimValueTypes.String)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenScret));
            var keyScret = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var Token = new JwtSecurityToken(
                claims: clams,
                expires: DateTime.UtcNow.Add(TokenLife),
                issuer: "https://localhost:7251/",
                audience: "https://localhost:7251/",
                signingCredentials: keyScret
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(Token);
            return jwt;
        }

        public static Dictionary<string, string> GetPropertyInTokenJwt(string token)
        {
            var handler = new JwtSecurityTokenHandler();

           
            // validate the token and extract the claims
            try
            {
                var claims = GetClaimsFromToken(token);
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

        public static IEnumerable<Claim> GetClaimsFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(TokenScret);

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // Optionally set ValidateLifetime to false if you don't want to validate the token's expiration
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return principal.Claims;
            }
            catch (Exception)
            {
                // Handle validation failure
                return Enumerable.Empty<Claim>();
            }
        }


        public static string GetRoleInTokenJwt(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            var tokenValidationParams = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "https://localhost:7058/", // Đảm bảo đúng issuer
                ValidAudience = "https://localhost:7058/", // Đảm bảo đúng audience
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenScret)),
                ClockSkew = TimeSpan.Zero // Tùy chỉnh để giảm thiểu thời gian chênh lệch
            };

            try
            {
                var principal = handler.ValidateToken(token, tokenValidationParams, out SecurityToken validatedToken);
                var claims = principal.Claims;
                var roleClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
                return roleClaim?.Value; // Trả về giá trị role nếu tồn tại, null nếu không
            }
            catch (SecurityTokenExpiredException ex)
            {
                return ("Error : Token expired: " + ex.Message);
            }
            catch (SecurityTokenValidationException ex)
            {
                return ("Error : Token validation failed: " + ex.Message);
            }
            catch (Exception ex)
            {
                return ("Error : An error occurred: " + ex.Message);
            }

            return null;
        }

        public static string GeneraterCodeTokent(User user)
        {
            var code = GenerationToken(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            return code;
        }
        public static Guid ConfirmEmail(User user, string code)
        {
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            Dictionary<string, string> valuePairClaim = GetPropertyInTokenJwt(code);

            string? userName = valuePairClaim.ContainsKey(nameof(User.Username)) ? valuePairClaim[nameof(User.Username)] : null;
            string? userId = valuePairClaim.ContainsKey(nameof(User.UserId)) ? valuePairClaim[nameof(User.UserId)] : null;
            string userIdNull = "00000000-0000-0000-0000-000000000000";
            if (userName != null && userId != null)
            {
                if (!user.UserId.ToString().Equals(userId))
                {
                    return Guid.Parse(userIdNull);
                }
                if (!user.Username.ToString().Equals(userName))
                {
                    return Guid.Parse(userIdNull);
                }
            }

            return user.UserId;
        }

    }
}
