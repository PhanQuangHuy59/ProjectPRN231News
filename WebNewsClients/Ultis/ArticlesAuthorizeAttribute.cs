using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebNewsAPIs.Common;
using WebNewsAPIs.Dtos;
using WebNewsAPIs.Services;

namespace WebNewsClients.Ultis
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class ArticlesAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            // Extract the JWT token from the cookie
          
            if (ValidateToken(filterContext.HttpContext))
            {
                filterContext.Result = null;
            }
            else
            {
                filterContext.Result = new RedirectResult("~/Home/Error400", true);
            }
        }
        
        private bool ValidateToken(HttpContext context)
        {
            ViewUserDto userLogin = InformationLogin.getUserLogin(context);
            return userLogin.RoleName.ToUpper().Equals("ARTICLES");
        }
    }
}
