using Core.Models;
using Logic;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Logic
{

    public static class AppHttpContext
    {
        static IServiceProvider services = null;

        /// <summary>
        /// Provides static access to the framework's services provider
        /// </summary>
        public static IServiceProvider Services
        {
            get { return services; }
            set
            {
                if (services != null)
                {
                    throw new Exception("Can't set once a value has already been set.");
                }
                services = value;
            }
        }

        /// <summary>
        /// Provides static access to the current HttpContext
        /// </summary>
        public static HttpContext Current
        {
            get
            {
                IHttpContextAccessor httpContextAccessor = services.GetService(typeof(IHttpContextAccessor)) as IHttpContextAccessor;
                return httpContextAccessor?.HttpContext;
            }
        }

    }

}
public class Session
{
    public static ApplicationUser GetCurrentUser()
    {
        var user = AppHttpContext.Current.Session.GetString("user");
        if (user != null)
        {
            var login = JsonConvert.DeserializeObject<ApplicationUser>(user);
            return login;
        }
        return null;
    }


    public static string GetRoleLayout()
    {
        var loggedInUser = GetCurrentUser();
        if (loggedInUser != null)
        {
            var superAdmin = loggedInUser.Roles.Contains(Session.Constants.SuperAdminRole);
            if (superAdmin)
            {
                return Constants.SuperAdminLayout;
            }
            else
            {
                var isSchoolAdmin = loggedInUser.Roles.Contains(Session.Constants.SchoolAdminRole);
                if (isSchoolAdmin)
                {
                    return Constants.SchoolAdminLayout;
                }
                else
                {
                    var isUser = loggedInUser.Roles.Contains(Session.Constants.UserRole);
                    if (isUser)
                    {
                        return Constants.UserLayout;
                    }
                }
            }
        }
        return Session.Constants.UserLayout;
    }

    public static class Constants
    {
        public static string SchoolAdminRole = "SchoolAdmin";
        public static string SuperAdminRole = "SuperAdmin";
        public static string UserRole = "User";
        public static string DefaultLayout = "~/Views/Shared/_Layout.cshtml";
        public static string SuperAdminLayout = "~/Views/Shared/_SuperAdminLayout.cshtml";
        public static string SchoolAdminLayout = "~/Views/Shared/_SchoolAdminLayout.cshtml";
        public static string UserLayout = "~/Views/Shared/_UserLayout.cshtml";
    }
}