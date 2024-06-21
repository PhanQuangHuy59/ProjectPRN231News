using Newtonsoft.Json;
using WebNewsAPIs.Common;
using WebNewsAPIs.Dtos;

namespace WebNewsClients.Ultis
{
    public static class InformationLogin
    {
        public static ViewUserDto getUserLogin(HttpContext httpContext)
        {
            string serial = httpContext.Session.GetString(SaveKeySystem.userLogin);
            try
            {
                var userLogin = JsonConvert.DeserializeObject<ViewUserDto>(serial);
                return userLogin;
            }
            catch (Exception ex)
            {

                return null;
            }

            return null;
        }
    }
}
