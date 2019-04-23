#region Using

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using VuBongBongWeb.Security;
using Library.Entity.Base;
using Library.Resource;
using log4net;
using VuBongBongWeb.App_Helpers;
using VuBongBongWeb.Management.WebCenterManagement;
using VuBongBongWeb.Security;
using MainLibrary.Entity.WebCenter;
using MainLibrary.Resource.WebCenter;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;

#endregion

namespace VuBongBongWeb
{
    public class UserManager : IUserManager
    {
        public const string UserNameCookie = "_UserNameCookie";
        //private static readonly UserStore<IdentityUser> UserStore = new UserStore<IdentityUser>();
        public static readonly UserManager Instance = new UserManager();

        private UserManager()
        {
        }

        //public static UserManager Create()
        //{
        //    //// We have to create our own user manager in order to override some default behavior:
        //    ////
        //    ////  - Override default password length requirement (6) with a length of 4
        //    ////  - Override user name requirements to allow spaces and dots
        //    //var passwordValidator = new MinimumLengthValidator(4);
        //    //var userValidator = new UserValidator<IdentityUser, string>(Instance)
        //    //{
        //    //    AllowOnlyAlphanumericUserNames = false,
        //    //    RequireUniqueEmail = true,
        //    //};

        //    //Instance.UserValidator = userValidator;
        //    //Instance.PasswordValidator = passwordValidator;

        //    return new UserManager();
        //}

        /// <summary>
        /// CreateAuthenticationTicket
        /// </summary>
        /// <param name="iAttendanceId"></param>
        public HttpCookie CreateAuthenticationTicket(User userDetail,
            int roleId, string roleName, string token,
            string avatarUrl, string profileImgUrl)
        {
            DateTime loginTime = DateTime.Now;
            WebPrincipalSerializeModel serializeModel = new WebPrincipalSerializeModel();

            serializeModel.AvatarUrl = avatarUrl;
            serializeModel.UserDetail = userDetail;
            //serializeModel.EmployeeId = employeeDetail.Id;
            serializeModel.ProfileImgUrl = profileImgUrl;
            //serializeModel.RoleId = roleId;
            //serializeModel.RoleName = roleName;
            //serializeModel.Token = token;
            serializeModel.LoginTime = loginTime;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string userData = serializer.Serialize(serializeModel);
            //string userData = JsonConvert.SerializeObject(serializeModel);

            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
              1, userDetail.UserName, loginTime, loginTime.AddHours(8), false, userData);
            string encTicket = FormsAuthentication.Encrypt(authTicket);
            HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);

            return faCookie;
        }

        public HttpCookie CreateEmployeeInfoTicket(User userDetail)
        {
            DateTime loginTime = DateTime.Now;
            User serializeModel = new User();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string userData = serializer.Serialize(userDetail);
            //string userData = JsonConvert.SerializeObject(userDetail);
            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
              1, userDetail.UserId.ToString(), loginTime, loginTime.AddHours(8), false, userData);
            string encTicket = FormsAuthentication.Encrypt(authTicket);
            HttpCookie faCookie = new HttpCookie(UserNameCookie, encTicket);

            return faCookie;
        }

        public bool GetAndCheckPermissionOnMenu(string controller, string action)
        {
            var log = LogManager.GetLogger(typeof(UserManager));
            try
            {
                var userModel = HttpContext.Current.User as WebPrincipal;
                bool hasPermission = false;

                #region Get menu items from server or session to check

                // Try to get menu items in session
                var menuItems = HttpContext.Current.Session[Resource.K_USER_MENU_LIST.ToString()] as IEnumerable<MenuItem>;

                // If out of session, get menu item again
                if (menuItems == null)
                {
                    //var data = new Hashtable();
                    //data.Add(Resource.K_ROLE_ID, userModel.RoleId);
                    var menuResult = new object();
                    using (var _repo = new WebBaseManager())
                    {
                        menuResult = _repo.GetMenuItemListByRole(userModel.UserDetail.RoleId);
                    }
                    if (menuResult != null)
                    {
                        menuItems = menuResult as IEnumerable<MenuItem>;
                        HttpContext.Current.Session[Resource.K_USER_MENU_LIST.ToString()] = menuItems;
                    }
                }

                if (menuItems != null)
                {
                    // Check each menu item
                    var checkMenuItems = new Queue<MenuItem>(menuItems);

                    while (checkMenuItems.Count != 0 && !hasPermission)
                    {
                        var menuItem = checkMenuItems.Dequeue();

                        if ((menuItem.MenuAction ?? string.Empty).ToLower().Equals(action.ToLower())
                            && (menuItem.MenuController ?? string.Empty).ToLower().Equals(controller.ToLower())
                            && menuItem.IsActive)
                        {
                            hasPermission = true;
                        }
                    }
                }

                #endregion

                return hasPermission;
            }
            catch (Exception ex)
            {
                log.Error("GetAndCheckPermissionOnMenu Error", ex);
            }
            return false;
        }

        public async Task<User> FindByUserNameAsync(string username, string password)
        {
            return await new Task<User>(() =>
            {
                var data = new User();
                using (var repo = new WebUserManager())
                {
                    data = repo.GetUserByUserName(username, password, out string error);
                }
                return data;
            });
        }

        public async Task<User> CreateUserAsync(User user)
        {
            return await new Task<User>(() =>
            {
                var data = new User();
                using (var repo = new WebUserManager())
                {
                    data = repo.CreateUser(user);
                }
                return data;
            });
        }
    }
}