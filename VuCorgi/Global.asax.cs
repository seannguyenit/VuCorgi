using VuBongBongWeb;
using VuBongBongWeb.Security;
using MainLibrary.Entity.WebCenter;
using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace VuBongBongWeb
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private object _locker = new object();
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_BeginRequest(object source, EventArgs e)
        {
            lock (_locker)
            {
                if (!Settings.IsSyncSiteAndMenu)
                {
                    var isConfigSaved = Settings.SetValue<bool>(nameof(Settings.IsSyncSiteAndMenu), "1");
                    var isMenuSync = MenuSyncConfig.RegisterMenuSyncConfig();
                    if (!isMenuSync)
                    {
                        Settings.SetValue<bool>(nameof(Settings.IsSyncSiteAndMenu), "0");
                    }
                }
            }
        }

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            HttpCookie employeeInfoCookie = Request.Cookies[UserManager.UserNameCookie];
            if (authCookie != null && employeeInfoCookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                FormsAuthenticationTicket authTicketEmployeeInfo = FormsAuthentication.Decrypt(employeeInfoCookie.Value);

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                if (authTicket.UserData == "OAuth") return;
                WebPrincipalSerializeModel serializeModel = serializer.Deserialize<WebPrincipalSerializeModel>(authTicket.UserData);
                User user = serializer.Deserialize<User>(authTicketEmployeeInfo.UserData);
                WebPrincipal newUser = new WebPrincipal(authTicket.Name);
                newUser.AvatarUrl = serializeModel.AvatarUrl;
                if (user != null && user.UserId == serializeModel.UserDetail.UserId)
                    newUser.UserDetail = user;
                else return;
                //newUser.MenuItems = serializeModel.MenuItems;
                newUser.ProfileImgUrl = serializeModel.ProfileImgUrl;
                newUser.UserDetail.RoleId = serializeModel.UserDetail.RoleId;
                newUser.UserDetail.RoleName = serializeModel.UserDetail.RoleName;
                newUser.UserDetail.Token = serializeModel.UserDetail.Token;
                newUser.LoginTime = DateTime.Now;

                HttpContext.Current.User = newUser;
            }
        }
    }
}
