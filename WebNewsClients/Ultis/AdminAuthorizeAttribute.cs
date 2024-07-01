
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebNewsAPIs.Common;
using WebNewsAPIs.Dtos;
using WebNewsAPIs.Services;

namespace WebNewsClients.Ultis
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class AdminAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {


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
            return userLogin.RoleName.ToUpper().Equals("ADMIN");
        }
    }
}
