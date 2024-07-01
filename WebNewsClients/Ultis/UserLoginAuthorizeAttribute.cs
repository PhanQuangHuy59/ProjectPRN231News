using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebNewsAPIs.Common;
using WebNewsAPIs.Dtos;
using WebNewsAPIs.Services;

namespace WebNewsClients.Ultis
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class UserLoginAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {


            ViewUserDto userLogin = InformationLogin.getUserLogin(filterContext.HttpContext);
            if (userLogin != null)
            {
                // Extract the JWT token from the cookie   
                filterContext.Result = null;
            }
            else
            {
                // If no token is found, set the filterContext.Result to a HttpUnauthorizedResult
                filterContext.Result = new RedirectResult("~/Home/Error400", true);
            }
        }

    }
}
