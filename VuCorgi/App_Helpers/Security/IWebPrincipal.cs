using MainLibrary.Entity.WebCenter;
using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace VuBongBongWeb.Security
{
    interface IWebPrincipal : IPrincipal
    {
        User UserDetail { get; set; }
        //int RoleId { get; set; }
        //string RoleName { get; set; }
        //string Token { get; set; }
        string AvatarUrl { get; set; }
        string ProfileImgUrl { get; set; }
        DateTime LoginTime { get; set; }
        IEnumerable<MenuItem> MenuItems { get; set; }
    }
}