using MainLibrary.Entity.WebCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VuBongBongWeb.App_Helpers
{
    public interface IUserManager
    {
        HttpCookie CreateAuthenticationTicket(User userDetail,
            int roleId, string roleName, string token,
            string avatarUrl, string profileImgUrl);

        bool GetAndCheckPermissionOnMenu(string controller, string action);
    }
}