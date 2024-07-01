using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace WebNewsAPIs.Extentions
{
	public static class AuthenAuthorExtention
	{
		public static  void ConfigAuthenAuthor(this WebApplicationBuilder builder)
		{
			builder.Services.AddAuthorization();
			builder.Services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(x =>
			{
				
				var key = builder.Configuration.GetSection("JwtSetting:Key").Value;
				x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
				{

					ValidIssuer = builder.Configuration.GetSection("JwtSetting:Issuer").Value,
					ValidAudience = builder.Configuration.GetSection("JwtSetting:Audience").Value,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
				};
			});
		}
	}
}
