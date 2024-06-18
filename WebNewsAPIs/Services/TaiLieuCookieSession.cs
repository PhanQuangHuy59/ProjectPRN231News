namespace WebNewsAPIs.Services
{
    public class TaiLieuCookieSession
    {
        public void TaiLieu(HttpContext context)
        {
            var cookieOptions = new CookieOptions
            {
                // Set the cookie properties 
                Path = "/",
                Expires = DateTime.UtcNow.AddDays(7),
                Secure = true, // Use "false" if not using HTTPS 
                HttpOnly = true,
                SameSite = (Microsoft.AspNetCore.Http.SameSiteMode)SameSiteMode.Strict
            };
            var cookieValue = context.Request.Cookies["myKey"];
            context.Response.Cookies.Append("myKey", "myValue", cookieOptions);



            
        }

    }
}
